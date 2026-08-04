## 1. Domain Layer — Enums & Value Objects

- [x] 1.1 Create `UserStatus` enum (ACTIVE, INACTIVE) in `Domain/Enums/`
- [x] 1.2 Create `Role` enum (ADMIN, TEACHER, STUDENT) in `Domain/Enums/`
- [x] 1.3 Create `LessonStatus` enum (ACTIVE, ARCHIVED) in `Domain/Enums/`
- [x] 1.4 Create `LessonVersionStatus` enum (DRAFT, PUBLISHED, RETIRED) in `Domain/Enums/`
- [x] 1.5 Create `QuestionType` enum (YES_NO, TRUE_FALSE, FILL_BLANK, SELECT_EMBEDDED, ESSAY) in `Domain/Enums/`

## 2. Domain Layer — Entities

- [x] 2.1 Create `User` entity with all fields from spec (id, email, password_hash, google_id, display_name, refresh_token_hash, refresh_token_expires_at, status, created_at) in `Domain/Entities/`
- [x] 2.2 Create `RoleAssignment` entity (id, user_id, role, assigned_by, assigned_at) with composite unique constraint on (user_id, role) in `Domain/Entities/`
- [x] 2.3 Create `Lesson` entity (id, _key, number, title, current_published_version_id, status) in `Domain/Entities/`
- [x] 2.4 Create `LessonVersion` entity (id, lesson_id, version_number, status, cloned_from_version_id, change_notes, published_at, created_at) in `Domain/Entities/`
- [x] 2.5 Create `LessonNode` entity (id, _key, lesson_version_id, parent_node_id, depth, order, title, description, requires_prior_sibling_answered) with self-referencing FK in `Domain/Entities/`
- [x] 2.6 Create `Question` entity (id, _key, lesson_node_id, order, question_type, prompt_text, metadata, reference_context) in `Domain/Entities/`

## 3. Application Layer — Auth Abstractions

- [x] 3.1 Add `IPasswordHasher` interface (Hash, Verify methods) in `Application/Abstractions/`
- [x] 3.2 Add `ITokenService` interface (GenerateAccessToken, GenerateRefreshToken, ValidateRefreshToken, GetPrincipalFromExpiredToken) in `Application/Abstractions/`
- [x] 3.3 Add `IGoogleAuthService` interface (ValidateIdTokenAsync returning email + name + googleId) in `Application/Abstractions/`

## 4. Application Layer — Auth DTOs

- [x] 4.1 Create `RegisterRequest` DTO (email, password, display_name) in `Application/DTOs/Auth/`
- [x] 4.2 Create `LoginRequest` DTO (email, password) in `Application/DTOs/Auth/`
- [x] 4.3 Create `GoogleAuthRequest` DTO (id_token) in `Application/DTOs/Auth/`
- [x] 4.4 Create `RefreshTokenRequest` DTO (refresh_token) in `Application/DTOs/Auth/`
- [x] 4.5 Create `AuthResponse` DTO (access_token, refresh_token, expires_at, user with id, email, display_name, roles) in `Application/DTOs/Auth/`
- [x] 4.6 Create `UserProfileDto` DTO (id, email, display_name, roles, status) in `Application/DTOs/Users/`

## 5. Application Layer — Auth Use Cases

- [x] 5.1 Create `RegisterUseCase` — validate uniqueness, hash password via IPasswordHasher, create User, generate tokens via ITokenService, return AuthResponse
- [x] 5.2 Create `LoginUseCase` — find user by email, verify password via IPasswordHasher, generate + rotate tokens, return AuthResponse
- [x] 5.3 Create `GoogleAuthUseCase` — validate id_token via IGoogleAuthService, find-or-create user (link by email if exists), generate tokens, return AuthResponse
- [x] 5.4 Create `RefreshTokenUseCase` — validate refresh token, rotate it, generate new access + refresh token pair, return AuthResponse
- [x] 5.5 Create `GetCurrentUserUseCase` — resolve user from ICurrentUserAccessor, return UserProfileDto

## 6. Application Layer — Auth Validators

- [x] 6.1 Create `RegisterRequestValidator` (FluentValidation: email required/valid, password min 8 chars, display_name required/max 100) in `Application/Validators/`
- [x] 6.2 Create `LoginRequestValidator` (FluentValidation: email required/valid, password required) in `Application/Validators/`
- [x] 6.3 Create `GoogleAuthRequestValidator` (FluentValidation: id_token required) in `Application/Validators/`

