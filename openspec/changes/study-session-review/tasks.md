## 1. API — New Endpoint

- [x] 1.1 Create `GetSessionQuestionsUseCase` in Application layer — queries `LessonAttempt` → published version tree → `StudentAnswer` + `AnswerFlag` per question in tree-traversal order
- [x] 1.2 Create `SessionQuestionsDto` and `QuestionWithAnswerDto` DTOs
- [x] 1.3 Add `GET /api/study-sessions/{sessionId}/questions` to `StudySessionsController`
- [x] 1.4 Extract shared tree-traversal logic from `StartStudySessionUseCase` into static helper used by both use cases
- [x] 1.5 Add `activeSessionId` to `LessonAttemptDto` — query active IN_PROGRESS session on load
- [x] 1.6 Add `latestAttemptId` to `TeacherAssignmentDto` — query most recent in-progress attempt per assigned student

## 2. API — SignalR Hub Auth + Event Push

- [x] 2.1 Add authorization to `StudySessionHub` — verify caller is assigned teacher or student for the session
- [x] 2.2 Push `QuestionReviewed` event from controller after `MarkAnswerReviewedUseCase` succeeds via `IHubContext<StudySessionHub>`
- [x] 2.3 Push `SessionAdvanced` event from controller after `AdvanceStudySessionUseCase` succeeds
- [x] 2.4 Push `SessionEnded` event from controller after `EndStudySessionUseCase` succeeds

## 3. UI — Dependencies + Services

- [x] 3.1 Install `@microsoft/signalr` in UI project (`pnpm add @microsoft/signalr`)
- [x] 3.2 Create `src/services/sessionApi.ts` — `useGetSessionQuestionsQuery` (provides `["Session"]` tag), `useMarkReviewedMutation`, `useAdvanceSessionMutation`, `useEndSessionMutation`
- [x] 3.3 Add `"Session"` to `tagTypes` in `src/store/apiSlice.ts`
- [x] 3.4 Create `src/hooks/useSessionHub.ts` — SignalR connection hook: auto-join group on mount, auto-leave on unmount, expose `onQuestionReviewed`, `onSessionAdvanced`, `onSessionEnded` callbacks
- [x] 3.5 Extend teacher services: update `TeacherAssignmentDto` with `latestAttemptId`, update `LessonAttemptDto` with `activeSessionId`

## 4. UI — Review Page

- [x] 4.1 Add `/teacher/review/:sessionId` route to `src/router.tsx` under `RequireRole("Teacher")`
- [x] 4.2 Create `src/pages/ReviewPage.tsx` — page shell fetching session questions, managing current index state
- [x] 4.3 Implement `ReviewHeader` — breadcrumb (Back to Students), lesson title, student name, progress counter (N/M), End Session button with ConfirmDialog
- [x] 4.4 Implement `QuestionMap` — 280px sidebar with scrollable question list, status icons (unanswered ○, current ●, reviewed ✓, flagged ⚑), click to navigate
- [x] 4.5 Implement `QuestionDisplay` — parent node breadcrumb, question type Badge, prompt text
- [x] 4.6 Implement `AnswerDisplay` — show student answer text or "Not yet answered" empty state
- [x] 4.7 Implement `ReviewControls` — Mark Reviewed toggle, Advance button
- [x] 4.8 Wire all mutations: mark-reviewed invalidates Session tag, advance calls API + updates local index, end session with confirm → navigate to /teacher
- [x] 4.9 Handle edge cases: no answers yet (empty state), refresh restores state, 403 on non-teacher access

## 5. UI — Teacher Page Changes

- [x] 5.1 Add "Start Review" button to each student card in `TeacherPage.tsx` — visible only when `latestAttemptId` exists
- [x] 5.2 Wire Start Review button: call `useStartSessionMutation`, navigate to `/teacher/review/:sessionId`

## 6. UI — Student Page Changes

- [x] 6.1 Add `ReviewBanner` component — shows "Teacher is reviewing your answers — Question N of M"
- [x] 6.2 Integrate `ReviewBanner` into `AttemptPage.tsx` — connected to SignalR via `useSessionHub`, shown when `activeSessionId` exists on the attempt
- [x] 6.3 Handle SignalR events: update question number on `SessionAdvanced`, hide banner on `SessionEnded`

## 7. Verification

- [x] 7.1 Run `dotnet build` and `dotnet test` in API project — verify no regressions
- [x] 7.2 Run `pnpm type-check`, `pnpm lint`, `pnpm build` in UI project
- [ ] 7.3 Manually verify: teacher starts session → walks through questions → marks reviewed → ends session
- [ ] 7.4 Manually verify: student page shows review banner during active session, updates on teacher advance
