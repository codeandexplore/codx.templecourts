## Context

The Admin page (`/admin`) has three tabs: Lessons, Roles, Assignments. Role management was recently extended with a searchable user dropdown and revoke support, plus a default `Student` role on registration. The `GET /admin/users` endpoint (added for the role dropdown) returns `UserDto(Id, Email, DisplayName)` — no status or role info.

The `User` domain entity has `PasswordHash` (set only via `CreateWithPassword`) and `RefreshTokenHash`, with no method to change a password post-creation or to toggle status. `UserStatus` enum is `Active`/`Inactive` but no domain method mutates it. `IPasswordHasher` provides `Hash` and `Verify`.

## Goals / Non-Goals

**Goals:**
- Admin can see all users with their status and roles in one place
- Admin can reset any user's password (admin chooses the new password)
- Admin can suspend (Inactive) and reactivate (Active) a user
- A fourth "Users" tab on the Admin page

**Non-Goals:**
- Self-service password reset / forgot-password email flow
- Forced password change on next login (no flag)
- Email/notification triggers on password reset or suspension
- Bulk operations (multi-select reset/suspend)
- Soft-delete or permanent user deletion

## Decisions

### D1: Admin sets the new password directly (vs auto-generated temp)

**Choice**: The reset-password dialog has a "New Password" input; admin types it, the server hashes it.

**Rationale**: Simplest to implement and reason about. Auto-generated temporary passwords require returning a one-time secret and a force-change flag for security, which is out of scope.

**Alternative**: Generate random temp password → rejected for MVP (adds one-time-secret and force-change complexity).

### D2: Reset password also clears refresh tokens

**Choice**: `User.ResetPassword(newHash)` sets `PasswordHash` and clears `RefreshTokenHash`/`RefreshTokenExpiresAt`.

**Rationale**: A reset should invalidate existing sessions. Clearing refresh tokens forces re-login everywhere, which is the correct security posture for an account reset.

### D3: Status toggle via single endpoint with explicit target

**Choice**: `PUT /admin/users/{userId}/status` with body `{ status: "Active" | "Inactive" }`.

**Rationale**: One endpoint for both suspend and reactivate; the client sends the desired state. Idempotent.

**Alternative**: Separate suspend/activate endpoints → more endpoints, no benefit for a two-state enum.

### D4: Fourth tab "Users" on the existing Admin page

**Choice**: Add a "Users" tab alongside Lessons, Roles, Assignments.

**Rationale**: Consistent with the existing tabbed dashboard; user management belongs with other admin functions. A dedicated page is unnecessary for a simple list + two actions.

### D5: Reuse the searchable-dropdown pattern for the user list

**Choice**: The Users tab shows a searchable list (filter by name/email) with per-row status badge, role badges, and action buttons.

**Rationale**: Matches the Roles tab's searchable user selector UX; users already know the pattern.

## Risks / Trade-offs

- **[Risk] Admin sets a weak password** → Mitigation: input has `minLength` validation; the app's existing password policy (min 8 chars) applies.
- **[Risk] Suspending a user mid-session** → Mitigation: status is checked at auth/refresh; existing access tokens remain valid until expiry. Acceptable for MVP (tokens are short-lived, 15 min).
- **[Trade-off] No force-change flag** → A user whose password was reset keeps the admin-set password until they change it. Acceptable for MVP.
