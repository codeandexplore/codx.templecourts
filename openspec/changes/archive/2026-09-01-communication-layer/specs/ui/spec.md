## ADDED Requirements

### Requirement: Inline thread panel on the teacher review page
The teacher review page SHALL show an inline thread panel for each answered question, displaying the conversation and a composer to post messages or post a check-question from the bank. The panel SHALL be read-only when the thread is locked.

#### Scenario: Teacher sees thread in review
- **WHEN** a teacher reviews an answered question with an open thread
- **THEN** the question panel SHALL show the thread messages and a composer

#### Scenario: Teacher posts a check-question into a thread
- **WHEN** a teacher selects a check-question from their bank and posts it into a thread
- **THEN** the message SHALL be created referencing the check-question and appear in the thread

### Requirement: Inline thread panel on the student lesson runner
The student lesson runner SHALL show an inline thread panel for each answered question, allowing the student to view and reply to messages. The panel SHALL be read-only when the thread is locked (after review).

#### Scenario: Student sees thread in runner
- **WHEN** a student views an answered question with a thread
- **THEN** the question card SHALL show the thread messages and a reply composer when open

#### Scenario: Student thread is read-only after review
- **WHEN** a student views an answered question whose thread is locked
- **THEN** the thread SHALL display messages without a composer

### Requirement: Teacher check-question bank page
The Teacher SHALL have a page listing their check-question bank with create, edit, and delete actions, plus an orphan indicator per item. The page SHALL be accessible from the teacher dashboard.

#### Scenario: Teacher views check-question bank
- **WHEN** a teacher opens the check-question bank page
- **THEN** all their check-questions SHALL list with note text and orphan status

#### Scenario: Teacher creates/edits/deletes a check-question
- **WHEN** a teacher creates, edits, or deletes a check-question on the page
- **THEN** the change SHALL be saved and the list SHALL refresh