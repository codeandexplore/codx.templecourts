## 1. Domain Entities & Enums

- [x] 1.1 Create `LessonAttemptStatus` enum (`InProgress`, `Completed`) in `Domain/Enums/`
- [x] 1.2 Create `LessonAttempt` entity with fields: `Id`, `StudentId`, `LessonKey`, `LessonVersionId`, `Status`, `StartedAt`, `CompletedAt` + nav properties to `User`, `LessonVersion`, `Answers`
- [x] 1.3 Create `StudentAnswer` entity with fields: `Id`, `StudentId`, `LessonAttemptId`, `QuestionKey`, `AnswerValue` (JsonDocument), `PromptSnapshot`, `QuestionTypeSnapshot`, `SubmittedAt` + nav properties
- [x] 1.4 Create `StudentQuestionNote` entity with fields: `Id`, `StudentId`, `QuestionKey`, `NoteText`, `CreatedAt` + nav to `User`

## 2. Domain Tests

- [x] 2.1 Test `LessonAttempt` factory method creates valid entity with all required fields
- [x] 2.2 Test `StudentAnswer` factory method creates valid entity and sets `SubmittedAt`
- [x] 2.3 Test `StudentQuestionNote` factory method creates valid entity with `CreatedAt`
- [x] 2.4 Test `LessonAttempt` invariant: cannot complete an already-completed attempt
- [x] 2.5 Test `StudentAnswer` invariant: `AnswerValue` cannot be null

## 3. Application Abstractions

- [x] 3.1 Add `DbSet<LessonAttempt> LessonAttempts` to `IAppDbContext`
- [x] 3.2 Add `DbSet<StudentAnswer> StudentAnswers` to `IAppDbContext`
- [x] 3.3 Add `DbSet<StudentQuestionNote> StudentQuestionNotes` to `IAppDbContext`

## 4. Infrastructure — EF Core Configuration

- [x] 4.1 Create `LessonAttemptConfiguration` : `IEntityTypeConfiguration<LessonAttempt>` with snake_case columns, FK to `User` and `LessonVersion`, index on `(StudentId, LessonKey)` for uniqueness lookup
- [x] 4.2 Create `StudentAnswerConfiguration` : `IEntityTypeConfiguration<StudentAnswer>` with snake_case columns, FK to `User` and `LessonAttempt`, unique index on `(LessonAttemptId, QuestionKey)` for upsert
- [x] 4.3 Create `StudentQuestionNoteConfiguration` : `IEntityTypeConfiguration<StudentQuestionNote>` with snake_case columns, FK to `User`, unique index on `(StudentId, QuestionKey)`
- [x] 4.4 Add new `DbSet<T>` properties to `AppDbContext` and apply configurations in `OnModelCreating`
- [x] 4.5 Create EF migration for the three new tables

## 5. Application — DTOs

- [x] 5.1 Create `LessonAttemptDto` (id, lessonKey, lessonVersionId, status, startedAt, completedAt, answeredQuestionKeys)
- [x] 5.2 Create `StudentAnswerDto` (id, lessonAttemptId, questionKey, answerValue, promptSnapshot, questionTypeSnapshot, submittedAt)
- [x] 5.3 Create `SubmitAnswerRequest` (attemptId, questionKey, answerValue as JSON object)
- [x] 5.4 Create `StudentQuestionNoteDto` (id, questionKey, noteText, createdAt)
- [x] 5.5 Create `CreateNoteRequest` / `UpdateNoteRequest` (noteText)

## 6. Application — Use Cases

- [x] 6.1 Implement `ListStudentLessonsUseCase` — returns `List<LessonDto>` for published lessons only; any authenticated user
- [x] 6.2 Modify `GetLessonTreeUseCase` to accept `bool includeReferenceContext` parameter; conditionally exclude `ReferenceContext` from `QuestionDto` when false
- [x] 6.3 Implement `StartLessonAttemptUseCase` — finds published version by lesson key, creates `LessonAttempt` pinned to version, or returns existing in-progress attempt
- [x] 6.4 Implement `GetAttemptUseCase` — returns `LessonAttemptDto` with list of answered question keys for the given attempt
- [x] 6.5 Implement `SubmitAnswerUseCase` — validates attempt is in-progress and belongs to caller, validates question exists in pinned version, enforces sibling gating, upserts answer with snapshot capture
- [x] 6.6 Implement `CreateStudentQuestionNoteUseCase` — creates note keyed on `question_key`, scoped to student
- [x] 6.7 Implement `GetStudentQuestionNoteUseCase` — returns single note by question_key or 404
- [x] 6.8 Implement `ListStudentQuestionNotesUseCase` — returns all notes for the current student
- [x] 6.9 Implement `UpdateStudentQuestionNoteUseCase` — updates existing note's text
- [x] 6.10 Implement `DeleteStudentQuestionNoteUseCase` — deletes note or returns 204 if none exists

## 7. Application — Validators & Sibling Gating

