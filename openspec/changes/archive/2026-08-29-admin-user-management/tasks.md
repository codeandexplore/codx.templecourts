## 1. API — Domain Entity

- [x] 1.1 Add `User.ResetPassword(string newPasswordHash)` method — sets PasswordHash and clears refresh tokens
- [x] 1.2 Add `User.Deactivate()` and `User.Activate()` methods — set Status to Inactive/Active

## 2. API — DTOs and Use Cases

- [x] 2.1 Extend `UserDto` with `Status` (string) and `Roles` (List<string>) fields
- [x] 2.2 Update `ListUsersUseCase` to include status and roles in the projection
- [x] 2.3 Create `ResetUserPasswordUseCase` — hash new password, call ResetPassword, save
- [x] 2.4 Create `UpdateUserStatusUseCase` — validate status, call Deactivate/Activate, save
- [x] 2.5 Add request DTOs: `ResetUserPasswordRequest { newPassword }`, `UpdateUserStatusRequest { status }`

## 3. API — Controller Endpoints

- [x] 3.1 Add `POST /admin/users/{userId}/reset-password` to `AdminController`
- [x] 3.2 Add `PUT /admin/users/{userId}/status` to `AdminController`

## 4. UI — Users Tab

- [x] 4.1 Create `src/components/admin/UsersTab.tsx` — searchable user list with name, email, status badge, role badges
- [x] 4.2 Add "Users" tab to `AdminPage.tsx` alongside Lessons, Roles, Assignments
- [x] 4.3 Implement Reset Password dialog — new password field, calls reset-password mutation
- [x] 4.4 Implement Suspend/Reactivate action button — calls status mutation, toggles label
- [x] 4.5 Add `useListUsersQuery` (extend to include status/roles), `useResetUserPasswordMutation`, `useUpdateUserStatusMutation` to `adminApi.ts`
- [x] 4.6 Add `Users` tagType + invalidatesTags so list refreshes after actions

## 5. Verification

- [x] 5.1 Run `dotnet build` and `dotnet test` in API project
- [x] 5.2 Run `pnpm type-check`, `pnpm lint`, `pnpm build` in UI project
- [ ] 5.3 Manually verify: admin resets a password, then logs in with the new password
- [ ] 5.4 Manually verify: admin suspends a user, user can no longer log in
- [ ] 5.5 Manually verify: Users tab lists all users with status and roles
