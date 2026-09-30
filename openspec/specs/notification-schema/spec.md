# Notification Schema

## Purpose

Skeleton for tenant-scoped notifications: the persistence model and read/ack semantics. Present as schema groundwork; delivery channels are out of MVP scope for now.
## Requirements
### Requirement: Notifications are tenant-scoped persistent records

The system SHALL persist notifications as rows scoped to a Tenant and addressed to a user. A notification SHALL record its message and a read/acknowledged flag.

#### Scenario: Notification is created for a user in a tenant
- **WHEN** an event produces a notification for a user
- **THEN** a notification row is persisted scoped to the user's Tenant with its message and `read = false`

#### Scenario: Read state can be updated
- **WHEN** a user acknowledges a notification
- **THEN** the notification's read flag is set to true

### Requirement: Existing delivery channels remain out of scope

The system SHALL NOT implement email, push, or other outbound delivery channels in this spec. Only the persisted notification record and its read/ack state are in scope.

#### Scenario: No delivery channel required
- **WHEN** a notification is created
- **THEN** no outbound email, push, or SMS is sent as part of this capability

### Requirement: Notification entity and table exist

The system SHALL provide a `Notification` entity and corresponding database table with the following fields: `Id` (Guid, PK), `RecipientId` (Guid, FK to User), `Type` (enum: NewThreadMessage, AnswerFlagged, TeacherAssigned, AppointmentCreated, AppointmentReminder, SessionStarted), `ReferenceType` (string, polymorphic entity type), `ReferenceId` (Guid, polymorphic entity ID), `ReadAt` (DateTimeOffset?, nullable), `DeliveryChannel` (enum: InApp, Email), `CreatedAt` (DateTimeOffset).

#### Scenario: Notification table exists after migration
- **WHEN** the EF Core migration is applied
- **THEN** the `Notifications` table exists with all specified columns

#### Scenario: Notification entity can be created and saved
- **WHEN** a `Notification` entity is instantiated and saved via `AppDbContext`
- **THEN** the entity is persisted to the database with all fields intact

### Requirement: No notification endpoints exist

The system SHALL NOT expose any API endpoints for creating, reading, or managing notifications in Phase 1. Notifications SHALL exist only as a database table.

#### Scenario: No notification endpoints
- **WHEN** any HTTP request is made to a path matching `/api/notifications` or `/api/notifications/*`
- **THEN** the system returns 404 Not Found (no controller exists)


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
The system SHALL create an in-app Notification (DeliveryChannel.InApp) for: a student replying in a thread (? teacher), an answer being flagged (? student), an answer being reviewed (? student), an appointment being created/confirmed (? both), a student being assigned/reassigned (? relevant parties), a study session starting (? student), and a study session ending (? student).

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
