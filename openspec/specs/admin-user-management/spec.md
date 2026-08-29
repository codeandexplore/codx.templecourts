# Admin User Management

Admin capabilities for managing user accounts: listing users with their status and roles, resetting passwords, and suspending/reactivating users.

## Requirements

### Requirement: Admin can list users with status and roles
The system SHALL provide `GET /admin/users` returning all users with their email, display name, status (Active/Inactive), and assigned roles. The list SHALL be sorted by email.

#### Scenario: List all users
- **WHEN** an admin calls `GET /admin/users`
- **THEN** the response SHALL include every user with their status and roles

#### Scenario: Non-admin blocked
- **WHEN** a non-admin calls `GET /admin/users`
- **THEN** the system SHALL return 403 Forbidden

### Requirement: Admin can reset a user's password
The system SHALL provide `POST /admin/users/{userId}/reset-password` accepting a new password. On success, the user's password hash SHALL be updated and their refresh tokens SHALL be cleared, forcing re-login.

#### Scenario: Reset password succeeds
- **WHEN** an admin resets a user's password with a valid new password
- **THEN** the user SHALL be able to log in with the new password and existing refresh tokens SHALL be invalidated

#### Scenario: Reset password with weak password rejected
- **WHEN** an admin submits a new password under 8 characters
- **THEN** the system SHALL return a validation error and NOT change the password

#### Scenario: Reset password for nonexistent user
- **WHEN** an admin resets a password for a nonexistent user ID
- **THEN** the system SHALL return 404 Not Found

### Requirement: Admin can suspend and reactivate users
The system SHALL provide `PUT /admin/users/{userId}/status` accepting a target status of Active or Inactive. Suspended users SHALL be prevented from authenticating; reactivated users SHALL be able to log in again.

#### Scenario: Suspend a user
- **WHEN** an admin sets a user's status to Inactive
- **THEN** the user SHALL no longer be able to authenticate

#### Scenario: Reactivate a user
- **WHEN** an admin sets a suspended user's status to Active
- **THEN** the user SHALL be able to authenticate again

#### Scenario: Invalid status value rejected
- **WHEN** an admin submits a status other than Active or Inactive
- **THEN** the system SHALL return a validation error
