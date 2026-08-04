## Context

Phase 0 is the first implementation phase for The Temple Courts. The API project has a Clean Architecture skeleton (Domain → Application → Infrastructure → API) with NuGet packages installed but zero application code. Before any lesson authoring, student experience, or review features can be built, we need working authentication (email/password + Google OAuth), a role system with admin-only endpoints, and the six core database tables that all later phases depend on.

The workspace uses PostgreSQL 16 (via Docker Compose), EF Core 10 with Npgsql, and JWT Bearer authentication. No ASP.NET Core Identity — the project follows a custom, concrete-use-case pattern.

## Goals / Non-Goals

**Goals:**
- Email/password registration and login issuing JWT access + refresh tokens
- Google OAuth login (one-time code exchange, account creation or linking)
- JWT Bearer middleware validating tokens on every request
- Token refresh endpoint (rotating refresh tokens)
- ADMIN, TEACHER, STUDENT roles stored in `RoleAssignment` table
- Middleware rejecting non-ADMIN requests to admin-only endpoints
- `ICurrentUserAccessor` resolving user identity + roles from JWT claims
- EF Core migration creating the six core tables
- Full test coverage: unit tests (Domain), use case tests (Application), controller tests (API), integration tests (end-to-end auth flow)

**Non-Goals:**
- UI login pages or OAuth redirect flows — Phase 0 is API-only
- Teacher/Student assignment logic (Phase 3a)
- Notification system (Phase 3b/4)
- Google Meet integration
- Encourager role
- Refresh token revocation list (acceptable for MVP; add later)
- Password reset flow (deferred)

## Decisions

### D1: Custom auth instead of ASP.NET Core Identity

**Choice:** Custom `User` entity with `password_hash` (BCrypt) and `google_id` fields. Manual JWT issuance via `Microsoft.AspNetCore.Authentication.JwtBearer`. Manual Google OAuth token exchange.

**Alternatives considered:**
- ASP.NET Core Identity: gives password hashing, user management, and OAuth for free, but its `UserManager<T>` is tightly coupled to EF Core and pollutes the Domain layer with Identity dependencies. The clean Domain layer is a non-negotiable architectural constraint.
- Duende IdentityServer: overkill for MVP; adds infrastructure complexity and licensing considerations.

**Rationale:** A custom approach keeps the Domain layer pure (no Identity references), follows the project's concrete-use-case pattern, and gives full control over the JWT payload shape to match the project's `application_role` claim convention.

### D2: BCrypt.Net-Next for password hashing

**Choice:** `BCrypt.Net-Next` NuGet package. Hash on registration, verify on login. Salt is embedded in the hash — no separate salt column needed.

**Alternatives considered:**
- Argon2id: stronger but requires a separate library with more complex configuration.
- PBKDF2 (built-in): adequate but slower to verify than BCrypt at equivalent security.

**Rationale:** BCrypt is well-tested, simple to integrate (one method call each for hash/verify), and the `password_hash` column stores the full BCrypt string (including embedded salt). Good enough for MVP.

### D3: JWT structure and claim mapping

**Choice:** JWT issued by the API itself (symmetric key, HS256). Claims:
- `sub`: user ID (Guid)
- `email`: user email
- `application_role`: comma-separated roles (e.g. "ADMIN,TEACHER")
- `exp`, `iat`, `jti`: standard JWT claims

`MapInboundClaims = false` to preserve original claim types. `RoleClaimType = "application_role"` to match project conventions.

**Alternatives considered:**
- Asymmetric signing (RS256): more secure for distributed systems but adds key management complexity. Overkill for a monolith API in MVP.
- Auth0/Clerk external provider: adds cost, latency, and dependency on a third party.

**Rationale:** Symmetric key with HS256 is the simplest approach for a single API. We can migrate to RS256 later if we need service-to-service auth.

### D4: Refresh token rotation

**Choice:** Refresh tokens stored as a column on the `User` entity (`refresh_token_hash`, `refresh_token_expires_at`). On refresh: validate the token, issue new access + refresh token pair, rotate the stored refresh token. Single active refresh token per user (issuing a new one invalidates the old).

**Alternatives considered:**
- Separate `RefreshToken` table: allows multiple devices but adds complexity. Not needed for MVP.
- No refresh tokens: forces re-login on every access token expiry. Poor UX.

