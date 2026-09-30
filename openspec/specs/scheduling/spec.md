# scheduling Specification

## Purpose
TBD - created by archiving change phase-4-scheduling-notifications. Update Purpose after archive.
## Requirements
### Requirement: Teacher can create an appointment

The system SHALL allow a Teacher to create an appointment for a student assigned to them. The appointment SHALL include scheduled_at, duration, and optional meeting_link.

#### Scenario: Create appointment
- **WHEN** a Teacher sends `POST /api/appointments` with student ID, scheduled time, and duration
- **THEN** a StudySchedule is created with status Proposed

### Requirement: Teacher can confirm or cancel an appointment

The system SHALL allow the Teacher to transition a Proposed appointment to Confirmed or Cancelled.

#### Scenario: Confirm appointment
- **WHEN** a Teacher confirms a Proposed appointment
- **THEN** the status changes to Confirmed

### Requirement: Student can view their appointments

The system SHALL allow a Student to list their own appointments.

#### Scenario: List appointments
- **WHEN** a Student sends `GET /api/appointments`
- **THEN** their StudySchedules are returned


### Requirement: Teacher can confirm and cancel appointments
The system SHALL provide `POST /api/appointments/{appointmentId}/confirm` and `POST /api/appointments/{appointmentId}/cancel`, both teacher-authorized, transitioning a Proposed appointment to Confirmed or Cancelled. `GET /api/appointments/{appointmentId}` SHALL return a single appointment to an authorized participant.

#### Scenario: Teacher confirms an appointment
- **WHEN** a Teacher sends `POST /api/appointments/{id}/confirm` for a Proposed appointment they created
- **THEN** the appointment status SHALL change to Confirmed

#### Scenario: Teacher cancels an appointment
- **WHEN** a Teacher sends `POST /api/appointments/{id}/cancel`
- **THEN** the appointment status SHALL change to Cancelled

#### Scenario: Invalid transition rejected
- **WHEN** a Teacher confirms a Cancelled or Completed appointment
- **THEN** the system SHALL return 400 Bad Request

### Requirement: Appointment creation validates the teacher-student assignment
`POST /api/appointments` SHALL only create an appointment when the student has an active TeacherAssignment to the current teacher.

#### Scenario: Assigned student
- **WHEN** a Teacher creates an appointment for a student assigned to them
- **THEN** the appointment SHALL be created with status Proposed

#### Scenario: Unassigned student rejected
- **WHEN** a Teacher creates an appointment for a student not assigned to them
- **THEN** the system SHALL return 403 Forbidden
