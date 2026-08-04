## ADDED Requirements

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
