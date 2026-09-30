## Why

The audit log writes role elevation and reassignment events but has no way to read them and doesn't cover role revocation. The `reference_context` serialization only strips for the Student role — unauthenticated callers can still receive it. None of the four Phase 5 regression scenarios are covered by tests.

## What Changes

- Audit role revocation (add a `RoleRevoked` audit entry)
- `GET /admin/audit-logs` (admin-only) returning audit entries, plus an "Audit Log" tab on the Admin page
- Fix the `reference_context` leak: strip it for unauthenticated callers (not just Student role)
- Add the four regression tests (flag block/unblock, version-bump-mid-attempt, reassignment attribution, depth-3 traversal/gating)

## Capabilities

### Modified Capabilities
- `audit-log`: Adds the role-revocation audit entry and a read endpoint.
- `ui`: Adds an "Audit Log" tab to the Admin page.

## Impact

- **New use case**: `ListAuditLogsUseCase`, `AuditLogDto`
- **Modified use case**: `RevokeRoleUseCase` (audit entry)
- **Modified serialization**: `RoleAwareJsonTypeResolver` (strip for unauthenticated)
- **New endpoint**: `GET /admin/audit-logs`
- **Modified UI**: `AdminPage` (Audit Log tab), `adminApi`
- **Tests**: regression tests for the four scenarios
- **No new entities or migrations**