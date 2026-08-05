## Why

Phases 0–4 shipped the full API surface. Phase 5 hardens it for launch: complete lesson content, security validation, audit logging, and regression tests for the most critical multi-system flows.

## What Changes

- Fill 15 placeholder lessons with real content at varying depths (depth-1, depth-2, depth-3)
- Security pass: verify role-check enforcement on all protected endpoints, verify reference_context is never leaked to Student responses
- Audit log: record entries on role elevation (Admin assigns Teacher) and TeacherAssignment creation/reassignment
- Regression tests: mid-attempt version publish, flag block/unblock, depth-3 traversal+gating, reassignment history integrity

## Capabilities

### New Capabilities
- `audit-log`: Audit entries for role elevation and reassignment events.

### Modified Capabilities
<!-- None. Existing specs already cover the behaviors being tested. -->

## Impact

- **Domain**: New `AuditLog` entity.
- **Tests**: New security tests (role enforcement, ref_context leak), regression tests (4 scenarios), expanded seed data.
- **Infrastructure**: AuditLog table + migration.
