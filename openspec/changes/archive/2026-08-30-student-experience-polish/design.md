## Context

The backend for notes (`StudentQuestionNote` CRUD, 5 use cases) and attempts (`StartLessonAttempt`, `GetAttempt`, `SubmitAnswer`) is complete and spec-conformant. The frontend has the RTK Query hooks for notes (`useGetNotesQuery`, `useCreateNoteMutation`, etc.) defined but unused. The `LessonAttempt.Complete()` domain method exists but no endpoint exposes it. The attempt runner (`AttemptPage.tsx`) shows all questions as stacked cards with no note affordance, no completion path, and no way to see a previously submitted answer.

## Goals / Non-Goals

**Goals:**
- Student can complete a lesson attempt (Finish button)
- Student sees a completion summary (X of Y answered)
- Student can leave/edit per-question notes inline on the lesson runner
- Student sees their previously submitted answer when editing

**Non-Goals:**
- Attempt history page (out of scope for this pass)
- Progress indicators on the lessons list / home dashboard
- Note list page
- Auto-save (explicit submit only)
- Multiple attempts per lesson (already out of scope per architecture)

## Decisions

### D1: Dedicated answers endpoint over folding into GetAttempt

**Choice**: New `GET /api/attempts/{attemptId}/answers` returns the student's answers; `GET /api/attempts/{id}` stays focused on attempt metadata + answered keys.

**Rationale**: Keeps the attempt DTO lightweight and gives a clean, reusable list of answers. The answers endpoint feeds both "Edit answer" pre-fill and the completion summary without coupling.

**Alternative**: Include answers in `GetAttempt` — rejected because it bloats the attempt DTO and mixes metadata with payload.

### D2: Explicit Finish button + summary over auto-complete

**Choice**: A "Finish Lesson" button in the runner header. Clicking it confirms, calls `POST /api/attempts/{id}/complete`, then shows a summary ("X of Y answered") with a "Back to Lessons" link.

**Rationale**: Gives the student control over when a lesson is done and a moment of closure. Auto-complete would surprise students who want to leave and return.

**Alternative**: Auto-complete on last answer — rejected; a student may answer everything but still want to revisit before finishing.

### D3: Inline note editor on the runner, not a separate page

**Choice**: Each question card gets a "Note" toggle that expands an inline editor (show existing note, save via create/update, delete if present).

**Rationale**: Notes are "personal margin notes" — taken in context while working through the lesson. A separate notes page loses that spatial context.

**Alternative**: Notes on the lesson detail page or a dedicated page — rejected for context loss.

### D4: Complete idempotently (no-op on already-completed)

**Choice**: `CompleteAttemptUseCase` returns the attempt as-is if already completed (idempotent), rather than throwing.

**Rationale**: A double-click or retry shouldn't error. Matches the "upsert" philosophy elsewhere.

**Alternative**: 409 on second complete — rejected for retry-friendliness.

## Risks / Trade-offs

- **[Risk] Completed attempt is read-only** → The runner must disable editing after completion. Mitigation: render answers read-only and hide the note/answer controls once `status === Completed`.
- **[Trade-off] Notes use `question_key` (stable) not `question_id`** → Notes survive version bumps (per architecture). Mitigation: fetch notes by `questionKey`; existing API already keys on it.
- **[Trade-off] No auto-save** → Notes/answers save on explicit click only. Acceptable for MVP; auto-save can come later.