- [x] 7.1 Create `SubmitAnswerRequestValidator` — validates `answerValue` is not null, `questionKey` is provided
- [x] 7.2 Implement `SiblingGatingService` — loads node tree for version, checks prior sibling's question subtree is fully answered; returns list of unsatisfied question keys on failure
- [x] 7.3 Create `CreateNoteRequestValidator` — validates `noteText` is not empty
- [x] 7.4 Create `UpdateNoteRequestValidator` — validates `noteText` is not empty

## 8. API — Reference Context Strip

- [x] 8.1 In `LessonsController.Get("{key}")`, read caller's `application_role` from JWT claims, pass `includeReferenceContext: false` when role is `Student`
- [x] 8.2 Verify existing Admin/Teacher flows still include `ReferenceContext` in `QuestionDto`

## 9. API — Controllers

- [x] 9.1 Add `GET /api/lessons` student-facing list to `LessonsController` (authenticated, returns published only)
- [x] 9.2 Create `StudentAttemptsController` with endpoints: `POST /api/lessons/{key}/attempts`, `GET /api/attempts/{id}`
- [x] 9.3 Create `StudentAnswersController` with endpoint: `POST /api/attempts/{attemptId}/answers`
- [x] 9.4 Create `StudentNotesController` with endpoints: `POST /api/notes/{questionKey}`, `GET /api/notes`, `GET /api/notes/{questionKey}`, `PUT /api/notes/{questionKey}`, `DELETE /api/notes/{questionKey}`

## 10. Seed Data

- [x] 10.1 Add a Student user to `DataSeeder` with email from `Student:Email` configuration
- [x] 10.2 Add sibling gating seed data to Lesson 1: set `RequiresPriorSiblingAnswered = true` on the second top-level node ("Manifested in Christ") so its questions are gated behind "Before the Foundation of the World" being fully answered
- [x] 10.3 Add optional `Student:Email` / `Student:Password` configuration keys to `appsettings.json` and `appsettings.Development.json` with placeholder values

## 11. Application Tests

- [x] 11.1 Test `StartLessonAttemptUseCase` — creates attempt, pins version, returns existing attempt on duplicate
- [x] 11.2 Test `StartLessonAttemptUseCase` — returns 404 when lesson has no published version
- [x] 11.3 Test `SubmitAnswerUseCase` — creates answer, captures snapshots
- [x] 11.4 Test `SubmitAnswerUseCase` — updates existing answer (upsert)
- [x] 11.5 Test `SubmitAnswerUseCase` — rejects answer for completed attempt
- [x] 11.6 Test `SubmitAnswerUseCase` — rejects answer for question not in pinned version
- [x] 11.7 Test sibling gating — answer accepted when prior sibling's subtree is fully answered
- [x] 11.8 Test sibling gating — answer rejected when prior sibling has unanswered questions
- [x] 11.9 Test sibling gating — no check when `requires_prior_sibling_answered = false`
- [x] 11.10 Test `ListStudentLessonsUseCase` — returns only published lessons
- [x] 11.11 Test `GetLessonTreeUseCase` with `includeReferenceContext = false` — `ReferenceContext` is null in output
- [x] 11.12 Test `GetLessonTreeUseCase` with `includeReferenceContext = true` — `ReferenceContext` is present
- [x] 11.13 Test CRUD use cases for StudentQuestionNote (create, read, list, update, delete)

## 12. API Tests

- [x] 12.1 Test `GET /api/lessons` returns 200 with published lessons only as authenticated Student
- [x] 12.2 Test `GET /api/lessons` returns 401 unauthenticated
- [x] 12.3 Test `GET /api/lessons/{key}` strips `reference_context` for Student role
- [x] 12.4 Test `GET /api/lessons/{key}` includes `reference_context` for Admin/Teacher role
- [x] 12.5 Test `POST /api/lessons/{key}/attempts` creates attempt as Student
- [x] 12.6 Test `POST /api/lessons/{key}/attempts` returns existing attempt on duplicate
- [x] 12.7 Test `GET /api/attempts/{id}` returns attempt with answered question keys
- [x] 12.8 Test `POST /api/attempts/{attemptId}/answers` submits and returns answer
- [x] 12.9 Test `POST /api/attempts/{attemptId}/answers` enforces sibling gating
- [x] 12.10 Test Student notes CRUD: create, read, list, update, delete

## 13. Integration Tests

- [x] 13.1 End-to-end: Student lists lessons, starts attempt, submits answers, reads progress
- [x] 13.2 End-to-end: Student submits answers to gated node before prior sibling → 422, then answers prior sibling → success
- [x] 13.3 End-to-end: Student creates note, reads it, updates it, deletes it
- [x] 13.4 Verify `reference_context` is not present in any Student-role response during the integration flow

## 14. Polish

- [x] 14.1 Run `dotnet build` — zero errors
- [x] 14.2 Run `dotnet test` — all tests pass
- [x] 14.3 Run OpenSpec validation: `openspec validate --change phase-2-student-experience`
