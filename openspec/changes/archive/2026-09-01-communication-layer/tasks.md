## 1. API — Threads

- [x] 1.1 Create `GetThreadUseCase` — load thread + messages with student/teacher participant auth
- [x] 1.2 Add `GET /api/threads/{threadId}` to `AnswerThreadsController`
- [x] 1.3 Fix `PostThreadMessageUseCase` — participant auth + locked thread returns 400
- [x] 1.4 Expose `threadId` per answered question in teacher `SessionQuestionsDto` and student `StudentAnswerDto`

## 2. API — Check-Question Bank

- [x] 2.1 Create `ListCheckQuestionsUseCase` with read-time orphan detection (teacher's own, ordered)
- [x] 2.2 Create `GetCheckQuestionUseCase` with orphan detection (ownership check)
- [x] 2.3 Create `UpdateCheckQuestionUseCase` (ownership check)
- [x] 2.4 Create `DeleteCheckQuestionUseCase` (ownership check)
- [x] 2.5 Add `GET` (list), `GET {id}`, `PUT {id}` endpoints to `TeacherCheckQuestionsController`; wire `DELETE` to the use case

## 3. UI — Services

- [x] 3.1 Create `threadApi.ts` — `getThread`, `postThreadMessage` (with `threadId` invalidation)
- [x] 3.2 Create `checkQuestionApi.ts` — list/get/create/update/delete
- [x] 3.3 Add `Threads` + `CheckQuestions` tagTypes to `apiSlice.ts`

## 4. UI — Thread Panels

- [x] 4.1 Create `ThreadPanel` component — message list + composer (disabled when locked)
- [x] 4.2 Integrate `ThreadPanel` into teacher `ReviewPage` per answered question
- [x] 4.3 Integrate `ThreadPanel` into student `AttemptPage` per answered question

## 5. UI — Check-Question Bank Page

- [x] 5.1 Create `/teacher/check-questions` page — list with orphan indicator, create/edit/delete
- [x] 5.2 Add route + sidebar link; wire to checkQuestionApi

## 6. Verification

- [x] 6.1 Run `dotnet build` and `dotnet test` in API project
- [x] 6.2 Run `pnpm type-check`, `pnpm lint`, `pnpm build` in UI project
- [x] 6.3 Manually verify exit flow: teacher posts check-question → student replies → review locks thread

## 7. Real-time Thread Updates

- [x] 7.1 Hub: add `JoinThread`/`LeaveThread` with participant auth
- [x] 7.2 Controller: push `ThreadMessagePosted` to thread group after posting
- [x] 7.3 Client: `useThreadHub` shared connection + refetch on new message
