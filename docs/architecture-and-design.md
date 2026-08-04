# The Temple Courts — MVP Architecture & Design

## 0. Product Context

**The Temple Courts** is an all-in-one Bible study and community application, built with a traditional, scripturally grounded tone. The name is drawn from Luke 2:46 — the young Jesus listening and asking questions among the teachers in the temple courts — a direct thematic fit for the app's core pedagogical design.

### Guided Discovery Principle (non-negotiable core constraint)

The app teaches by **leading the truth-seeker to discover biblical truth themselves** — it never states or dictates conclusions. Concretely:

- The Teacher role asks a sequence of questions and points to related Scripture, rather than explaining, summarizing, or declaring conclusions.
- Verses are surfaced as **evidence to examine**, not pre-interpreted content.
- The standard for all content is the **biblical author's (and God's) intended meaning** of a passage — never the personal opinion or inference of any teacher, encourager, or the app itself.

This principle governs lesson content, question flow, and Teacher-role authoring throughout the entire application — every design decision below is expected to preserve it.

### Roles

- **Student** — works through lessons, answers questions, reads Scripture.
- **Teacher** — walks a student through a lesson live, asks questions, never declares answers.
- **Encourager** — _deferred to post-MVP_.

Studies happen face-to-face or via call; the app is the shared content and review interface, not a video platform (see §11).

---

## 1. MVP Scope

### In Scope

- Email + Google account registration/login
- Admin: full lesson authoring, dynamic recursive lesson structure, versioning
- Student: answer questions (free order, with optional sibling-gating), save/resume, submit
- Teacher: live guided review session (`StudySession`), async prep via chat threads
- Teacher/Student notes: check-understanding bank, per-answer chat, personal question notes
- Admin: elevate user to Teacher, assign/reassign Student↔Teacher (1:1), oversight
- Appointment scheduling (basic CRUD; external meeting link only — no embedded video)
- Notifications (in-app + email)

### Out of Scope (deferred, not designed against)

- Encourager role
- Co-teacher, group/class studies (MVP is strictly 1 Teacher : 1 Student)
- Embedded video calling (Google Meet Add-on SDK or custom WebRTC — parked, see §11)
- Multiple lesson attempts per student
- Reporting/analytics dashboards
- Offline mode
- Licensed Bible verse content/lookup service (verse info is free-text only in MVP)

---

## 2. Core Roles & Permissions

