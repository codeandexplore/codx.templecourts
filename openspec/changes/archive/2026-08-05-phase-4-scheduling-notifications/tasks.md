## 1. Domain

- [ ] 1.1 Create `AppointmentStatus` enum (Proposed, Confirmed, Completed, Cancelled)
- [ ] 1.2 Create `StudySchedule` entity: Id, TeacherAssignmentId, ScheduledAt, Duration, MeetingLink, Status, CreatedBy

## 2. Infrastructure

- [ ] 2.1 Add DbSet to IAppDbContext + AppDbContext
- [ ] 2.2 Create EF config + migration
- [ ] 2.3 Create Mailgun email service interface + implementation

## 3. Application

- [ ] 3.1 Implement CreateAppointmentUserCase
- [ ] 3.2 Implement Confirm/Cancel use cases
- [ ] 3.3 Implement ListAppointmentsUserCase
- [ ] 3.4 Wire email to PostThreadMessageUseCase and CreateAppointment

## 4. API

- [ ] 4.1 Create AppointmentsController

## 5. Polish

- [ ] 5.1 Build, test, validate
