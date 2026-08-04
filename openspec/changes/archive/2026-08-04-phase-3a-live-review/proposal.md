## Why

TeacherAssignment (Phase 3a-i) established the 1:1 Teacher↔Student link. Now the live review workflow needs the entities and rules that make it work: StudySession for question sequencing, AnswerFlag for unanswered-at-review tracking, the reviewed toggle (live-only, never async), and real-time sync via SignalR so Teacher and Student stay in lockstep during a session.

## What Changes

- Add `StudySession` entity with status lifecycle, question sequencing (tree-traversal), and multi-session resumption per LessonAttempt.
- Add `AnswerFlag` entity — raised when Teacher tries to mark an unanswered question as reviewed during a live session; auto-resolved when the student submits the answer.
- Modify `StudentAnswer` to add `reviewed`, `reviewed_by`, `reviewed_at`, `reviewed_in_session_id` (required when reviewed=true). Introduce live-only reviewed toggle with hard gate.
- Modify `StartLessonAttemptUseCase`: hard stop — block new LessonAttempt creation if any unresolved AnswerFlag exists for the student.
- Add SignalR hub for StudySession real-time sync between Teacher and Student.
- Teacher starts/stops/advances session; marks answers reviewed; flags unanswered questions.

## Capabilities

### New Capabilities
- `study-session`: StudySession lifecycle, tree-traversal-based question sequencing, multi-session resumption, live-only reviewed toggle, AnswerFlag.
- `live-review-signalr`: SignalR hub for real-time session state sync between Teacher and Student.

### Modified Capabilities
- `student-attempts`: Modify `StartLessonAttemptUseCase` — hard stop on unresolved AnswerFlags.
- `lesson-reading`: Modify `Student can start a lesson attempt` requirement — add unresolved-flag check.

## Impact

- **Domain**: New `StudySession` entity + `StudySessionStatus` enum. New `AnswerFlag` entity + `AnswerFlagType` enum. Modified `StudentAnswer` entity (4 new fields). Modified `LessonAttempt` entity (none, but use case logic changes).
- **Application**: New use cases `StartStudySession`, `EndStudySession`, `AdvanceStudySession`, `MarkAnswerReviewed`, `RaiseAnswerFlag`. Modified `StartLessonAttemptUseCase` (add flag check).
- **Infrastructure**: New `DbSet` entries, EF configs, migration. SignalR hub registration in DI.
- **API**: SignalR `StudySessionHub`. New `StudySessionsController`. Modified `StudentAttemptsController` (start attempt now checks flags).
- **BREAKING**: `StartLessonAttemptUseCase` now throws if unresolved AnswerFlags exist (behavior change).
- **NuGet**: Add `Microsoft.AspNetCore.SignalR` to API project.
