## ADDED Requirements

### Requirement: Student can leave notes on questions in the lesson runner
The lesson runner (Attempt page) SHALL provide a note affordance on each question card. The student SHALL be able to create, view, edit, and delete a per-question note. Notes SHALL be stored per question_key and persist across lesson versions.

#### Scenario: Add a note to a question
- **WHEN** a student opens the note editor on a question and saves text
- **THEN** a note SHALL be created for that question and displayed inline

#### Scenario: Edit an existing note
- **WHEN** a student opens the note editor on a question that already has a note
- **THEN** the existing note text SHALL be shown and editable, and saving SHALL update it

#### Scenario: Delete a note
- **WHEN** a student deletes a note on a question
- **THEN** the note SHALL be removed

### Requirement: Student can finish a lesson with a completion summary
The lesson runner SHALL provide a "Finish Lesson" button when the attempt is in progress. On confirmation, the attempt SHALL be marked complete and a summary SHALL display showing the number of questions answered out of the total.

#### Scenario: Finish lesson shows summary
- **WHEN** a student clicks "Finish Lesson" and confirms
- **THEN** the attempt SHALL be marked complete and a summary SHALL display "X of Y answered"

#### Scenario: Completed lesson is read-only
- **WHEN** a student views a completed attempt
- **THEN** the questions and answers SHALL render read-only with no editing controls

### Requirement: Student can view their submitted answer when editing
When a student edits a previously answered question, the input SHALL be pre-filled with their submitted answer text.

#### Scenario: Edit answer pre-filled
- **WHEN** a student clicks "Edit answer" on an answered question
- **THEN** the input SHALL be pre-filled with the previously submitted answer

### Requirement: Student sees lesson completion status and attempt history
The lessons list SHALL show a completion/in-progress indicator per lesson. The lesson detail page SHALL show a "Completed" badge and the list of past attempts (status, dates, answered count) when the lesson has prior attempts.

#### Scenario: Lessons list shows completion status
- **WHEN** a student views the lessons list after completing a lesson
- **THEN** that lesson's card SHALL display a "Completed" indicator

#### Scenario: Lesson detail shows attempt history
- **WHEN** a student opens a lesson they have attempted before
- **THEN** the page SHALL display the list of past attempts with status, dates, and answered count
