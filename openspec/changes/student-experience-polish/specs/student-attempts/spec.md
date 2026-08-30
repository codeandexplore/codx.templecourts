## ADDED Requirements

### Requirement: Student can complete a lesson attempt
The system SHALL provide `POST /api/attempts/{attemptId}/complete` to mark a lesson attempt complete. Completing an already-completed attempt SHALL be idempotent (return the attempt unchanged). The attempt SHALL belong to the current student.

#### Scenario: Complete an in-progress attempt
- **WHEN** a Student sends `POST /api/attempts/{id}/complete` for their in-progress attempt
- **THEN** the attempt status SHALL change to Completed and the response SHALL return the updated attempt

#### Scenario: Complete an already-completed attempt
- **WHEN** a Student sends `POST /api/attempts/{id}/complete` for an already-completed attempt
- **THEN** the system SHALL return the attempt unchanged without error

#### Scenario: Complete another student's attempt
- **WHEN** a Student sends `POST /api/attempts/{id}/complete` for another student's attempt
- **THEN** the system SHALL return 403 Forbidden

### Requirement: Student can list their submitted answers
The system SHALL provide `GET /api/attempts/{attemptId}/answers` returning all submitted answers for the attempt, each with the question key, answer value, prompt snapshot, question type snapshot, and submission time. The attempt SHALL belong to the current student.

#### Scenario: List answers for own attempt
- **WHEN** a Student sends `GET /api/attempts/{id}/answers` for their attempt
- **THEN** the response SHALL include every submitted answer with `questionKey`, `answerValue`, `promptSnapshot`, `questionTypeSnapshot`, and `submittedAt`

#### Scenario: List answers for another student's attempt
- **WHEN** a Student sends `GET /api/attempts/{id}/answers` for another student's attempt
- **THEN** the system SHALL return 403 Forbidden

### Requirement: Student can list their attempts
The system SHALL provide `GET /api/attempts` returning all of the current student's lesson attempts, each with the lesson number and title, status, started/completed times, and answered count. Attempts SHALL be ordered most-recent-first.

#### Scenario: List my attempts
- **WHEN** a Student sends `GET /api/attempts`
- **THEN** the response SHALL include every attempt with lesson number/title, status, timestamps, and answered count, newest first

