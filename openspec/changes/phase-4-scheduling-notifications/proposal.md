## Why

Teachers need to schedule live study sessions with students. Phase 4 adds the StudySchedule entity and wires email notifications for appointments and other Phase 1–3b events. Mailgun sends transactional emails — no embedded video (external meeting_link only).

## What Changes

- Add `StudySchedule` entity (Appointment) with status lifecycle (Proposed → Confirmed → Completed/Cancelled).
- Teacher creates/confirms/cancels appointments; student sees their own.
- Wire Mailgun email delivery for appointment created, appointment reminder, new thread message.
- Wire existing notification triggers end-to-end (in-app notifications already created; this adds email delivery path).

## Capabilities

### New Capabilities
- `scheduling`: StudySchedule CRUD, status lifecycle, student-teacher appointment visibility.
- `email-delivery`: Mailgun email delivery for appointment + thread notifications.

### Modified Capabilities
<!-- None -->

## Impact

- **Domain**: New `StudySchedule` entity + `AppointmentStatus` enum.
- **Application**: New use cases for schedule CRUD, email service.
- **Infrastructure**: Mailgun SDK integration, new DbSet, migration.
- **API**: New `AppointmentsController`.
