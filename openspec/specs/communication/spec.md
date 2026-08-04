# communication Specification

## Purpose
TBD - created by archiving change phase-3b-communication-layer. Update Purpose after archive.
## Requirements
### Requirement: AnswerThread is created with the first StudentAnswer

When a StudentAnswer is first created for a question, the system SHALL automatically create an AnswerThread with status OPEN, linked 1:1 to the answer.

#### Scenario: Thread auto-created
- **WHEN** a Student submits their first answer for a question
- **THEN** an AnswerThread is created with status OPEN

### Requirement: Teacher and Student can post messages in a thread

Both the Teacher (primary) and the Student SHALL be able to post a ThreadMessage in the AnswerThread. Messages SHALL be ordered chronologically.

#### Scenario: Post message
- **WHEN** a participant sends `POST /api/threads/{threadId}/messages` with `{ "bodyText": "..." }`
- **THEN** a ThreadMessage is created and returned

#### Scenario: Thread is locked
- **WHEN** a participant tries to post in a LOCKED thread
- **THEN** the system returns 400 Bad Request

### Requirement: Teacher can post a check question into a thread

A Teacher SHALL be able to reference a TeacherCheckQuestion when posting a message, via `source_check_question_id`.

#### Scenario: Post with check question reference
- **WHEN** a Teacher posts a message with a `source_check_question_id`
- **THEN** the message is created with the reference

### Requirement: Thread locks when answer is reviewed

When a StudentAnswer is marked reviewed (via live StudySession), the associated AnswerThread SHALL be locked.

#### Scenario: Thread locked on review
- **WHEN** a Teacher marks a student's answer as reviewed
- **THEN** the AnswerThread's status changes to LOCKED

### Requirement: Notification created on new thread message

When a message is posted in a thread, the system SHALL create a Notification for the other participant.

#### Scenario: Notification on teacher message
- **WHEN** a Teacher posts a message in a student's thread
- **THEN** a Notification is created for the Student

#### Scenario: Notification on student message
- **WHEN** a Student posts a message in their thread
- **THEN** a Notification is created for the Teacher

