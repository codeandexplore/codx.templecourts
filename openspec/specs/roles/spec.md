# roles Specification

## Purpose
TBD - created by archiving change phase-0-foundations. Update Purpose after archive.
## Requirements
### Requirement: Roles are assigned via RoleAssignment records

The system SHALL store role assignments in a `RoleAssignment` table with columns for `user_id`, `role` (enum: ADMIN, TEACHER, STUDENT), `assigned_by`, and `assigned_at`. A user MAY have multiple role assignments (e.g., a user can be both ADMIN and TEACHER).

#### Scenario: Admin assigns a role
- **WHEN** an ADMIN creates a `RoleAssignment` record for a user with role TEACHER
- **THEN** the system persists the assignment and the user's subsequent JWT tokens include the TEACHER role

#### Scenario: User with multiple roles
- **WHEN** a user has both ADMIN and TEACHER RoleAssignment records
- **THEN** their JWT `application_role` claim contains both "ADMIN" and "TEACHER"

### Requirement: ICurrentUserAccessor resolves the current user's identity and roles

The system SHALL provide an `ICurrentUserAccessor` service that extracts the current user's ID, email, display name, and roles from the JWT claims in `HttpContext`. This service SHALL be available in the Application layer via the abstraction defined in `Application.Abstractions`.

#### Scenario: Authenticated user context
- **WHEN** a request arrives with a valid JWT containing `sub`, `email`, and `application_role` claims
- **THEN** `ICurrentUserAccessor.UserId` returns the user's GUID, `Email` returns the email, and `Roles` returns the list of roles

#### Scenario: Unauthenticated request
- **WHEN** a request arrives without a valid JWT (anonymous endpoint or missing token)
- **THEN** `ICurrentUserAccessor.UserId` returns `Guid.Empty` and `Roles` returns an empty collection

### Requirement: Admin-only endpoints are gated by role

The system SHALL reject requests to admin-only endpoints when the caller does not have the ADMIN role. The check SHALL return 403 Forbidden (not 401 — the user is authenticated but not authorized). The gating SHALL use a declarative authorization attribute, not inline role checks in controllers.

#### Scenario: Admin accesses admin endpoint
- **WHEN** a user with ADMIN role sends a request to an endpoint decorated with `[RequireRole(Role.ADMIN)]`
- **THEN** the request proceeds normally

#### Scenario: Non-admin accesses admin endpoint
- **WHEN** a user with TEACHER role sends a request to an endpoint decorated with `[RequireRole(Role.ADMIN)]`
- **THEN** the system returns 403 Forbidden

#### Scenario: Unauthenticated user accesses admin endpoint
- **WHEN** a user without any JWT token sends a request to an admin-only endpoint
- **THEN** the system returns 401 Unauthorized (authentication fails before authorization)

### Requirement: Admin can list and manage role assignments

The system SHALL provide endpoints for an ADMIN to create and list `RoleAssignment` records. Role assignment changes SHALL be auditable — the `assigned_by` and `assigned_at` fields are always populated.

#### Scenario: Admin lists all role assignments
- **WHEN** an ADMIN sends a GET request to `/admin/role-assignments`
- **THEN** the system returns all RoleAssignment records with user and assigner details

#### Scenario: Admin assigns a role to a user
- **WHEN** an ADMIN sends a POST request to `/admin/role-assignments` with a `user_id` and `role`
- **THEN** the system creates the RoleAssignment record and returns 201 Created

#### Scenario: Non-admin attempts role management
- **WHEN** a TEACHER sends a request to `/admin/role-assignments`
- **THEN** the system returns 403 Forbidden

