## ADDED Requirements

### Requirement: Teacher and Student can fetch an answer thread
The system SHALL provide `GET /api/threads/{threadId}` returning the thread with its messages ordered chronologically. Only the thread's student or that student's active primary teacher SHALL access it.

#### Scenario: Student fetches own thread
- **WHEN** a Student sends `GET /api/threads/{id}` for their thread
- **THEN** the response SHALL include the thread status and all messages ordered by creation time

#### Scenario: Teacher fetches a student's thread
- **WHEN** a Teacher sends `GET /api/threads/{id}` for an assigned student's thread
- **THEN** the response SHALL include the thread with its messages

#### Scenario: Non-participant blocked
- **WHEN** a user who is neither the student nor the active teacher sends `GET /api/threads/{id}`
- **THEN** the system SHALL return 403 Forbidden

### Requirement: Thread posting enforces participant authorization and lock state
`POST /api/threads/{threadId}/messages` SHALL only allow the thread's student or active primary teacher to post. Posting to a locked thread SHALL return 400 Bad Request.

#### Scenario: Non-participant cannot post
- **WHEN** a user who is not a thread participant sends `POST /api/threads/{id}/messages`
- **THEN** the system SHALL return 403 Forbidden

#### Scenario: Posting to a locked thread
- **WHEN** a participant posts to a thread whose status is Locked
- **THEN** the system SHALL return 400 Bad Request and NOT create a message

### Requirement: Thread ID exposed per answered question
The teacher session-questions response and the student attempt-answers response SHALL include the `threadId` for each answered question that has a thread.

#### Scenario: Session questions include thread IDs
- **WHEN** a teacher fetches session questions for a lesson with answered questions
- **THEN** each answered question SHALL include its thread ID

#### Scenario: Attempt answers include thread IDs
- **WHEN** a student fetches their attempt answers
- **THEN** each answer SHALL include its thread ID

### Requirement: Teacher can manage a check-question bank with orphan detection
The system SHALL provide full CRUD for a teacher's check-questions: `GET /api/check-questions` (list), `GET /api/check-questions/{id}`, `PUT /api/check-questions/{id}`, and `DELETE /api/check-questions/{id}`. The `IsOrphaned` field SHALL be computed at read-time by checking whether the question key exists in a current published lesson version.

#### Scenario: Teacher lists check-questions
- **WHEN** a Teacher sends `GET /api/check-questions`
- **THEN** the response SHALL include their check-questions with orphan status computed

#### Scenario: Teacher updates a check-question
- **WHEN** a Teacher sends `PUT /api/check-questions/{id}` with new note text
- **THEN** the check-question SHALL be updated

#### Scenario: Teacher deletes a check-question
- **WHEN** a Teacher sends `DELETE /api/check-questions/{id}`
- **THEN** the check-question SHALL be removed