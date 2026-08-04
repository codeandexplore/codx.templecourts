## Why

The Temple Courts has a complete architecture plan and scaffolded project structure, but zero application code exists — no authentication, no database schema, no role guards. Before any lesson authoring, student experience, or review features can be built, the foundational identity and data layers must be in place. Phase 0 establishes the bedrock every subsequent phase builds on.

## What Changes

- **Email + password registration/login** with JWT Bearer token issuance and refresh
- **Google OAuth** login and account linking
- **Role system**: ADMIN, TEACHER, STUDENT roles with server-enforced authorization
- **Admin-guard middleware** that rejects non-ADMIN requests to protected endpoints
- **Core database schema** via EF Core migration: `User`, `RoleAssignment`, `Lesson`, `LessonVersion`, `LessonNode`, `Question`
- **Test infrastructure validation**: first unit and integration tests proving the vertical slice works end-to-end (register → get token → call protected endpoint)

## Capabilities

### New Capabilities
- `auth`: Email/password registration and login, Google OAuth authentication, JWT Bearer token issuance and validation, token refresh endpoint
- `roles`: Role assignment (ADMIN, TEACHER, STUDENT), server-enforced role checks, admin-only endpoint gating, teacher/student role availability
- `core-schema`: EF Core entities and database migration for User, RoleAssignment, Lesson, LessonVersion, LessonNode, Question — the six foundational tables all later phases depend on

### Modified Capabilities
<!-- No existing specs to modify — all capabilities are new -->

## Impact

- **API project** (`codx.temple-api`): All four layers affected — Domain entities, Application use cases, Infrastructure (DbContext, repositories, auth handler), API (auth controller, admin-guard middleware, role endpoints)
- **Database**: First EF Core migration creating 6 tables
- **Dependencies**: EF Core 10 + Npgsql already configured; JWT Bearer and Google auth packages already referenced; no new NuGet packages needed
- **Tests**: Domain, Application, API, and Integration test projects will receive their first real tests
- **No UI or E2E impact**: Phase 0 is API-only; UI and E2E remain scaffolding until later phases
