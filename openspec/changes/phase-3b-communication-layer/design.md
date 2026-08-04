## Context

Phase 3a delivered live review (StudySession, AnswerFlag, reviewed toggle). Phase 3b adds the async communication layer: Teacher prep questions between sessions, thread-based conversations on student answers, and notification wiring. The thread locks when an answer is reviewed (live session boundary).

## Goals / Non-Goals

**Goals:**
- Teacher creates reusable check-understanding questions keyed on `question_key`
- Orphan detection: flag questions whose `question_key` isn't in the current published version
- AnswerThread auto-created when StudentAnswer is created
- Teacher/Student post messages in thread; thread locks when answer is reviewed
- Teacher can post a banked check question into a thread
- Notification created on new thread message

**Non-Goals:**
- Notification delivery (email/push) — Phase 4
- Complex threading (nested replies, reactions) — flat timeline only
- Read receipts

## Decisions

### Thread: auto-created on first answer

**Decision:** An `AnswerThread` is created when the first `StudentAnswer` is submitted for a question. No separate API to create threads.

**Rationale:** Per architecture doc: "AnswerThread is OPEN the moment a StudentAnswer exists." Threads mirror answers 1:1.

### TeacherCheckQuestion: orphan detection at read-time

**Decision:** No stored `orphaned` flag. At read-time, check if the `question_key` exists in the lesson's current published version's question set.

**Rationale:** Per architecture doc §5.9: "orphaned status computed at read-time, never stored as a boolean." Avoids stale flag issues.

### Lock on reviewed

**Decision:** When `StudentAnswer.reviewed` is set to true (via mark-reviewed in live session), the associated `AnswerThread` is locked.

**Rationale:** Per architecture doc §5.4: "Thread becomes LOCKED the moment reviewed flips to true."

### Notification creation: fire-and-forget

**Decision:** Create a `Notification` row synchronously during the use case execution. No event bus / outbox pattern.

**Rationale:** Simple for MVP. Email delivery is Phase 4. In-app notifications are queryable from the notifications table.

## Risks / Trade-offs

- **Thread locking race**: Mark-reviewed and thread-post could race. Mitigation: mark-reviewed sets the lock in the same use case before returning.
- **Orphan detection cost**: Requires querying the lesson's current version's question set. Mitigation: indexed, cheap for 16 lessons.
