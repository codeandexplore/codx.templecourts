# Audit Log

## ADDED Requirements

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
