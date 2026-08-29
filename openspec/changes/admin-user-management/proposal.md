## Why

The Admin page can currently assign/revoke roles and manage assignments, but there is no way to manage user accounts themselves — an admin cannot reset a forgotten password, suspend a compromised account, or see a user's status and roles in one place. The `GET /admin/users` endpoint exists but returns only id/email/displayName, and there are no password-reset or status-toggle endpoints.

## What Changes

- Extend `GET /admin/users` response to include each user's `Status` (Active/Inactive) and assigned `Roles`
- New endpoint `POST /admin/users/{userId}/reset-password` — admin sets a new password for any user (resets password hash + clears refresh tokens)
- New endpoint `PUT /admin/users/{userId}/status` — admin suspends (Inactive) or reactivates (Active) a user
- New `User.ResetPassword()` and `User.Deactivate()`/`User.Activate()` domain methods
- Admin page gains a "Users" tab with a searchable user list, reset-password dialog, and activate/suspend action

## Capabilities

### New Capabilities
- `admin-user-management`: Admin can list users with status and roles, reset any user's password, and suspend/reactivate users

### Modified Capabilities
- `ui`: Admin page gains a fourth "Users" tab alongside Lessons, Roles, and Assignments

## Impact

- **New use cases**: `ResetUserPasswordUseCase`, `UpdateUserStatusUseCase`
- **Modified use case**: `ListUsersUseCase` (include status + roles)
- **New DTO**: `UserDto` gains `Status` and `Roles` fields; new `ResetUserPasswordRequest`, `UpdateUserStatusRequest`
- **Modified domain entity**: `User` (add ResetPassword, Deactivate, Activate methods)
- **New controller endpoint**: `POST /admin/users/{userId}/reset-password`, `PUT /admin/users/{userId}/status`
- **UI**: New `UsersTab` component, `adminApi` mutations, AdminPage fourth tab
- **No new dependencies**
