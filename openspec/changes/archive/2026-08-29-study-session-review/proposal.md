## Why

The API already supports live study sessions (5 endpoints, SignalR hub, mark-reviewed with live-only gate and flag creation), but the frontend has zero session UI. The Teacher page only shows a student list — no way to start a session, review answers, or end a session. The SignalR hub exists but is never connected from the frontend. This blocks the core teacher workflow of reviewing student answers in a live setting.

## What Changes

- New page `/teacher/review/:sessionId` — dedicated review tool with question-by-question walkthrough, student answer display, mark-reviewed toggle, and session lifecycle controls
- New API endpoint `GET /api/study-sessions/{sessionId}/questions` — returns tree-traversal question list with per-question answer and flag status
- SignalR hub push events (`QuestionReviewed`, `SessionAdvanced`, `SessionEnded`) so the student sees live review progress
- Teacher page gets "Start Review" button on each student card
- Student Attempt page shows a review status bar when an active session exists
- `@microsoft/signalr` client dependency added to UI

## Capabilities

### New Capabilities
- `study-session-review`: Teacher live review of student answers — start/advance/end sessions, mark answers as reviewed, flag unanswered questions, real-time SignalR sync to student

### Modified Capabilities
- `ui`: Teacher page adds Start Review button per student; student Attempt page adds review status bar; new `/teacher/review/:sessionId` route; sidebar link to review page

## Impact

- **New page**: `src/pages/ReviewPage.tsx`
- **New components**: `ReviewHeader`, `QuestionMap`, `QuestionDisplay`, `AnswerDisplay`, `ReviewControls`
- **New hook**: `src/hooks/useSessionHub.ts` (SignalR connection)
- **New API service**: `src/services/sessionApi.ts` (getSessionQuestions)
- **New use case**: `GetSessionQuestionsUseCase` in Application layer
- **Modified API**: `StudySessionsController` (new GET endpoint + SignalR push calls), `StudySessionHub` (authorization)
- **Modified files**: `teacherApi.ts` (extend DTO), `apiSlice.ts` (add Session tagType), `router.tsx` (add route), `TeacherPage.tsx` (Start Review button), `AttemptPage.tsx` (ReviewBanner)
- **New dependency**: `@microsoft/signalr` in UI package.json
