## Context

The API already has 5 session endpoints, a SignalR hub at `/hubs/study-session`, and use cases for start/advance/end/mark-reviewed with live-only gate and flag creation. The frontend has zero session-related UI — the Teacher page only lists students. The SignalR hub has no client connection and doesn't push business events.

The design system (`ui-design-system`) provides shadcn components and Clean Sacred tokens. The lesson authoring phase established patterns for dedicated tool pages (`/admin/editor`), slide-out panels, and recursive tree rendering.

## Goals / Non-Goals

**Goals:**
- Teacher can start a live review session from the student list
- Dedicated review page at `/teacher/review/:sessionId` with question-by-question walkthrough
- Teacher sees each question's prompt, student answer, and can mark as reviewed
- Teacher advances through questions in tree-traversal order and ends the session
- SignalR pushes review events to the student in real-time
- Student sees a review status bar on their Attempt page when an active session exists
- New API endpoint returning questions with answer/flag status per session

**Non-Goals:**
- Thread messages and communication (Phase 3b)
- Check-question bank UI (Phase 3b)
- Notifications (Phase 3b)
- Student-triggered session actions
- Session history or resumption views

## Decisions

### D1: Dedicated page route vs. tab vs. slide-out panel

**Choice**: New route `/teacher/review/:sessionId` with full page layout.

**Rationale**: The review experience needs full viewport width for question display and a question-map sidebar. A tab would be cramped; a slide-out panel constrains the review to 500px. Same pattern as the Lesson Editor (`/admin/editor`) — dedicated tool pages for focused work.

**Alternative**: Tab or panel — rejected for space constraints.

### D2: New API endpoint for questions with answers

**Choice**: `GET /api/study-sessions/{sessionId}/questions` returns tree-traversal ordered questions with per-question answer+flag status. Implemented as `GetSessionQuestionsUseCase`.

**Rationale**: The existing `GetStudySessionUseCase` only returns session metadata (status, timestamps). The teacher needs to see all questions with their answer state to navigate the review. A single endpoint avoids N+1 API calls.

**Alternative**: Expose separate answer and flag list endpoints → more network calls, more complex client state management.

### D3: SignalR event push from controllers, not hub

**Choice**: Controllers push events via `IHubContext<StudySessionHub>` after use case execution, rather than making the hub call use cases directly.

**Rationale**: Keeps use cases reusable (REST + SignalR), hub stays thin. Controllers compose: call use case → push event. This is the standard ASP.NET Core Signals pattern.

**Alternative**: Hub calls use cases directly → couples hub to business logic, harder to test.

### D4: Student review status via LessonAttemptDto extension

**Choice**: Add `activeSessionId?: Guid` to `LessonAttemptDto`. The Attempt page checks this field on load to show/hide the review banner.

**Rationale**: Avoids a separate API call per page load. The field is cheap to populate (single join on StudySession) and directly answers "is there an active session?"

**Alternative**: Separate endpoint `GET /api/study-sessions?attemptId=&status=IN_PROGRESS` → extra request, more client logic.

### D5: Teacher page "Start Review" via latest attempt

**Choice**: Extend `TeacherAssignmentDto` with `latestAttemptId?: Guid`. The Start Review button uses this ID directly.

**Rationale**: Simple, avoids a separate lookup. A student has one active attempt at a time (MVP constraint). If no attempt exists, the button is hidden.

**Alternative**: Dropdown to select attempt → unnecessary for MVP (one attempt per student).

### D6: Client-side current-question state

**Choice**: Use React `useState(currentQuestionIndex)` for tracking position in the question list. The API advance endpoint syncs the server state.

**Rationale**: Simple, reliable. The server's `CurrentQuestionId` tracks position; the client mirrors it but drives navigation. On page load, the client finds the index of `currentQuestionId` in the question list.

**Alternative**: Server-driven navigation (server tells client which question to show) → adds latency per navigation step.

### D7: Question map sidebar with status icons

**Choice**: Fixed 280px left sidebar showing all questions with status dots (unanswered ○, current ●, reviewed ✓, flagged ⚑). Clicking a question navigates to it.

**Rationale**: Spatial orientation for the teacher — see which questions are done, current, and remaining. Same pattern as many review/code-review tools.

## Risks / Trade-offs

- **[Risk] Tree-traversal ordering mismatch**: The new use case must use the same depth-first walk as `StartStudySessionUseCase`. Mitigation: Extract shared traversal logic into a static method used by both use cases.
- **[Risk] SignalR reconnection in React**: Hub connections drop and reconnect; stale event handlers can cause duplicates. Mitigation: Custom hook `useSessionHub` with proper cleanup on unmount, connection lifecycle management.
- **[Risk] Student answer concurrency during review**: Student can submit new answers while teacher reviews, causing the question map to be stale. Mitigation: The question map refetches on each mark-reviewed/advance action via RTK Query tag invalidation. Acceptable for MVP.
- **[Trade-off] No optimistic UI for advance/mark-reviewed**: Cache invalidation refetches the full question list on each action. With ~12 questions this is fast on localhost. Can add optimistic updates later if needed.
