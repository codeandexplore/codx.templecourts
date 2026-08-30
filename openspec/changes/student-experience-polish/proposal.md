## Why

The student experience is incomplete: the notes API is fully built but has no UI surface, lessons cannot be completed (the "Finish" action doesn't exist), and the "Edit answer" flow opens a blank input with no way to see a previously submitted answer. The Phase 2 exit criteria — "student completes a lesson end-to-end … and can leave notes" — remain unmet in the UI.

## What Changes

- New endpoint `POST /api/attempts/{attemptId}/complete` — marks a lesson attempt complete via `LessonAttempt.Complete()`
- New endpoint `GET /api/attempts/{attemptId}/answers` — returns the student's submitted answers for an attempt
- Inline note-taking UI on the lesson runner (per-question margin note), backed by the existing notes API
- "Finish Lesson" button with a completion summary (X of Y answered)
- "Edit answer" pre-fills the input with the previously submitted answer

## Capabilities

### Modified Capabilities
- `student-attempts`: Adds attempt completion (`POST /api/attempts/{id}/complete`) and a list-my-answers endpoint (`GET /api/attempts/{id}/answers`).
- `ui`: Adds inline notes, the Finish button + completion summary, and pre-filled answer editing to the lesson runner.

## Impact

- **New use cases**: `CompleteAttemptUseCase`, `ListAttemptAnswersUseCase`
- **New endpoints**: `POST /api/attempts/{attemptId}/complete`, `GET /api/attempts/{attemptId}/answers`
- **New UI component**: `QuestionNote` (inline note editor)
- **Modified files**: `StudentAttemptsController.cs`, `AttemptPage.tsx`, `studentApi.ts`, `apiSlice.ts`
- **No new entities or migrations** — reuses `LessonAttempt.Complete()` and existing `StudentAnswer`/`StudentQuestionNote`