| Action                                               | Admin                  | Teacher                            | Student                                |
| ---------------------------------------------------- | ---------------------- | ---------------------------------- | -------------------------------------- |
| Manage lessons/questions/versions                    | ✅                     | ❌                                 | ❌                                     |
| Elevate user to Teacher                              | ✅                     | ❌                                 | ❌                                     |
| Assign/reassign Student↔Teacher                      | ✅ (any)               | ✅ (claim unassigned student only) | ❌                                     |
| See `reference_context` (expected-answer / guidance) | ✅                     | ✅                                 | ❌ never sent to student client        |
| Answer lesson questions                              | ❌                     | (has own copy, doesn't answer)     | ✅                                     |
| View student answers                                 | ❌ unless also teacher | own students only                  | own only                               |
| Mark an answer "reviewed"                            | ❌                     | ✅ — **live session only**         | ❌                                     |
| Post/reply in an answer's chat thread                | ❌                     | ✅ anytime thread is open          | ✅ anytime thread is open              |
| Add reusable check-understanding note                | ❌                     | ✅ (bank, tied to master question) | ❌                                     |
| Add personal note on a question                      | ❌                     | ❌                                 | ✅ always, regardless of review status |
| Create/manage appointments                           | ❌                     | ✅                                 | (request only, future)                 |

---

## 3. Lesson Structure — Dynamic Recursive Tree

Unlike a fixed `Lesson → Part → Section → Question` hierarchy, lesson structure is **dynamic**: a lesson can be `Lesson → Section → Question`, `Lesson → Section → Sub-Section → Question`, or any mix, up to a bounded depth. This is modeled as a single self-referencing node type rather than separate `Part`/`Section` tables.

### Structural Rules

- A `Lesson` **cannot** hold `Question`s directly — every lesson requires at least one `LessonNode` level.
- **Minimum nesting: 1 level.** A `LessonVersion` must have at least one top-level `LessonNode` before it can be published.
- **Maximum nesting: 3 levels deep.**
- Every `LessonNode` requires, at minimum, a **title** and a **description** (free text — this is also where verse references/context now live, e.g. "Covers John 3:16–18," since there's no separate structured verse field in MVP).
- **Leaf-only questions:** a `LessonNode` holds either child nodes _or_ questions — never both. This keeps traversal order unambiguous.

```
LessonNode
- id
- lesson_version_id
- parent_node_id (nullable — null = top-level node directly under the Lesson)
- node_key (stable UUID — survives version bumps)
- depth (1, 2, or 3 — enforced max)
- order (sibling order under the same parent)
- title (required)
- description (required — free text, includes verse reference/context)
- requires_prior_sibling_answered: boolean

Question
- id
- lesson_node_id (FK, required — a Question always attaches to a leaf LessonNode, never to a Lesson directly)
- question_key (stable UUID)
- order
- question_type: YES_NO | TRUE_FALSE | FILL_BLANK | SELECT_EMBEDDED | ESSAY
- prompt_text
- metadata (JSON, type-specific config)
- reference_context (JSON, nullable — guidance/expected answer; NEVER served to STUDENT role)
```

### Enforcement

| Rule                                    | Enforced at                                                                                                                  |
| --------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------- |
| Lesson has no direct questions          | `Question.lesson_node_id` is a required FK; no path exists to attach a question elsewhere                                    |
| Minimum 1 top-level node before publish | Publish-time check on `LessonVersion`                                                                                        |
| Maximum depth 3                         | `POST /lesson-nodes` rejects if `parent.depth = 3`                                                                           |
| Leaf-only questions                     | Adding a question to a node with existing children is rejected; adding a child to a node with existing questions is rejected |

### Traversal & Gating

- The lesson's linear sequence (used for session walkthrough, Next/Previous, and completeness checks) is a **depth-first traversal** of `LessonNode`s ordered by `order` at each level; only leaf nodes contribute questions to the sequence.
- Computed **on read** from the tree — not stored as a separate materialized order — so reordering nodes can never drift out of sync with the displayed sequence.
- **Sibling gating (`requires_prior_sibling_answered`):** when set on a node, all `Question`s under the **immediately preceding sibling** (same `parent_node_id`, `order - 1`), including that sibling's full descendant tree, must be answered before this node's own questions can be answered. Enforced at `POST /student-answers`.

### Example structures this supports

- `Lesson → Section → Question` (depth 1)
- `Lesson → Section → Sub-Section → Question` (depth 2)
- `Lesson → Section → Sub-Section → Sub-Sub-Section → Question` (depth 3, ceiling)
- Mixed within the same lesson — some branches shallow, others deep — since nothing enforces uniform depth across the tree.

---

## 4. Full Data Model

```
User
- id, email, password_hash (nullable), google_id (nullable)
- display_name, status, created_at

RoleAssignment
- user_id, role (ADMIN | TEACHER | STUDENT)
- assigned_by, assigned_at

TeacherAssignment
- student_id, primary_teacher_id, assigned_by
- status (ACTIVE | ENDED)
- assigned_at, ended_at
-- modeled as history, never overwritten in place

Lesson
- id, number, title
- current_published_version_id (FK -> LessonVersion)
- status (ACTIVE | ARCHIVED)

LessonVersion
- id, lesson_id, version_number
- status (DRAFT | PUBLISHED | RETIRED)
- cloned_from_version_id (nullable)
- change_notes, published_at, created_at

LessonNode          -- see §3 for full definition
Question             -- see §3 for full definition

LessonAttempt
- id, student_id, lesson_id
- lesson_version_id -- pinned at creation, never changes for this attempt
- started_at, completed_at, status

StudentAnswer
- id, student_id, question_id, question_key (denormalized)
- lesson_attempt_id
- answer_value (JSON — one shape for all question types)
- question_prompt_snapshot, question_type_snapshot -- captured at answer time
- submitted_at
- reviewed: boolean
- reviewed_by, reviewed_at
- reviewed_in_session_id -- REQUIRED when reviewed = true

AnswerFlag
- id, student_id, question_id, lesson_attempt_id
- flag_type: UNANSWERED_AT_REVIEW
- raised_in_session_id
- resolved_at (nullable — auto-set when student submits the answer)

AnswerThread
- id, student_answer_id (1:1)
- status: OPEN | LOCKED
- locked_at

ThreadMessage
- id, answer_thread_id, author_id, body_text
- source_check_question_id (nullable FK -> TeacherCheckQuestion)
- created_at

TeacherCheckQuestion   -- reusable bank, keyed on question_key (survives version bumps)
- id, teacher_id, question_key
- note_text, created_at
-- "orphaned" status computed at read-time: question_key not present in
-- Lesson.current_published_version_id's question set. Never deleted, never stored as a flag.

StudentQuestionNote   -- student's own note, keyed on question_key
- id, student_id, question_key
- note_text, created_at
-- always postable/editable, independent of review or thread-lock status

StudySchedule (Appointment)
- id, teacher_assignment_id, scheduled_at, duration
- meeting_link (plain external URL, no embedding)
- status: PROPOSED | CONFIRMED | COMPLETED | CANCELLED
- created_by

StudySession
- id, appointment_id, lesson_attempt_id, sequence_number
- start_question_id, end_question_id (nullable)
- current_question_id
- status: NOT_STARTED | IN_PROGRESS | COMPLETED | ABANDONED
- started_at, ended_at

Notification
- id, recipient_id
- type: NEW_THREAD_MESSAGE | ANSWER_FLAGGED | TEACHER_ASSIGNED |
        APPOINTMENT_CREATED | APPOINTMENT_REMINDER | SESSION_STARTED
- reference_type, reference_id (polymorphic pointer)
- read_at (nullable)
- delivery_channel: IN_APP | EMAIL
- created_at
```

---

## 5. Key Design Decisions & Rules

### 5.1 Answer storage

Single `answer_value` JSON column across all 5 question types, keyed off `question_type` for rendering/validation — keeps adding a future question type cheap (no schema change).

### 5.2 No auto-grading, ever

There is no "correct/incorrect" concept anywhere, consistent with the Guided Discovery Principle. `reference_context` is guidance/expected-answer context for Admin/Teacher only — stripped at the **response-serialization layer**, never sent to a Student-role client at all.

### 5.3 Reviewed = live-session-only action

- `StudentAnswer.reviewed` can only be set `true` while `reviewed_in_session_id` points to a `StudySession` with `status = IN_PROGRESS`.
- Blocked entirely if `answer_value` is empty.
- Models the real-world flow: student reads verse aloud, gives an answer, teacher discusses (asking, never telling), marks reviewed — live only, never asynchronous.

### 5.4 Async is for conversation, not for marking review

- `AnswerThread` is `OPEN` the moment a `StudentAnswer` exists, regardless of session state — teacher can pre-load a check-understanding question anytime between sessions; student can reply anytime.
- Thread becomes `LOCKED` the moment `reviewed` flips to `true` — the natural "we've discussed this, moving on" boundary.
- `StudentQuestionNote` is separate from the thread entirely — always open, never locked, since it's the student's personal margin note, not a conversation expecting a reply.

### 5.5 Unanswered question during a live session

- Teacher is **blocked from marking reviewed** if unanswered, but **not blocked from advancing** the session to other questions.
- Student is **not blocked** from continuing to answer other questions in the lesson.
- System raises an `AnswerFlag`, visible to both parties until the student submits an answer, which auto-resolves it.

### 5.6 Sibling-level answer gating

Optional per-node rule, scoped to only the **immediately preceding sibling node** (not a cumulative check across all earlier nodes) — see §3 for full mechanics. Enforced at `POST /student-answers`, not just in the UI.

### 5.7 Hard stop before next lesson

A new `LessonAttempt` cannot be created if the student has **any unresolved `AnswerFlag`** (`resolved_at IS NULL`) from a prior lesson. This checks unresolved **flags**, not unreviewed answers — a lesson legitimately spans several live sessions with many answered-but-not-yet-reviewed questions in normal progress, and that must never block lesson progression.

### 5.8 Multi-session lessons

- A `Lesson` typically takes several live sessions to fully walk through.
- Each live session is its own `StudySession`, sequenced (`sequence_number`) against the same `LessonAttempt`.
- A new session defaults `start_question_id` to the question right after the previous session's `end_question_id` (tree traversal order), with the teacher able to jump/override.
- "Lesson review-complete" is a computed rollup: every `StudentAnswer` under the `LessonAttempt` is `reviewed = true`.

### 5.9 Lesson lifecycle & versioning

Two update paths:

**A. In-place patch** (no new version) — cosmetic/non-semantic only (typo fixes). Admin confirms explicitly; logged as an edit history entry.

**B. New draft version** — for anything that changes actual content/structure/meaning (add/remove node or question, change type/metadata/expected answer, reorder, restructure the tree). Workflow:

1. Admin creates a new `DRAFT` `LessonVersion`, deep-cloned recursively from the current `PUBLISHED` version (bounded, max 3 levels) — all `_key` fields preserved on unchanged items.
2. Admin edits freely; new items get new `_key`s.
3. Admin publishes → new version `PUBLISHED`, old version `RETIRED`, `Lesson.current_published_version_id` updated.

- **In-flight student protection:** `LessonAttempt.lesson_version_id` is set once at creation and never changes — a student mid-lesson keeps seeing exactly the content they started with, even if a new version publishes mid-way.
- **New attempts** always pick up whatever is currently `current_published_version_id`.
- **Stable identity (`_key`) vs. version-specific row (`id`):** `TeacherCheckQuestion` and `StudentQuestionNote` key on `question_key` (survives version bumps); `StudentAnswer` pins to the specific `question_id` it was answered against, protected further by denormalized prompt/type snapshots.
- **Orphaned bank notes:** if a question is retired in a new version, its `TeacherCheckQuestion` entries are **flagged, never deleted** — computed at read-time (question_key absent from the current published version), not stored as a boolean.

### 5.10 Reassignment history

`TeacherAssignment` is a history table (`status: ACTIVE/ENDED`), never a plain foreign key on Student — reassigning ends the old row and creates a new one, preserving who taught whom and when.

### 5.11 Bible verse content

No structured verse field in MVP — verse references/context live inside each `LessonNode.description` as free text.

**Future (post-MVP):** a thin `VerseContentService` interface (`getVerseText(reference, translation)`) can call out to a licensed Bible content provider for an on-demand pop-up, once a provider is chosen and its licensing terms (including caching/redistribution limits) are reviewed. Candidates to evaluate: API.Bible, ESV API, Bible Gateway, YouVersion.

---

## 6. Roadmap

### Phase 0 — Foundations

- Repo, CI/CD, environments
- Auth: email/password + Google OAuth
- Core schema: User, RoleAssignment, Lesson, LessonVersion, LessonNode, Question
- Admin-guard middleware; all role checks enforced server-side

**Exit criteria:** register/login works; admin can hit protected endpoints.

### Phase 1 — Lesson Authoring (recursive structure + versioning from the start)

- Admin CRUD: recursive `LessonNode` tree editor (add child node / add question at any leaf, up to depth 3)
- Question type selector + per-type config form (5 types)
- `reference_context` authoring field (admin/teacher-visible only)
- Sibling-gating checkbox (`requires_prior_sibling_answered`)
- Draft-clone / patch-vs-version-bump admin workflow
- Publish/draft/retired lifecycle, minimum-1-node-before-publish check
- Seed the existing 16 lessons
- `Notification` schema lands here (delivery wiring comes later)

**Exit criteria:** admin builds and publishes a full lesson of arbitrary depth (≤3 levels), retrievable via API in student-safe shape (no `reference_context` leak); admin can create a new draft version and publish it without breaking existing attempts.

### Phase 2 — Student Experience

- Lesson runner: depth-first tree navigation, all 5 input types, works regardless of structure depth
- Answer save/autosave/submit, `LessonAttempt` tracking (pinned to `lesson_version_id`)
- Sibling-gating enforcement — core answering logic, not deferred
- `StudentQuestionNote` — ships here since it has no teacher/session dependency

**Exit criteria:** student completes a lesson end-to-end, including a gated node, across at least two different structural depths (e.g. one depth-1 lesson, one depth-2 lesson), and can leave notes.

_(Phase 1 and Phase 2 can run in parallel after Phase 0.)_

### Phase 3a — Review Core

- `StudySession`: start, advance, multi-session resumption per `LessonAttempt`, tree-traversal-based question sequencing
- `AnswerFlag`: create on unanswered-at-review, auto-resolve on answer submit
- Live-only `reviewed` toggle with hard gate
- Hard stop on next-`LessonAttempt` creation when unresolved flags exist
- Admin: elevate to Teacher; Teacher: claim unassigned student; Admin: reassign (history-preserving)

**Exit criteria:** teacher runs a full multi-session walkthrough of a lesson with a real student; flags surface correctly; reassignment doesn't break history; unresolved flag correctly blocks starting the next lesson.

### Phase 3b — Communication Layer

- `TeacherCheckQuestion` bank CRUD (keyed on `question_key`, orphan status computed at read-time)
- `AnswerThread` + `ThreadMessage`: post/reply, lock on `reviewed`, post-bank-question-into-thread
- Notification triggers: new thread message (both directions), flag raised, teacher assigned, reassignment

**Exit criteria:** teacher pre-loads a check-understanding question async, student replies, thread locks the moment it's marked reviewed live.

### Phase 4 — Scheduling & Notification Polish

- `StudySchedule` CRUD (create/confirm/cancel), external `meeting_link` field only
- Notification delivery: appointment created/reminder
- Full email delivery wiring for all Phase 1–3b triggers

**Exit criteria:** teacher schedules a session, both parties get emailed, link is external (no embedding).

### Phase 5 — Hardening & Launch

- Full 16-lesson content load in staging with real users, across varying structural depths
- Security pass: role-check coverage, `reference_context` leak testing, OAuth flow
- Audit log on: role elevation, reassignment
- Regression tests:
  - Student answers out of order, gets reassigned twice — thread/flag history stays correctly attributed
  - New `LessonVersion` published mid-attempt — in-flight student unaffected
  - Unresolved flag blocks next lesson; resolving it unblocks
  - Depth-3 lesson with mixed-depth branches traverses and gates correctly

---

## 7. Notification Triggers (MVP)

| Event                                  | Notify                            | Channel                 |
| -------------------------------------- | --------------------------------- | ----------------------- |
| Teacher posts message in AnswerThread  | Student                           | in-app + email          |
| Student replies in AnswerThread        | Teacher                           | in-app + email          |
| AnswerFlag raised                      | Student                           | in-app (email optional) |
| Student assigned to Teacher            | Both                              | email                   |
| Student reassigned                     | Student, old Teacher, new Teacher | email                   |
| Appointment created/confirmed          | Both                              | email                   |
| Appointment reminder (e.g. 24h before) | Both                              | email                   |
| StudySession started                   | Student                           | in-app/push             |

---

## 8. Things Identified as Missed / Worth Revisiting

- **Encourager's exact permissions** — deferred; needs resolution before that role is built (read-only observer vs. can leave notes vs. gets notifications).
- **Essay "completion" definition** — handled via the same `reviewed` flag as other types; no separate rubric/score in MVP.
- **Group/class studies** — MVP is strictly 1:1; generalizing `TeacherAssignment` later would touch several tables.
- **Minors / parental consent** — not addressed in MVP; worth revisiting given the likely age range of some students.
- **Search across lessons/questions** — not in MVP scope, likely needed as lesson count grows past 16.
- **Admin-level reporting/oversight dashboards** — deferred.
- **Bible content provider licensing** — needs vetting before the future verse pop-up feature is built.
- **Admin authoring UX for deep trees** — a 3-level recursive tree editor is a heavier UI lift than a fixed two-level form; worth a dedicated design pass in Phase 1.

---

## 9. Naming Notes

The name search (App Store, Play Store, domain availability) revealed heavy saturation in adjacent spaces — "Berean"-branded Bible apps and "Ask"-variant AI chatbot Bible apps are both crowded territories the app intentionally steers clear of. **The Temple Courts** was selected as a strong, uncrowded, and thematically resonant alternative.

---

## 10. Parked for Future Consideration — Video Integration

Two real architectural paths exist for tying video into the live session; **neither is in MVP** (MVP uses a plain external `meeting_link` field):

**Option A — Google Meet Add-on SDK.** The study tool runs _inside_ a real Google Meet call, as a side-panel or main-stage add-on. Google fully controls video tile layout — no custom fixed regions are possible inside Meet's own UI. Requires Google Workspace Marketplace registration/review.

**Option B — Custom WebRTC video** (e.g., Daily.co, Twilio Video, Agora, LiveKit) embedded directly in the app's own UI. This is the only path that achieves a fully custom layout (e.g., half lesson content, one-fourth student video, one-fourth teacher video), since video tiles are rendered as ordinary elements the app controls — but it is no longer "Google Meet," even though login can still use Google OAuth.

There is no combination that delivers both "actually Google Meet" and "arbitrary custom composite layout" — this decision is deferred to a future phase.

---

## 11. Document Status

This document reflects the full design as of the current planning session: role model, dynamic recursive lesson structure (up to 3 levels, leaf-only questions), all 5 question types, answer/review/flag workflow, async chat threads, multi-session live review, lesson versioning and lifecycle, scheduling, notifications, and an explicit MVP scope cut — all governed throughout by the Guided Discovery Principle.

**Suggested next steps:**

1. Full ER diagram reflecting this final model
2. REST API endpoint specification, phase by phase
3. Ticket/user-story breakdown for Phase 0–1
4. Admin-facing tree-authoring UI flow (draft creation → node/question editing → publish)
