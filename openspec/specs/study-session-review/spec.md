# Study Session Review

The live review workflow: teachers start study sessions, walk through a student's answers question-by-question, mark answers as reviewed (raising flags for unanswered ones), and end sessions. Real-time SignalR pushes review progress to the student.

## Requirements

### Requirement: Teacher can view questions with student answers in a session
The system SHALL provide an endpoint `GET /api/study-sessions/{sessionId}/questions` that returns the lesson's questions in tree-traversal order with per-question answer status, reviewed status, and flag status. The response SHALL include the student's display name, lesson number and title, session status, and the current question ID.

#### Scenario: Session with answered and unanswered questions
- **WHEN** a teacher fetches questions for a session where the student has answered 3 of 12 questions
- **THEN** the response SHALL list 12 questions in tree-traversal order, 3 with answer objects populated and 9 with null answers

#### Scenario: Session with reviewed questions
- **WHEN** a teacher fetches questions after marking question #3 as reviewed
- **THEN** question #3 SHALL show `isReviewed: true` and other questions SHALL show `isReviewed: false`

#### Scenario: Session with flagged question
- **WHEN** a teacher fetches questions after marking an empty answer as reviewed (creating an AnswerFlag)
- **THEN** the flagged question SHALL include a flag object with type "UnansweredAtReview"

### Requirement: Teacher can start a study session from the student list
The Teacher Page SHALL display a "Start Review" button on each assigned student card when the student has an in-progress LessonAttempt. Clicking the button SHALL call `POST /api/study-sessions` and navigate to `/teacher/review/:sessionId`.

#### Scenario: Start Review with active attempt
- **WHEN** a teacher clicks "Start Review" on a student with an in-progress attempt
- **THEN** a new StudySession SHALL be created and the browser SHALL navigate to the review page

#### Scenario: No Start Review when no attempt exists
- **WHEN** a student has no in-progress LessonAttempt
- **THEN** the "Start Review" button SHALL be hidden or disabled

### Requirement: Teacher can walk through questions in review
The review page at `/teacher/review/:sessionId` SHALL display one question at a time with: the question prompt, question type badge, parent node breadcrumb, student answer text (or "Not yet answered"), a mark-reviewed toggle, and advance/end buttons. The current question SHALL be highlighted in the question map sidebar.

#### Scenario: Navigating between questions
- **WHEN** a teacher clicks "Advance" on question #2
- **THEN** the API SHALL be called to advance the session and question #3 SHALL become the current question

#### Scenario: Marking an answer as reviewed
- **WHEN** a teacher clicks "Mark Reviewed" on a question with a student answer
- **THEN** the answer SHALL be marked as reviewed and the question map SHALL update

#### Scenario: Marking an empty answer as reviewed
- **WHEN** a teacher clicks "Mark Reviewed" on a question where the student has not submitted an answer
- **THEN** an AnswerFlag SHALL be created and the UI SHALL display a flag indicator

### Requirement: Teacher can end a study session
The review page SHALL provide an "End Session" button with a confirmation dialog. On confirmation, the system SHALL call `PUT /api/study-sessions/{sessionId}/end` and navigate back to the student list.

#### Scenario: Ending a session
- **WHEN** a teacher clicks "End Session" and confirms
- **THEN** the session status SHALL change to COMPLETED and the browser SHALL navigate to `/teacher`

### Requirement: Real-time SignalR push to student during review
When a teacher marks an answer as reviewed, advances the session, or ends the session, the server SHALL push the corresponding event to the student via SignalR. The student's Attempt page SHALL display a review status bar showing the teacher's current position.

#### Scenario: Student sees review progress
- **WHEN** an active StudySession exists and the teacher advances to question #4
- **THEN** the student's Attempt page SHALL show "Teacher is reviewing your answers — Question 4 of 12"

#### Scenario: Student sees session ended
- **WHEN** the teacher ends the session
- **THEN** the student's Attempt page SHALL hide the review status bar
