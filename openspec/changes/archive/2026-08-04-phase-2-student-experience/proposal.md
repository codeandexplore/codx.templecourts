## Why

The API currently has no student-facing surface: lessons can only be authored by Admins, with no way for a Student to start, answer, or note-take against a lesson. Phase 2 closes that gap with the core student experience — lesson runner, answer recording, sibling-gated sequencing — aligned with the Guided Discovery Principle (no auto-grading, no `reference_context` leak).

## What Changes

- Add `LessonAttempt` and `StudentAnswer` domain entities with a one-attempt-per-student-per-lesson constraint.
- Add `StudentQuestionNote` entity keyed on stable `question_key` (survives version bumps).
- Add student-facing API endpoints: list published lessons, read lesson tree, start attempt, submit/update answers with sibling-gating enforcement, CRUD notes.
- Fix `reference_context` leak: strip it from responses when the caller is a Student.
- Enforce sibling gating (`requires_prior_sibling_answered`) at answer-submit time — the immediately preceding sibling's entire question subtree must be answered before the gated node's questions can be answered.

## Capabilities

### New Capabilities
- `student-attempts`: LessonAttempt creation (pinned to lesson version), answer submission with sibling-gating enforcement, answer updating.
- `student-notes`: StudentQuestionNote CRUD — create, read, update, delete per-question notes scoped to the student.

### Modified Capabilities
<!-- None. The existing lesson-reading spec already covers student lesson listing, attempt creation, answer recording, and reference_context stripping — Phase 2 implements it. -->


## Impact

- **Domain**: New entities `LessonAttempt`, `StudentAnswer`, `StudentQuestionNote` + `LessonAttemptStatus` enum.
- **Application**: New use cases `StartLessonAttempt`, `SubmitAnswer`, `UpdateAnswer`, `CreateStudentQuestionNote`, `UpdateStudentQuestionNote`, `GetStudentQuestionNote`, `DeleteStudentQuestionNote`, `ListStudentLessons`. Existing `GetLessonTreeUseCase` modified to accept an `includeReferenceContext` flag.
- **Infrastructure**: `IAppDbContext` extended with `DbSet<LessonAttempt>`, `DbSet<StudentAnswer>`, `DbSet<StudentQuestionNote>`. `AppDbContext` updated. New EF migration.
- **API**: New `StudentLessonsController` and `StudentAnswersController`. Modified `LessonsController` to strip `reference_context` by role.
- **No breaking changes** to Phase 0/1 endpoints or contract.
