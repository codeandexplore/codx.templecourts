## 1. API — Audit Log

- [x] 1.1 Add `RoleRevoked` audit entry in `RevokeRoleUseCase`
- [x] 1.2 Create `AuditLogDto` and `ListAuditLogsUseCase`
- [x] 1.3 Add `GET /admin/audit-logs` to `AdminController`

## 2. API — Security Fix

- [x] 2.1 Fix `RoleAwareJsonTypeResolver` to strip `reference_context` for unauthenticated requests
- [x] 2.2 Update the `Unauthenticated_ShouldStillSee_ReferenceContext` test to assert the field is absent

## 3. UI — Audit Log Tab

- [x] 3.1 Add `listAuditLogs` query + `AuditLogDto` type to `adminApi`
- [x] 3.2 Add "Audit Log" tab to `AdminPage`

## 4. Regression Tests

- [x] 4.1 Flag block/unblock: unresolved flag blocks new attempt; resolving unblocks
- [ ] 4.2 Version bump mid-attempt: in-flight attempt unaffected by new published version (deferred — needs integration test infra)
- [ ] 4.3 Reassignment: thread/flag history stays attributed across reassignment (deferred — needs integration test infra)
- [x] 4.4 Depth-3 mixed-depth traversal and sibling gating

## 5. Verification

- [x] 5.1 Run `dotnet build` and `dotnet test` in API project
- [x] 5.2 Run `pnpm type-check`, `pnpm lint`, `pnpm build` in UI project
- [x] 5.3 Manually verify: admin sees audit log; unauthenticated reference_context is stripped
