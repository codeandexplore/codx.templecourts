# UI

The main UI capability for the Temple Courts frontend. Covers page layouts, sidebar navigation, role-gated content, user-facing pages (Home, Lessons, Lesson Detail, Attempt, Teacher, Admin), and authentication pages (Login, Register).

## Requirements

### Requirement: Sidebar layout with refined brand, icons, and user area
The sidebar (AppLayout) SHALL be redesigned with: serif "The Temple Courts" heading, Heroicons on every navigation link, a subtle divider between brand and nav, and a bottom user section with avatar placeholder and sign-out button. Navigation links SHALL use role-gating (Teacher/Admin links hidden when user lacks role).

#### Scenario: Sidebar renders brand in serif
- **WHEN** the AppLayout renders
- **THEN** "The Temple Courts" heading SHALL display in serif font with `text-xl font-semibold`

#### Scenario: Navigation links show icons
- **WHEN** the sidebar renders navigation links
- **THEN** each link SHALL display its corresponding Heroicon: Home (`AcademicCapIcon`), Lessons (`BookOpenIcon`), Teacher (`UserGroupIcon`), Admin (`Cog6ToothIcon`)

#### Scenario: Teacher link hidden for students
- **WHEN** a user with role "Student" loads any page
- **THEN** the "Teacher" navigation link SHALL NOT be visible in the sidebar

#### Scenario: Sign out available in sidebar
- **WHEN** any authenticated user views the sidebar
- **THEN** the user's display name and a ghost "Sign Out" button SHALL appear at the bottom of the sidebar

### Requirement: Home page with welcome content
The Home page SHALL display a centered welcome section with serif heading, a Luke 2:46 Scripture reference as a decorative pull quote, and brief guiding text about the app's purpose. No data fetching required.

#### Scenario: Home page renders welcoming content
- **WHEN** an authenticated user navigates to `/`
- **THEN** the page SHALL display "Welcome to The Temple Courts" in serif heading, the Luke 2:46 reference in italic serif, and guiding text about Bible study and community

### Requirement: Login and Register pages with centered card layout
Login and Register pages SHALL render outside the sidebar layout as centered cards with: serif heading, form fields with accessible labels and error messages, submit button with loading state, and a link to the alternate page. On authentication success, redirect to `/`. If already authenticated, redirect immediately.

#### Scenario: Login form validates and submits
- **WHEN** a user enters valid email and password and clicks "Sign In"
- **THEN** the form SHALL submit, authenticate, and redirect to `/`

#### Scenario: Login shows validation errors
- **WHEN** a user submits the login form with invalid email or password under 8 characters
- **THEN** inline validation errors SHALL appear below each invalid field

#### Scenario: Authenticated user redirected from login
- **WHEN** an already-authenticated user navigates to `/login`
- **THEN** the page SHALL immediately redirect to `/`

### Requirement: Lessons page with grid of lesson cards
The Lessons page SHALL display all published lessons in a responsive grid of Lesson Cards. Each card SHALL show lesson number, title, a Published badge, and a clickable surface that navigates to `/lessons/:key`. Cards SHALL use the interactive Card variant with hover feedback.

#### Scenario: Published lessons render as cards
- **WHEN** the Lessons page loads with published lessons available
- **THEN** each lesson SHALL render as an interactive Card with number, title, and Published badge

#### Scenario: Click navigates to lesson detail
- **WHEN** a user clicks a lesson card
- **THEN** the browser SHALL navigate to `/lessons/:key` for that lesson

### Requirement: Lesson Detail page with tree viewer and start attempt button
The Lesson Detail page SHALL render the lesson's recursive node tree with: depth-based indentation for nested nodes, gating indicators (lock icon for sibling-gated nodes), question type labels, and a primary "Start Lesson" button that creates a new LessonAttempt. If an existing unfinished attempt exists, SHALL show "Continue Lesson" instead.

#### Scenario: Lesson tree renders with indentation
- **WHEN** a lesson with depth-3 nodes is loaded
- **THEN** nodes at depth 1 SHALL have no indent, depth 2 SHALL have `ml-4`, depth 3 SHALL have `ml-8`

#### Scenario: Gated node shows lock
- **WHEN** a node has `requires_prior_sibling_answered: true` and the sibling is unanswered
- **THEN** the node SHALL display a `LockClosedIcon` and be visually subdued

#### Scenario: Start Lesson creates attempt
- **WHEN** a user clicks "Start Lesson" with no existing attempt
- **THEN** a LessonAttempt SHALL be created and the browser SHALL navigate to `/attempt/:attemptId`

