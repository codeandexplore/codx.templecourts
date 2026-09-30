# audit-log Specification

## Purpose
TBD - created by archiving change phase-5-hardening. Update Purpose after archive.
## Requirements
### Requirement: Audit entries recorded on role elevation

When an Admin assigns the Teacher role to a user, the system SHALL record an audit entry.

#### Scenario: Role elevation audit
- **WHEN** an Admin assigns the Teacher role
- **THEN** an AuditLog entry is created with action "RoleAssigned", including the target user and role

### Requirement: Audit entries recorded on reassignment

When an Admin reassigns a student to a different teacher, the system SHALL record an audit entry.

#### Scenario: Reassignment audit
- **WHEN** an Admin reassigns a student
- **THEN** an AuditLog entry is created with action "StudentReassigned", including both old and new teacher IDs


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
