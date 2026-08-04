## Why

After a live StudySession (Phase 3a-ii), the Teacher and Student need an async communication channel between sessions. TeacherCheckQuestion is a reusable bank of check-understanding prompts. AnswerThread + ThreadMessage is the conversation surface. And notifications need to fire when someone posts.

## What Changes

- Add `TeacherCheckQuestion` entity — reusable bank per-teacher, keyed on `question_key`. Orphan status computed at read-time.
- Add `AnswerThread` (1:1 with StudentAnswer) and `ThreadMessage` entities. Thread locks when answer is reviewed.
- Add notification creation on thread messages, flag raised, and teacher assignment events.
- Add API endpoints for threads, messages, and check questions.

## Capabilities

### New Capabilities
- `teacher-check-questions`: Bank CRUD, orphan detection at read-time.
- `answer-threads`: Thread + message CRUD, lock-on-reviewed, post-check-question-into-thread.

### Modified Capabilities
<!-- None. Existing notification-schema spec already covers triggers; we're implementing. -->

## Impact

- **Domain**: New entities `TeacherCheckQuestion`, `AnswerThread`, `ThreadMessage`. `AnswerThreadStatus` enum.
- **Application**: New use cases for check-question CRUD, thread CRUD, message post/reply, orphan status computation.
- **Infrastructure**: New DbSet entries, EF configs, migration.
- **API**: New `AnswerThreadsController`, `TeacherCheckQuestionsController`.
- **No breaking changes**.