### Requirement: Attempt page with inline question answering
The Attempt page (lesson runner) SHALL flatten the lesson tree into a sequential question list with: question prompt, question type label, textarea for answer input, submit button, answered/not-answered status indicator, and progress counter (N/M). Answered questions SHALL show the submitted text and an "Edit" link.

#### Scenario: Question list renders with progress
- **WHEN** the Attempt page loads with 5 questions and 2 answered
- **THEN** the page SHALL show "2 of 5 answered" and each question SHALL display its status (answered in emerald, unanswered in default)

#### Scenario: Submit answer updates status
- **WHEN** a user types an answer and clicks "Submit"
- **THEN** the answer SHALL be saved via API, the question SHALL show its answered status, and the progress counter SHALL increment

### Requirement: Teacher page with student list
The Teacher page SHALL display the teacher's assigned students as a list of Student Cards showing display name and email. If no students are assigned, it SHALL show a prompt to claim a student. Student Cards SHALL use the default Card component variant.

#### Scenario: Teacher sees assigned students
- **WHEN** a teacher with assigned students loads `/teacher`
- **THEN** each student SHALL render as a Card with display name and email

#### Scenario: Teacher with no students sees prompt
- **WHEN** a teacher with no assigned students loads `/teacher`
- **THEN** the page SHALL display a prompt message about claiming a student

### Requirement: Admin page with lesson and role management tabs
The Admin page SHALL use the shadcn Tabs component with two tabs: "Lessons" (list all lessons with create button and dialog) and "Roles" (list role assignments with assign form). The Create Lesson dialog SHALL include lesson number and title fields. The Assign Role form SHALL include user ID, role dropdown, and submit button.

#### Scenario: Lessons tab lists all lessons
- **WHEN** the admin selects the Lessons tab
- **THEN** all lessons SHALL render with number, title, and status. A "Create Lesson" button SHALL open the create dialog.

#### Scenario: Roles tab allows role assignment
- **WHEN** the admin selects the Roles tab and fills in a user ID and role
- **THEN** clicking "Assign Role" SHALL call the assign-role API and refresh the role list

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

### Requirement: Teacher Study Session Review page at /teacher/review/:sessionId
The Teacher SHALL have access to a dedicated review page at `/teacher/review/:sessionId`, accessible via the student list. The page SHALL include a header with session info (lesson title, student name, progress counter), a question map sidebar showing all questions with status indicators, a question display area showing the current question prompt and student answer, and review controls (mark-reviewed, advance, end). The page SHALL use the design system's serif headings, parchment backgrounds, and shadcn components.

#### Scenario: Review page renders for Teacher
- **WHEN** a Teacher navigates to `/teacher/review/:sessionId` after starting a session
- **THEN** the page SHALL render with the session header, question map, and current question display

#### Scenario: Review page blocked for non-Teacher
- **WHEN** a Student or Admin navigates to `/teacher/review/:sessionId`
- **THEN** the RequireRole guard SHALL redirect to the permission-denied page

### Requirement: Student review status bar on Attempt page
The Attempt page SHALL display a review status bar when an active StudySession exists for the current LessonAttempt. The bar SHALL show which question the teacher is currently reviewing and the total question count. The bar SHALL connect to SignalR to receive live updates.

#### Scenario: Review bar appears during active session
- **WHEN** a student loads the Attempt page and an active StudySession exists
- **THEN** a status bar SHALL display at the top showing "Teacher is reviewing your answers — Question N of M"

#### Scenario: Review bar updates on teacher advance
- **WHEN** the teacher advances to a new question during the session
- **THEN** the review bar SHALL update to show the new question number via SignalR

#### Scenario: Review bar disappears when session ends
- **WHEN** the teacher ends the session
- **THEN** the review bar SHALL be removed from the Attempt page via SignalR

### Requirement: Teacher page shows Start Review button
The Teacher page student list SHALL display a "Start Review" button on each student card. The button SHALL be visible only when the student has an in-progress LessonAttempt. The `TeacherAssignmentDto` SHALL include an optional `latestAttemptId` field.

#### Scenario: Start Review button visible for student with attempt
- **WHEN** the teacher views a student with an in-progress LessonAttempt
- **THEN** a "Start Review" button SHALL be visible on that student's card

#### Scenario: Start Review button hidden for student without attempt
- **WHEN** the teacher views a student with no in-progress LessonAttempt
- **THEN** no "Start Review" button SHALL be visible
