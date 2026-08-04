# Student Notes

## Purpose

Per-question personal margin notes for Students, keyed on the stable `question_key` identifier so notes survive lesson version bumps.

## ADDED Requirements

### Requirement: Student can create a note on a question

The system SHALL allow a Student to create a personal note on any question identified by its stable `question_key`. Notes SHALL be private to the creating student.

#### Scenario: Create note
- **WHEN** a Student sends `POST /api/notes/{questionKey}` with a request body containing `noteText`
- **THEN** a `StudentQuestionNote` is created scoped to the student and `question_key`, with `created_at` set to now

#### Scenario: Note text is required
- **WHEN** a Student sends `POST /api/notes/{questionKey}` with an empty `noteText`
- **THEN** the system returns 422 Unprocessable Entity

#### Scenario: Unauthenticated
- **WHEN** an unauthenticated client sends `POST /api/notes/{questionKey}`
- **THEN** the system returns 401 Unauthorized

### Requirement: Student can read their own notes

The system SHALL return the Student's notes, optionally filtered by question key.

#### Scenario: Get specific note
- **WHEN** a Student sends `GET /api/notes/{questionKey}`
- **THEN** the system returns the note for that student and question, or 404 if none exists

#### Scenario: List all notes
- **WHEN** a Student sends `GET /api/notes`
- **THEN** the system returns all notes for the authenticated student

### Requirement: Student can update a note

The system SHALL allow a Student to update the note text on an existing note. Updating a non-existent note SHALL NOT auto-create it.

#### Scenario: Update existing note
- **WHEN** a Student sends `PUT /api/notes/{questionKey}` with new `noteText`
- **THEN** the existing note's text is updated

#### Scenario: Update non-existent note
- **WHEN** a Student sends `PUT /api/notes/{questionKey}` for a question they have no note on
- **THEN** the system returns 404 Not Found

### Requirement: Student can delete a note

The system SHALL allow a Student to delete their own note. Deleting a non-existent note SHALL be idempotent (return 204 either way).

#### Scenario: Delete existing note
- **WHEN** a Student sends `DELETE /api/notes/{questionKey}` for a note they own
- **THEN** the note is removed and 204 No Content is returned

#### Scenario: Delete non-existent note
- **WHEN** a Student sends `DELETE /api/notes/{questionKey}` for a question with no note
- **THEN** the system returns 204 No Content (no error)