## 7. Application Layer — Role Abstractions & Use Cases

- [x] 7.1 Add `ICurrentUserAccessor` interface (already exists — verify it has UserId, Email, DisplayName, Roles, IsInRole) in `Application/Abstractions/`
- [x] 7.2 Create `AssignRoleUseCase` — validate caller is ADMIN, check no duplicate (user_id, role), create RoleAssignment, call SaveChangesAsync
- [x] 7.3 Create `ListRoleAssignmentsUseCase` — returns all RoleAssignment records with user and assigner details
- [x] 7.4 Create DTOs for role assignment request/response in `Application/DTOs/Admin/`

## 8. Infrastructure Layer — EF Core DbContext & Entity Configuration

- [x] 8.1 Create `AppDbContext` with DbSet<User>, DbSet<RoleAssignment>, DbSet<Lesson>, DbSet<LessonVersion>, DbSet<LessonNode>, DbSet<Question> in `Infrastructure/Data/`
- [x] 8.2 Create `UserConfiguration` (IEntityTypeConfiguration<User>) — PK, unique email index, column types, nullable password_hash/google_id
- [x] 8.3 Create `RoleAssignmentConfiguration` — PK, FK to User, composite unique index on (user_id, role), FK assigned_by → User
- [x] 8.4 Create `LessonConfiguration` — PK, unique index on _key, FK current_published_version_id → LessonVersion (nullable)
- [x] 8.5 Create `LessonVersionConfiguration` — PK, FK to Lesson, FK cloned_from_version_id → LessonVersion (nullable)
- [x] 8.6 Create `LessonNodeConfiguration` — PK, unique index on _key, FK to LessonVersion, self-referencing FK parent_node_id (nullable), depth column
- [x] 8.7 Create `QuestionConfiguration` — PK, unique index on _key, FK to LessonNode, JSON columns for metadata and reference_context

## 9. Infrastructure Layer — Auth Implementations

- [x] 9.1 Add `BCrypt.Net-Next` NuGet package to Infrastructure project
- [x] 9.2 Create `PasswordHasher` implementing `IPasswordHasher` (BCrypt.HashPassword, BCrypt.Verify) in `Infrastructure/Auth/`
- [x] 9.3 Create `TokenService` implementing `ITokenService` — HS256 symmetric key from config, issues JWT with sub/email/application_role claims, generates cryptographically random refresh tokens, hashes refresh tokens before storage in `Infrastructure/Auth/`
- [x] 9.4 Create `GoogleAuthService` implementing `IGoogleAuthService` — calls Google tokeninfo endpoint, validates audience, extracts email/name/sub in `Infrastructure/Auth/`

## 10. Infrastructure Layer — DI Registration & Unit of Work

- [x] 10.1 Create `InfrastructureExtensions` class with `AddInfrastructure(IServiceCollection, IConfiguration)` — registers AppDbContext, all service implementations, IUnitOfWork in `Infrastructure/`
- [x] 10.2 Create `UnitOfWork` implementing `IUnitOfWork` — wraps AppDbContext.SaveChangesAsync in `Infrastructure/Data/`
- [x] 10.3 Update `Application/Abstractions/IUnitOfWork` interface if needed (verify it has SaveChangesAsync with CancellationToken)

## 11. Infrastructure Layer — First EF Core Migration

- [x] 11.1 Configure connection string in `appsettings.Development.json` (PostgreSQL: Host=localhost;Database=templecourts;Username=templecourts;Password=templecourts_dev)
- [x] 11.2 Create EF Core migration: `dotnet ef migrations add InitialCreate` generating all 6 tables
- [x] 11.3 Verify migration is reversible: `dotnet ef migrations remove` and re-add cleanly
- [x] 11.4 Apply migration against local PostgreSQL (Docker Compose) and verify all 6 tables exist with correct schema

## 12. API Layer — JWT & Auth Wiring

- [x] 12.1 Add JWT configuration section to `appsettings.json` (Jwt: Secret, Issuer, Audience, AccessTokenExpiryMinutes, RefreshTokenExpiryDays)
- [x] 12.2 Update `Program.cs` — call `AddInfrastructure()`, add JWT Bearer authentication with `MapInboundClaims = false`, `RoleClaimType = "application_role"`, configure Serilog
- [x] 12.3 Add HTTPS, CORS, and exception handling middleware to `Program.cs`

## 13. API Layer — ICurrentUserAccessor Implementation

