# study-session Specification

## Purpose
TBD - created by archiving change phase-3a-live-review. Update Purpose after archive.
## Requirements
### Requirement: Teacher can create a StudySession for a LessonAttempt

The system SHALL allow a Teacher to start a StudySession against an active LessonAttempt. The session SHALL default to the tree-traversal-ordered first unanswered question. If a prior session exists for this attempt, the new session SHALL default start_question_id to the next question after the prior session's end_question_id.

#### Scenario: First session for attempt
- **WHEN** a Teacher starts a session for an attempt with no prior sessions
- **THEN** a StudySession is created with start_question_id set to the first question in tree-traversal order

#### Scenario: Multi-session resumption
- **WHEN** a Teacher starts a new session for an attempt that had a prior session ending at question Q5
- **THEN** the new session's start_question_id defaults to Q6 (next in tree-traversal order)

#### Scenario: Teacher does not own the student
- **WHEN** a Teacher tries to start a session for a student not assigned to them
- **THEN** the system returns 403 Forbidden

### Requirement: Teacher can advance the session to a specific question

The system SHALL allow the Teacher to set the session's current_question_id to any question in the lesson's tree traversal sequence.

#### Scenario: Advance to next question
- **WHEN** a Teacher sets current_question_id to the next unreviewed question
- **THEN** the session's current_question_id is updated and broadcast via SignalR

### Requirement: Teacher can end a StudySession

The system SHALL allow the Teacher to end a session. Ending SHALL set the session status to Completed and record ended_at. A completed session SHALL NOT be modified.

#### Scenario: End session
- **WHEN** a Teacher ends an IN_PROGRESS session
- **THEN** the session transitions to Completed with ended_at set

### Requirement: Teacher can mark an answer as reviewed during a live session

The system SHALL allow the Teacher to toggle a StudentAnswer's `reviewed` field to `true`, only while the associated StudySession is IN_PROGRESS. The `reviewed_in_session_id` SHALL be set to the current session ID. The Teacher identity SHALL be recorded in `reviewed_by`.

#### Scenario: Mark reviewed during session
- **WHEN** a Teacher marks a student's answer as reviewed during an IN_PROGRESS session
- **THEN** the answer's reviewed flag is set to true with the session and teacher IDs recorded

#### Scenario: Block reviewed when session not IN_PROGRESS
- **WHEN** a Teacher attempts to mark an answer reviewed outside an IN_PROGRESS session
- **THEN** the system returns 400 Bad Request

#### Scenario: Block reviewed when answer is empty
- **WHEN** a Teacher attempts to mark an unanswered question as reviewed
- **THEN** the system raises an AnswerFlag of type UNANSWERED_AT_REVIEW and returns 422 Unprocessable Entity

### Requirement: AnswerFlag is auto-raised when marking unanswered as reviewed

When a Teacher attempts to mark a StudentAnswer reviewed but the answer_value is empty, the system SHALL automatically create an AnswerFlag of type UNANSWERED_AT_REVIEW linked to the session.

#### Scenario: Flag raised on empty answer
- **WHEN** a Teacher marks an unanswered question as reviewed
- **THEN** an AnswerFlag is created and the reviewed operation is rejected

### Requirement: AnswerFlag auto-resolves when student submits answer

When a Student submits an answer for a question that has an unresolved AnswerFlag, the system SHALL automatically set the flag's resolved_at to now.

#### Scenario: Flag resolved on answer submit
- **WHEN** a Student submits an answer for a question with an unresolved AnswerFlag
- **THEN** the flag's resolved_at is set to now

### Requirement: Unresolved AnswerFlags block new LessonAttempts

When a Student attempts to start a new LessonAttempt (for any lesson), the system SHALL check for any unresolved AnswerFlags (resolved_at IS NULL). If any exist, the attempt creation SHALL be blocked.

#### Scenario: Block new attempt when flag unresolved
- **WHEN** a Student with an unresolved AnswerFlag tries to start a new LessonAttempt
- **THEN** the system returns 422 Unprocessable Entity

#### Scenario: Allow new attempt when flag resolved
- **WHEN** a Student whose only AnswerFlag has been resolved tries to start a new LessonAttempt
- **THEN** the attempt is created successfully

### Requirement: Session state is broadcast to connected participants via SignalR

The system SHALL broadcast session state changes (start, advance, end, mark reviewed, flag raised) to all participants connected to the session's SignalR group. Student connections SHALL receive a subset of state (no reference_context).

#### Scenario: Session state broadcast on advance
- **WHEN** a Teacher advances the session to a new question
- **THEN** both Teacher and Student clients receive the updated session state in real time

