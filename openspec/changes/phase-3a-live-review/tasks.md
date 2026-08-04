## 1. Domain Entities & Enums

- [ ] 1.1 Create `StudySessionStatus` enum (`NotStarted`, `InProgress`, `Completed`, `Abandoned`) in `Domain/Enums/`
- [ ] 1.2 Create `AnswerFlagType` enum (`UnansweredAtReview`) in `Domain/Enums/`
- [ ] 1.3 Create `StudySession` entity: `Id`, `LessonAttemptId`, `SequenceNumber`, `StartQuestionId`, `EndQuestionId`, `CurrentQuestionId`, `Status`, `StartedAt`, `EndedAt` + nav props
- [ ] 1.4 Create `AnswerFlag` entity: `Id`, `StudentId`, `QuestionId`, `QuestionKey`, `LessonAttemptId`, `FlagType`, `RaisedInSessionId`, `ResolvedAt` + nav props
- [ ] 1.5 Modify `StudentAnswer`: add `Reviewed`, `ReviewedBy`, `ReviewedAt`, `ReviewedInSessionId` fields

## 2. Domain Tests

- [ ] 2.1 Test `StudySession` factory creates valid entity
- [ ] 2.2 Test session lifecycle: Start → InProgress → Completed
- [ ] 2.3 Test `AnswerFlag` factory and Resolve()
- [ ] 2.4 Test `StudentAnswer.MarkReviewed()` sets all reviewed fields
- [ ] 2.5 Test `StudentAnswer` invariant: cannot mark reviewed without session ID

## 3. Infrastructure

- [ ] 3.1 Add `DbSet<StudySession>` and `DbSet<AnswerFlag>` to `IAppDbContext`
- [ ] 3.2 Create `StudySessionConfiguration` and `AnswerFlagConfiguration`
- [ ] 3.3 Update `StudentAnswerConfiguration` for new fields
- [ ] 3.4 Update `AppDbContext`
- [ ] 3.5 Create EF migration

## 4. Application — DTOs

- [ ] 4.1 Create `StudySessionDto`, `StartSessionRequest`, `AdvanceSessionRequest`, `MarkReviewedRequest`
- [ ] 4.2 Create `AnswerFlagDto`

## 5. Application — Use Cases

- [ ] 5.1 Implement `StartStudySessionUseCase` — creates session, computes tree sequence, sets start question
- [ ] 5.2 Implement `AdvanceStudySessionUseCase` — updates current_question_id
- [ ] 5.3 Implement `EndStudySessionUseCase` — completes session
- [ ] 5.4 Implement `MarkAnswerReviewedUseCase` — toggles reviewed; raises AnswerFlag if unanswered
- [ ] 5.5 Implement `GetStudySessionUseCase` — returns session with state
- [ ] 5.6 Modify `StartLessonAttemptUseCase` — check for unresolved AnswerFlags before creating

## 6. SignalR Hub

- [ ] 6.1 Install `Microsoft.AspNetCore.SignalR` NuGet package
- [ ] 6.2 Create `StudySessionHub` with groups, join/leave, authorization
- [ ] 6.3 Create hub methods for broadcasting session state
- [ ] 6.4 Wire up use cases to call hub from server-side on state changes
- [ ] 6.5 Add `app.MapHub<StudySessionHub>("/hubs/study-session")` in Program.cs

## 7. API — Controllers

- [ ] 7.1 Create `StudySessionsController` with endpoints for start/advance/end/mark-reviewed
- [ ] 7.2 Modify `StudentAttemptsController.Start` to handle flag-block error

## 8. Application Tests

- [ ] 8.1 Test `StartStudySessionUseCase` — creates session, default sequencing
- [ ] 8.2 Test `MarkAnswerReviewedUseCase` — sets reviewed fields
- [ ] 8.3 Test `MarkAnswerReviewedUseCase` — raises flag when unanswered
- [ ] 8.4 Test `StartLessonAttemptUseCase` — blocked by unresolved flag

## 9. API Tests

- [ ] 9.1 Test session start/advance/end endpoints
- [ ] 9.2 Test mark-reviewed endpoint

## 10. Integration Tests

- [ ] 10.1 End-to-end: Teacher starts session, advances, marks reviewed, ends session
- [ ] 10.2 End-to-end: flag raised on unanswered, student submits → flag resolved

## 11. Polish

- [ ] 11.1 Run `dotnet build` — zero errors
- [ ] 11.2 Run `dotnet test` — all tests pass
- [ ] 11.3 Validate: `openspec validate phase-3a-live-review --type change`
