## Context

The audit log has `AuditLog` entity + migration + two write sites (`AssignRoleUseCase` for Teacher/Admin elevation, `ReassignStudentUseCase` for reassignment). There is no read path, and `RevokeRoleUseCase` does not audit. `RoleAwareJsonTypeResolver` strips `referenceContext` only for the `Student` role; unauthenticated requests serialize it (leak). The four Phase 5 regression scenarios have no coverage.

## Goals / Non-Goals

**Goals:**
- Audit role revocation
- Admin can list audit log entries via API + view them in the Admin page
- Unauthenticated callers never receive `reference_context`
- Cover the four regression scenarios with tests

**Non-Goals:**
- Rate limiting, security headers, OpenTelemetry wiring (net-new, deferred)
- Admin input validation (deferred)
- OAuth flow (deferred — no OAuth provider configured)

## Decisions

### D1: Audit read as admin-only list endpoint

**Choice**: `GET /admin/audit-logs` returns all audit entries newest-first, `[RequireRole("Admin")]`.

**Rationale**: Matches the existing admin oversight pattern; no filtering/pagination for MVP (audit volume is small).

### D2: Revoke-role audit with target user

**Choice**: `RevokeRoleUseCase` writes `AuditLog.Create("RoleRevoked", adminId, targetUserId)` before removing the assignment, mirroring `AssignRoleUseCase`.

**Rationale**: Completes the elevation/revocation pair the audit-log spec implies.

### D3: reference_context stripped for unauthenticated

**Choice**: `RoleAwareJsonTypeResolver` returns `false` (strip) when the request is unauthenticated, not `true`.

**Rationale**: Unauthenticated users must never see teacher guidance. The current behavior returns `true` (serialize) for unauthenticated — the leak.

### D4: Regression tests as integration tests where traversal/DB is required

**Choice**: The four scenarios are integration tests using the existing test host (flag block/unblock can also be covered at the application layer with mocks).

**Rationale**: Traversal, version pinning, and reassignment history need real EF behavior.

## Risks / Trade-offs

- **[Risk] Integration test infra** — the existing integration test host has a "server not started" failure in CI. Mitigation: follow the working `ReferenceContextLeakTests` pattern; where possible, write application-layer tests with mocks that pass reliably.
- **[Trade-off] No audit filtering/pagination** — acceptable for MVP; can add later.
