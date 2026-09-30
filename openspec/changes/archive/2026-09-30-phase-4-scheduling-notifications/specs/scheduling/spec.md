## ADDED Requirements

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