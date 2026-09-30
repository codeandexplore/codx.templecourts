## ADDED Requirements

### Requirement: Teacher appointments page
The Teacher SHALL have an appointments page listing their appointments (as teacher or creator), with actions to create, confirm, and cancel. Creating SHALL use a student picker (assigned students) plus scheduled time, duration, and an external meeting link.

#### Scenario: Teacher lists and manages appointments
- **WHEN** a teacher opens the appointments page
- **THEN** their appointments SHALL list with status, and Proposed ones SHALL offer Confirm/Cancel actions

#### Scenario: Teacher creates an appointment
- **WHEN** a teacher creates an appointment for an assigned student with a time and meeting link
- **THEN** the appointment SHALL appear in the list as Proposed

### Requirement: Student can view their appointments
The Student SHALL be able to view their appointments with status and meeting link. No create/confirm/cancel actions (teacher-driven).

#### Scenario: Student sees appointments
- **WHEN** a student opens their appointments view
- **THEN** their appointments SHALL list with status, time, and meeting link when confirmed

### Requirement: Notification bell and inbox
The app layout SHALL show a notification bell with an unread-count badge. Opening it SHALL display the user's notifications; clicking a notification SHALL mark it read.

#### Scenario: Unread badge and inbox
- **WHEN** a user has unread notifications
- **THEN** the bell SHALL show an unread count and the inbox SHALL list notifications, marking them read on click