- [x] 13.1 Create `CurrentUserAccessor` implementing `ICurrentUserAccessor` — extracts UserId from `sub` claim, Email, DisplayName, Roles from `application_role` claim split by comma, using IHttpContextAccessor in `API/Middleware/`

## 14. API Layer — Auth Controller

- [x] 14.1 Create `AuthController` with POST `/auth/register` (anonymous) delegating to RegisterUseCase
- [x] 14.2 Add POST `/auth/login` (anonymous) delegating to LoginUseCase
- [x] 14.3 Add POST `/auth/google` (anonymous) delegating to GoogleAuthUseCase
- [x] 14.4 Add POST `/auth/refresh` (anonymous) delegating to RefreshTokenUseCase
- [x] 14.5 Add GET `/auth/me` (authenticated) delegating to GetCurrentUserUseCase

## 15. API Layer — Role Authorization

- [x] 15.1 Create `RequireRoleAttribute` (custom authorization attribute accepting params Role[]) in `API/Authorization/`
- [x] 15.2 Create `RequireRoleHandler` (AuthorizationHandler) — checks if user has required role from `application_role` claim, fails with 403 if not
- [x] 15.3 Register authorization handler and policy in `Program.cs`

## 16. API Layer — Admin Controller

- [x] 16.1 Create `AdminController` with `[RequireRole(Role.ADMIN)]` at class level
- [x] 16.2 Add GET `/admin/role-assignments` delegating to ListRoleAssignmentsUseCase
- [x] 16.3 Add POST `/admin/role-assignments` delegating to AssignRoleUseCase

## 17. API Layer — Error Handling & Seeding

- [x] 17.1 Create `GlobalExceptionHandler` implementing `IExceptionHandler` — maps NotFoundException → 404, ConflictException → 409, ForbiddenException → 403, ValidationException → 422, generic → 500 in `API/Middleware/`
- [x] 17.2 Create database seeder — if no ADMIN user exists, create one from config values (AdminEmail, AdminPassword) and assign ADMIN role in `Infrastructure/Data/`
- [x] 17.3 Add health check endpoints (`/healthz`, `/readyz`) with EF Core health check in `Program.cs`

## 18. Tests — Domain Unit Tests

- [x] 18.1 Create `User_Should` test class — validates entity creation, email uniqueness constraint, nullable password_hash/google_id scenarios in `Domain.Tests/`
- [x] 18.2 Create `RoleAssignment_Should` test class — validates role enum values, composite unique constraint scenarios
- [x] 18.3 Create `LessonNode_Should` test class — validates depth 1–3, self-referencing parent, leaf-only constraint

## 19. Tests — Application Use Case Tests

- [x] 19.1 Create `RegisterUseCase_Should` — mock IPasswordHasher/ITokenService, test successful registration, duplicate email → ConflictException, weak password → ValidationException in `Application.Tests/`
- [x] 19.2 Create `LoginUseCase_Should` — mock IPasswordHasher/ITokenService, test successful login, invalid credentials → ForbiddenException
- [x] 19.3 Create `GoogleAuthUseCase_Should` — mock IGoogleAuthService, test new user creation, returning user, email-linking scenario
- [x] 19.4 Create `RefreshTokenUseCase_Should` — mock ITokenService, test successful refresh, expired/invalid token → ForbiddenException
- [x] 19.5 Create `AssignRoleUseCase_Should` — test successful assignment, duplicate rejection, non-admin blocked

## 20. Tests — API Controller + Integration Tests

- [x] 20.1 Create `AuthController_Should` — unit tests for each endpoint returning correct HTTP status codes with mocked use cases in `API.Tests/`
- [x] 20.2 Create `AdminController_Should` — tests for admin role assignment, non-admin rejection
- [x] 20.3 Create integration test: full auth flow — register → login → access protected endpoint → 200 in `API.IntegrationTests/`
- [x] 20.4 Create integration test: unauthorized access — no token → 401, teacher token → admin endpoint → 403
- [x] 20.5 Create integration test: Google auth flow — simulate valid Google token → new user created → login
- [x] 20.6 Create integration test: `reference_context` is not leaked in student-facing response (verify JSON serialization excludes the field for STUDENT role)

## 21. Polish & Validation

- [x] 21.1 Run `dotnet build` — zero errors, zero warnings across all projects
- [x] 21.2 Run `dotnet test` — all tests pass
- [x] 21.3 Verify Swagger UI shows all endpoints at `/swagger` with correct auth documentation
- [x] 21.4 Bump API version to `0.2.0` (minor — new endpoints + schema)
