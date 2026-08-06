## ADDED Requirements

### Requirement: Admin Lesson Editor page at /admin/editor
The Admin SHALL have access to a dedicated lesson editor page at `/admin/editor`, accessible via the sidebar "Lesson Editor" link (Admin-only). The page SHALL include a lesson selector dropdown, version manager, recursive tree editor, and slide-out question editor panel. The page SHALL use the design system's serif headings, parchment backgrounds, and shadcn components.

#### Scenario: Editor page renders for Admin user
- **WHEN** an Admin user navigates to `/admin/editor` via the sidebar link
- **THEN** the page SHALL render with a lesson selector, Back to Admin link, and full viewport editor area

#### Scenario: Editor page blocked for non-Admin
- **WHEN** a Teacher or Student navigates to `/admin/editor`
- **THEN** the RequireRole guard SHALL redirect to the permission-denied page

#### Scenario: Sidebar shows Lesson Editor link
- **WHEN** an Admin user views the sidebar
- **THEN** a "Lesson Editor" link with pencil icon SHALL appear in the Admin section

### Requirement: Admin Assignments tab on /admin
The Admin Dashboard SHALL include a third tab "Assignments" alongside the existing "Lessons" and "Roles" tabs. The tab SHALL display all teacher-student assignments with status, dates, and a reassign action. The sidebar SHALL show an "Assignments" link (Admin-only).

#### Scenario: Assignments tab appears on Admin page
- **WHEN** Admin navigates to `/admin`
- **THEN** three tabs SHALL be visible: Lessons, Roles, and Assignments

#### Scenario: Sidebar shows Assignments link
- **WHEN** an Admin user views the sidebar
- **THEN** an "Assignments" link with people icon SHALL appear in the Admin section
