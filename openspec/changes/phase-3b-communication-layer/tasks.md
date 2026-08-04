## 1. Domain Entities & Enums

- [ ] 1.1 Create `AnswerThreadStatus` enum (`Open`, `Locked`)
- [ ] 1.2 Create `TeacherCheckQuestion` entity: `Id`, `TeacherId`, `QuestionKey`, `NoteText`, `CreatedAt`
- [ ] 1.3 Create `AnswerThread` entity: `Id`, `StudentAnswerId`, `Status`, `LockedAt`
- [ ] 1.4 Create `ThreadMessage` entity: `Id`, `AnswerThreadId`, `AuthorId`, `BodyText`, `SourceCheckQuestionId`, `CreatedAt`

## 2. Domain Tests

- [ ] 2.1 Test TeacherCheckQuestion factory
- [ ] 2.2 Test AnswerThread factory + Lock()
- [ ] 2.3 Test ThreadMessage factory

## 3. Infrastructure

- [ ] 3.1 Add DbSets to IAppDbContext and AppDbContext
- [ ] 3.2 Create EF configurations
- [ ] 3.3 Create migration

## 4. Application — Use Cases

- [ ] 4.1 Implement CheckQuestion CRUD (create, list, delete) with orphan detection
- [ ] 4.2 Implement thread + message use cases (create, get, post message, list messages)
- [ ] 4.3 Modify SubmitAnswerUseCase: auto-create AnswerThread on first answer
- [ ] 4.4 Modify MarkAnswerReviewedUseCase: lock thread when marking reviewed
- [ ] 4.5 Add notification creation on thread message post

## 5. API — Controllers

- [ ] 5.1 Create TeacherCheckQuestionsController
- [ ] 5.2 Create AnswerThreadsController

## 6. Application Tests

- [ ] 6.1 Test check-question CRUD
- [ ] 6.2 Test thread auto-creation on answer submit
- [ ] 6.3 Test thread lock on mark-reviewed

## 7. Polish

- [ ] 7.1 Build, test, validate
