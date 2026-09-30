## 1. API — Appointments

- [x] 1.1 Create `ConfirmAppointmentUseCase` and `CancelAppointmentUseCase` (teacher/participant auth, call entity methods)
- [x] 1.2 Create `GetAppointmentUseCase` (single lookup, participant auth)
- [x] 1.3 Add confirm/cancel/get endpoints to `AppointmentsController`
- [x] 1.4 Add assignment validation to `CreateAppointmentUseCase` (active TeacherAssignment to current teacher)

## 2. API — Notifications

- [x] 2.1 Add `Notification.MarkRead()` domain method
- [x] 2.2 Create `ListNotificationsUseCase`, `MarkNotificationReadUseCase`, `MarkAllNotificationsReadUseCase`
- [x] 2.3 Add `NotificationsController` (list, mark-read, read-all)
- [x] 2.4 Add `NotificationDto` (with unread state + reference info)

## 3. API — Wire In-App Triggers

- [x] 3.1 Fix `PostThreadMessageUseCase` — notify the thread's teacher on student reply
- [x] 3.2 `MarkAnswerReviewedUseCase` — notify student when a flag is raised
- [x] 3.3 `CreateAppointmentUseCase` / confirm — notify both parties
- [x] 3.4 `ClaimStudentUseCase` / `ReassignStudentUseCase` — notify student/teacher
- [x] 3.5 `StartStudySessionUseCase` — notify student on session start

## 4. UI — Appointments

- [x] 4.1 Create `appointmentsApi` service
- [x] 4.2 Teacher `/teacher/appointments` page — list, create (student picker), confirm/cancel
- [x] 4.3 Student appointments view (route or section)

## 5. UI — Notifications

- [x] 5.1 Create `notificationsApi` service + `NotificationDto` type
- [x] 5.2 Add `NotificationBell` component + unread badge to `AppLayout`
- [x] 5.3 Implement inbox dropdown with mark-read

## 6. Verification

- [x] 6.1 Run `dotnet build` and `dotnet test` in API project
- [x] 6.2 Run `pnpm type-check`, `pnpm lint`, `pnpm build` in UI project
- [ ] 6.3 Manually verify: teacher schedules + confirms, student sees it; thread reply/flag produce notifications; bell shows unread
