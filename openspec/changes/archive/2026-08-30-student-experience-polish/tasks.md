## 1. API — Completion + Answers Endpoints

- [x] 1.1 Create `CompleteAttemptUseCase` — verify ownership, call `attempt.Complete()`, idempotent on completed
- [x] 1.2 Create `ListAttemptAnswersUseCase` — return all `StudentAnswerDto` for the attempt, scoped to owner
- [x] 1.3 Add `POST /api/attempts/{attemptId}/complete` to `StudentAttemptsController`
- [x] 1.4 Add `GET /api/attempts/{attemptId}/answers` to `StudentAttemptsController`

## 2. UI — API Services

- [x] 2.1 Add `completeAttempt` mutation and `getAttemptAnswers` query to `studentApi.ts`
- [x] 2.2 Add `Answers` tagType to `apiSlice.ts` and wire invalidation so answers refresh after submit

## 3. UI — Inline Notes

- [x] 3.1 Create `QuestionNote` component — inline note editor (show/create/edit/delete)
- [x] 3.2 Fetch notes via `useGetNotesQuery` and integrate `QuestionNote` into each question card in `AttemptPage.tsx`

## 4. UI — Finish + Summary

- [x] 4.1 Add "Finish Lesson" button to the runner header (visible when attempt in progress)
- [x] 4.2 Wire Finish → confirm dialog → `completeAttempt` → render summary ("X of Y answered") with "Back to Lessons" link
- [x] 4.3 Make completed attempts read-only (hide answer/note editing controls)

## 5. UI — Show Existing Answers

- [x] 5.1 Fetch answers via `getAttemptAnswers` and pre-fill "Edit answer" inputs with the submitted value

## 6. Verification

- [x] 6.1 Run `dotnet build` and `dotnet test` in API project
- [x] 6.2 Run `pnpm type-check`, `pnpm lint`, `pnpm build` in UI project
- [x] 6.3 Manually verify: student leaves a note, finishes a lesson, sees summary, edits an answer with pre-filled text

## 7. Completion Status + Attempt History

- [x] 7.1 Create `ListMyAttemptsUseCase` and `GET /api/attempts` endpoint
- [x] 7.2 Add `listMyAttempts` query + `StudentAttemptSummaryDto` to UI services
- [x] 7.3 Show completion/in-progress indicator on the lessons list
- [x] 7.4 Show "Completed" badge + attempt history on the lesson detail page

## 8. Dialog Background Fix

- [x] 8.1 Fix transparent dialog/confirm background (oklch → design tokens)
