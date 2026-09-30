## ADDED Requirements

### Requirement: User can list and read their notifications
The system SHALL provide `GET /api/notifications` returning the current user's notifications newest-first, `POST /api/notifications/{id}/read` to mark one read, and `POST /api/notifications/read-all` to mark all read. Only the recipient SHALL access a notification.

#### Scenario: List my notifications
- **WHEN** a user sends `GET /api/notifications`
- **THEN** the response SHALL include their notifications ordered newest-first with unread state

#### Scenario: Mark a notification read
- **WHEN** a user sends `POST /api/notifications/{id}/read`
- **THEN** the notification SHALL be marked read (idempotent)

#### Scenario: Mark all read
- **WHEN** a user sends `POST /api/notifications/read-all`
- **THEN** all their notifications SHALL be marked read

#### Scenario: Access another user's notification blocked
- **WHEN** a user sends `POST /api/notifications/{id}/read` for another user's notification
- **THEN** the system SHALL return 403 Forbidden

### Requirement: In-app notifications created for key triggers
The system SHALL create an in-app Notification (DeliveryChannel.InApp) for: a student replying in a thread (→ teacher), an answer being flagged (→ student), an answer being reviewed (→ student), an appointment being created/confirmed (→ both), a student being assigned/reassigned (→ relevant parties), a study session starting (→ student), and a study session ending (→ student).

#### Scenario: Student reply notifies teacher
- **WHEN** a Student posts a message in their answer thread
- **THEN** a Notification SHALL be created for the thread's teacher

#### Scenario: Flag raised notifies student
- **WHEN** an unanswered answer is flagged during review
- **THEN** a Notification SHALL be created for the student

#### Scenario: Answer reviewed notifies student
- **WHEN** a Teacher marks an answer as reviewed
- **THEN** a Notification SHALL be created for the student

#### Scenario: Session started notifies student
- **WHEN** a Teacher starts a study session for a student
- **THEN** a Notification SHALL be created for the student

#### Scenario: Session ended notifies student
- **WHEN** a Teacher ends a study session for a student
- **THEN** a Notification SHALL be created for the student