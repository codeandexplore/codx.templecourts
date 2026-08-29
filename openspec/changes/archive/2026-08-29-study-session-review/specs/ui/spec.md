## ADDED Requirements

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
