## Why

The communication layer is only ~25% implemented. Answer threads can be posted to but never fetched — neither the teacher nor student can see a thread. The check-question bank has create-only (delete is a no-op stub, no list/get/update), and orphan detection is entirely missing. The Phase 3b exit criterion — "teacher pre-loads a check-understanding question, student replies, thread locks when marked reviewed live" — is not achievable end-to-end today.

## What Changes

- `GET /api/threads/{threadId}` — fetch a thread with its messages (participant-authorized)
- `POST /api/threads/{threadId}/messages` — fix participant authorization and return 400 on locked threads
- Thread ID exposed per answered question in teacher session-questions and student attempt-answers responses
- Check-question bank full CRUD: `GET` list (with read-time orphan detection), `GET {id}`, `PUT {id}`, fix `DELETE` stub
- UI: inline thread panel on the teacher review page, inline thread panel on the student lesson runner, and a teacher check-question bank page

## Capabilities

### Modified Capabilities
- `communication`: Adds thread fetching, thread participant authorization, locked-thread 400, and full check-question CRUD with orphan detection.
- `ui`: Adds inline thread panels to the teacher review and student runner, and a teacher check-question bank page.

## Impact

- **New use cases**: `GetThreadUseCase`, `ListCheckQuestionsUseCase`, `GetCheckQuestionUseCase`, `UpdateCheckQuestionUseCase`, `DeleteCheckQuestionUseCase`
- **Modified use case**: `PostThreadMessageUseCase` (participant auth + locked→400)
- **Modified controllers**: `AnswerThreadsController` (GET thread), `TeacherCheckQuestionsController` (list/get/update/delete)
- **Modified DTOs**: session-questions + attempt-answers gain `threadId`; check-question DTO orphan computation
- **UI**: `ThreadPanel` component, check-question bank page, services (threadApi, checkQuestionApi)
- **No new entities or migrations**