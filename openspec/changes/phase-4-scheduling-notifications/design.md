## Context

Appointment create/list work with the direct `StudentId`/`TeacherId` FK model (confirmed to keep). `StudySchedule.Confirm()`/`Cancel()` exist but are unwired. Notifications persist (table + entity) but have no controller, no mark-read, and only one of six triggers. Email is a logging stub and deferred to a later pass.

## Goals / Non-Goals

**Goals:**
- Teachers can confirm and cancel appointments; create validates the teacher-student assignment
- Students and teachers can list their appointments (existing) and see status
- In-app notification inbox: list, mark-read, mark-all-read
- Wire in-app notifications for the key triggers (thread reply, flag, appointment, assign/reassign, session start)
- UI: teacher appointments page, student view, notification bell/inbox

**Non-Goals:**
- Real email delivery (config-ready Mailgun deferred)
- Appointment reminder scheduling (manual/immediate for MVP, no cron)
- Google Calendar / iCal / recurring appointments / embedded video
- Notification SignalR push (inbox refetches; live push can reuse the hub pattern later)

## Decisions

### D1: Confirm/cancel as separate endpoints

**Choice**: `POST /api/appointments/{id}/confirm` and `POST /api/appointments/{id}/cancel`, teacher-authorized, calling the existing entity methods. Also `GET /api/appointments/{id}`.

**Rationale**: Matches the scheduling spec (teacher confirms/cancels). The entity methods already guard state transitions.

### D2: Assignment validation on create

**Choice**: `CreateAppointmentUseCase` verifies the target student has an active `TeacherAssignment` to the current teacher before creating; otherwise `ForbiddenException`.

**Rationale**: Closes the spec gap ("for a student assigned to them").

### D3: Notification controller + mark-read

**Choice**: `GET /api/notifications` (own, newest first), `POST /api/notifications/{id}/read`, `POST /api/notifications/read-all`. `Notification.MarkRead()` sets `ReadAt` idempotently.

**Rationale**: Simple read/ack surface consistent with the entity's `ReadAt` field. Amends the stale `notification-schema` "no endpoints" requirement.

### D4: In-app triggers via shared helper

**Choice**: A small static `NotificationFactory` (or inline `Notification.Create`) used by the wired use cases; all `DeliveryChannel.InApp`. No email channel this pass.

**Rationale**: Avoids triplicating the create call. Email channel added later when the real service lands.

### D5: Reusable notification bell in AppLayout

**Choice**: A `NotificationBell` in the sidebar header — unread badge, dropdown inbox, mark-read on click. `notificationsApi` refetches on open.

**Rationale**: Consistent global placement; no dedicated notifications page needed for MVP.

## Risks / Trade-offs

- **[Risk] Many triggers = broad surface** → Scope the wired triggers to the key ones in the §7 table; document the rest as deferred.
- **[Trade-off] No live push** → Inbox refetches when opened. Real-time can reuse the existing SignalR hub later.
- **[Trade-off] Email deferred** → Appointments/notifications work in-app; email is a follow-up once the provider key exists.