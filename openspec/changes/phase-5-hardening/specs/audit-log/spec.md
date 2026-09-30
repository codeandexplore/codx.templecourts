## ADDED Requirements

### Requirement: Admin can list audit log entries
The system SHALL provide `GET /admin/audit-logs` (admin-only) returning audit entries newest-first, each with the action, performer, target user, metadata, and timestamp.

#### Scenario: Admin lists audit entries
- **WHEN** an Admin sends `GET /admin/audit-logs`
- **THEN** the response SHALL include audit entries ordered newest-first

#### Scenario: Non-admin blocked
- **WHEN** a non-admin sends `GET /admin/audit-logs`
- **THEN** the system SHALL return 403 Forbidden

### Requirement: Role revocation is audited
When a role assignment is revoked, the system SHALL write a `RoleRevoked` audit entry referencing the target user.

#### Scenario: Revoke role writes audit entry
- **WHEN** an Admin revokes a role assignment
- **THEN** a `RoleRevoked` audit entry SHALL be created with the target user recorded