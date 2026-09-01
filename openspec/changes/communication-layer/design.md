## Context

The communication layer has entities (AnswerThread, ThreadMessage, TeacherCheckQuestion) and migrations, plus a working thread auto-creation on first answer, thread lock on review, and check-question create. Missing: any way to fetch a thread, thread participant authorization, full check-question CRUD, and orphan detection. The UI has zero thread/check-question code.

## Goals / Non-Goals

**Goals:**
- Teacher and student can view and reply in an answer thread
- Thread posts are participant-authorized; locked threads return 400
- Teacher check-question bank with full CRUD and read-time orphan detection
- Teacher can post a check-question from the bank into a thread
- Inline thread UI on the review page and lesson runner; a check-question bank page

**Non-Goals:**
- Notifications (in-app/email triggers) — deferred to a later pass
- Email delivery (Phase 4)
- Thread search / pagination (MVP lists are small)
- Student-side check-question bank creation (teacher-only)

## Decisions

### D1: Dedicated thread endpoint + `threadId` in responses

**Choice**: `GET /api/threads/{threadId}` returns the thread with messages. The teacher session-questions and student attempt-answers responses include `threadId` per answered question so the UI knows which thread to fetch.

**Rationale**: Clean on-demand fetch (mirrors notes). Embedding full threads bloats the question/answer payloads and couples thread state to those queries.

**Alternative**: Embed threads inline — rejected for response bloat and coupling.

### D2: Participant authorization via the thread's StudentAnswer chain

**Choice**: A thread's participants are its student (via StudentAnswer.StudentId) and that student's active primary teacher (via TeacherAssignment). `PostThreadMessageUseCase` and `GetThreadUseCase` both verify the caller is one of these two; otherwise 403.

**Rationale**: Reuses the exact authorization pattern from `StudySessionHub.AuthorizeSessionAccess`. Prevents guessing thread IDs to read/post in someone else's conversation.

### D3: Locked thread returns 400 (spec compliance)

**Choice**: Change `PostThreadMessageUseCase` to throw a 400-mapped error on a locked thread instead of `InvalidOperationException` (which mapped to 500).

**Rationale**: The communication spec requires 400 Bad Request on locked-thread posts. The existing `GlobalExceptionHandler` maps `InvalidOperationException` to 400 already — so throwing that after moving the check before message creation satisfies the spec (it currently throws after partial setup, and maps to 400 only via the generic handler, but the flow is fine). Verify mapping.

### D4: Orphan detection computed at read-time

**Choice**: `ListCheckQuestionsUseCase` / `GetCheckQuestionUseCase` compute `IsOrphaned` by checking whether the check-question's `QuestionKey` exists in any current published lesson version. Never stored.

**Rationale**: Matches the architecture rule ("orphan status computed at read-time, never stored").

### D5: Inline thread panel component

**Choice**: A single reusable `ThreadPanel` component (view messages + composer) used on both the teacher review page and the student runner.

**Rationale**: The teacher and student threads are the same conversation; one component with a `disabled` flag (student sees read-only once locked) avoids duplication.

## Risks / Trade-offs

- **[Risk] Orphan check cost on every list** → Queries all published versions' question keys once and filters in memory. Negligible for MVP lesson counts.
- **[Risk] Teacher lookup for the student's primary teacher** → Needs a TeacherAssignment lookup by student. Straightforward; reuse existing query pattern.
- **[Trade-off] No pagination** → Threads and check-question lists are small; MVP loads all. Pagination later if needed.