## Context

Phase 3a-ii builds on TeacherAssignment (3a-i) and StudentAnswer/LessonAttempt (Phase 2). A live StudySession is the real-time meeting where a Teacher walks a Student through lesson questions, marking answers as reviewed, flagging unanswered questions, and advancing through the lesson tree.

The Guided Discovery Principle governs every Teacher action: never telling conclusions, always asking. The reviewed toggle is the boundary between "we discussed this" and "moving on."

## Goals / Non-Goals

**Goals:**
- Teacher creates StudySession linked to a LessonAttempt
- Session sequences questions in tree-traversal order, resuming from where the last session left off
- Teacher marks a StudentAnswer as reviewed (only during IN_PROGRESS session)
- Unanswered-at-review → Auto-raises AnswerFlag
- Student submitting answer → Auto-resolves AnswerFlag
- New LessonAttempt blocked when any unresolved AnswerFlag exists
- SignalR hub broadcasts session state changes in real time

**Non-Goals:**
- Video/audio calling (external meeting_link only)
- Asynchronous review (reviewed can ONLY be set during live session)
- Auto-grading or correct/incorrect scoring
- Appointment scheduling (Phase 4)
- Notification triggers for session events (Phase 4)
- AnswerThread/TeacherCheckQuestion (Phase 3b)

## Decisions

### SignalR: teacher-owned state, student observes

**Decision:** The StudySession state lives on the server. Teacher actions (start, advance, mark reviewed) mutate state and broadcast to the connected student. Student actions (submit answer) also broadcast back. The session hub manages groups keyed by session ID.

**Rationale:** Simpler than full CRDT. Single source of truth (server). Teacher owns the session — student can't advance or mark reviewed.

### reviewed_in_session_id is REQUIRED for reviewed=true

**Decision:** `StudentAnswer.reviewed_in_session_id` is a non-nullable FK constraint when `reviewed = true`. Attempting to mark reviewed without an active StudySession throws.

**Rationale:** Per architecture doc: "Reviewed = live-session-only action." The FK constraint is the enforcement mechanism.

### AnswerFlag: single type for MVP

**Decision:** Only `UNANSWERED_AT_REVIEW` flag type. No other flag types in MVP.

**Rationale:** Per data model. Additional flag types are deferrable.

### Multi-session sequencing: computed on read

**Decision:** Session start_question_id defaults to the next question after the previous session's end_question_id (in tree-traversal order). Teacher can override.

**Rationale:** Per architecture doc §5.8. Computed from the tree, not a materialized sequence.

### Hard stop: check at StartLessonAttempt

**Decision:** When `StartLessonAttemptUseCase` runs, check if the student has ANY `AnswerFlag` with `resolved_at IS NULL` across ALL their prior attempts. If yes → block new attempt creation.

**Rationale:** Per architecture doc §5.7. Enforces lesson completion before moving to the next one.

## Risks / Trade-offs

- **SignalR disconnect**: If teacher or student drops connection during a session, the session stays IN_PROGRESS. Mitigation: the teacher can rejoin and continue; session end is an explicit action.
- **Race on reviewed**: Two rapid "mark reviewed" calls on the same answer could race. Mitigation: idempotent — second call is a no-op.
- **Tree traversal complexity**: Computing the linear sequence from a 3-level tree requires recursive traversal. Mitigation: depth capped at 3, simple to cache in a single query.
