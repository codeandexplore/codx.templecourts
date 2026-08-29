## ADDED Requirements

### Requirement: Admin page Users tab
The Admin Dashboard SHALL include a fourth tab "Users" alongside Lessons, Roles, and Assignments. The tab SHALL display a searchable list of all users with name, email, status badge (Active/Inactive), role badges, and actions to reset password and suspend/reactivate.

#### Scenario: Users tab appears on Admin page
- **WHEN** an Admin navigates to `/admin`
- **THEN** four tabs SHALL be visible: Lessons, Roles, Assignments, and Users

#### Scenario: Users list shows status and roles
- **WHEN** an Admin selects the Users tab
- **THEN** each user SHALL display their name, email, status badge, and role badges

#### Scenario: Reset password action opens dialog
- **WHEN** an Admin clicks "Reset Password" on a user
- **THEN** a dialog SHALL open with a new password field and confirm/cancel buttons

#### Scenario: Suspend/reactivate action toggles status
- **WHEN** an Admin clicks "Suspend" on an active user
- **THEN** the user SHALL be marked Inactive and the list SHALL refresh to show the new status
