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

