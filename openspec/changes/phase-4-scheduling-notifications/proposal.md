## Why

Appointment creation/list works, but there is no way to confirm or cancel an appointment, and creation doesn't validate the teacher-student assignment. In-app notifications are schema-only: no controller exists to list/read them, and only one of the six triggers is wired (thread message teacher→student, with the student→teacher direction left as a TODO). There is no UI for either feature.

## What Changes

- Appointments: confirm and cancel endpoints (entity methods exist but are unwired), single lookup, and assignment validation on create
- Notifications: `MarkRead` domain method, `NotificationsController` (list, mark-read, mark-all-read), and wiring of in-app triggers (thread reply→teacher, flag→student, appointment created/confirmed→both, assign/reassign, session started)
- UI: teacher appointments page (list/create/confirm/cancel), student appointment view, and a notification bell + inbox in the layout

## Capabilities

### Modified Capabilities
- `scheduling`: Adds confirm/cancel/single-get for appointments and assignment validation on create.
- `notification-schema`: Adds list/mark-read endpoints and in-app trigger wiring (this spec currently says no endpoints exist — now amended).
- `ui`: Adds teacher appointments page, student appointment view, and notification inbox.

## Impact

- **New use cases**: `ConfirmAppointmentUseCase`, `CancelAppointmentUseCase`, `GetAppointmentUseCase`, `ListNotificationsUseCase`, `MarkNotificationReadUseCase`, `MarkAllNotificationsReadUseCase`
- **Modified use cases**: `CreateAppointmentUseCase` (assignment validation), `PostThreadMessageUseCase` (student→teacher trigger), `MarkAnswerReviewedUseCase` (flag trigger), `ClaimStudentUseCase`/`ReassignStudentUseCase` (assignment triggers), `StartStudySessionUseCase` (session trigger)
- **New domain method**: `Notification.MarkRead()`
- **New controller**: `NotificationsController`
- **Modified controller**: `AppointmentsController` (confirm/cancel/get)
- **UI**: `appointmentsApi`, `notificationsApi`, `AppointmentsPage`, `NotificationBell`, `AppointmentCard` components, routes + sidebar links
- **No new entities or migrations**