## ADDED Requirements

### Requirement: Admin page shows an Audit Log tab
The Admin Dashboard SHALL include an "Audit Log" tab listing audit entries (action, performer, target user, metadata, timestamp).

#### Scenario: Audit Log tab lists entries
- **WHEN** an Admin selects the Audit Log tab
- **THEN** audit entries SHALL display with action, performer, target, and timestamp

### Requirement: reference_context never served to unauthenticated users
The serialization layer SHALL strip `reference_context` for unauthenticated requests, not just for the Student role.

#### Scenario: Unauthenticated request strips reference_context
- **WHEN** an unauthenticated client receives a response containing a question's `reference_context`
- **THEN** the `reference_context` field SHALL be absent