**Rationale:** Storing the refresh token on the User row keeps the schema simple and enforces one-session-at-a-time, which aligns with the 1:1 Teacher↔Student model.

### D5: Google OAuth flow

**Choice:** API endpoint `POST /auth/google` accepts an `id_token` from the Google Sign-In client SDK. The API validates the token locally — fetches Google's JWKS from `https://www.googleapis.com/oauth2/v3/certs`, verifies the RS256 signature, and validates `aud` (must match the configured Client ID), `iss` (must be `accounts.google.com` or `https://accounts.google.com`), and `exp` (must not be expired). Extracts email, name, and `sub` (Google account ID) from the validated claims. Either creates a new User (with `google_id` set) or links to an existing User by email.

**Why local JWKS validation over the deprecated tokeninfo endpoint:** Google's `oauth2/v3/tokeninfo` endpoint is officially deprecated. Local validation is faster (no network call per login, JWKS keys are cached), more reliable (no external dependency at request time), and is Google's recommended approach.

**Alternatives considered:**
- Google `tokeninfo` endpoint: deprecated, adds ~200ms latency per login, and is a single-point-of-failure.
- Full OAuth redirect flow (API handles the redirect dance): adds complexity of callback URLs, state parameters, and session management. Better handled by the SPA client, which already has the Google SDK.
- Firebase Auth proxy: adds another service dependency.

**Rationale:** The SPA handles the Google OAuth consent screen; it sends the resulting `id_token` to the API. The API only needs to validate and extract claims — simple, stateless, and the same pattern works for mobile later. Local JWKS validation is the correct, performant, and future-proof approach.

**Setup required:** See `docs/workspace-setup.md` §8 for Google Cloud Console setup. The only config value needed is the OAuth Client ID, stored via `dotnet user-secrets set "GoogleAuth:ClientId" "..."`.

### D6: Admin-guard implementation

**Choice:** An ASP.NET Core `IAuthorizationMiddlewareResultHandler` combined with a custom `[RequireRole(Role.ADMIN)]` authorization attribute. Returns 403 Forbidden for non-ADMIN requests. Simple, declarative, and avoids duplicating role checks in every controller.

**Rationale:** This follows the standard ASP.NET authorization pipeline and integrates with Swagger for documentation. Controllers stay clean — just add the attribute.

### D7: Entity design — _key fields as stable identity

**Choice:** `LessonNode`, `Question`, and `Lesson` each get a `_key` field (Guid, generated once at creation, survives version bumps) and an auto-incrementing integer `id` (version-specific). Relationships use `_key` for stable references. The full versioning workflow (clone → edit → publish) is a Phase 1 concern, but the schema must support it from Phase 0.

**Rationale:** The architecture doc is explicit about this two-tier identity model. Building the schema with both fields from the start avoids a Phase 1 migration that adds `_key` columns and backfills them.

### D8: No repository interfaces for simple CRUD

**Choice:** Use cases access the `AppDbContext` directly (injected via DI). This follows the Ezra-derived pattern of concrete use cases without unnecessary abstraction layers.

**Rationale:** EF Core's `DbSet<T>` is already a repository pattern. Wrapping it in another interface adds indirection without value in a monolith. The `IUnitOfWork` abstraction is kept for `SaveChangesAsync` to enable test mocking.

## Risks / Trade-offs

- **Single refresh token per user** means logging in on a second device invalidates the first session. Acceptable for MVP; revisit if multi-device becomes a requirement.
- **Symmetric key (HS256)** means the signing key is a shared secret. If the API ever needs to issue tokens verified by other services, we'll need to migrate to RS256.
- **Google JWKS endpoint outage** would prevent new Google logins if the cached keys expire during the outage. Mitigated by long JWKS cache TTL (24 hours) and the fact that existing users can still log in with email/password.
- **No password reset flow** in Phase 0 means a forgotten password requires admin intervention (manual hash reset). Must be addressed before production use.

## Open Questions

- **Seed data strategy**: Should Phase 0 include a database seeder that creates the initial ADMIN user (e.g., via environment variable for email/password)? Or is that a separate operational concern?
- **Google OAuth Client ID configuration**: Which Google Cloud project? Need to add the Client ID to `appsettings.json` or user secrets.
- **Token expiry durations**: Proposed 15-minute access tokens, 7-day refresh tokens. Confirm these are acceptable.
