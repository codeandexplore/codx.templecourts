# student-attempts Specification

## Purpose
TBD - created by archiving change phase-2-student-experience. Update Purpose after archive.
## Requirements
### Requirement: Student can start a lesson attempt

The system SHALL create a `LessonAttempt` when a Student begins a lesson. The attempt SHALL pin `lesson_version_id` to the published version in effect at start time. Only one attempt per student per lesson SHALL exist at a time; starting a second SHALL return the existing in-progress attempt.

#### Scenario: First attempt created
- **WHEN** a Student with no existing attempt sends `POST /api/lessons/{key}/attempts`
- **THEN** a `LessonAttempt` is created with `lesson_version_id` pinned to the current published version, `status = InProgress`, and `started_at` set to now

#### Scenario: Duplicate attempt returns existing
- **WHEN** a Student with an existing `InProgress` attempt for the same lesson sends `POST /api/lessons/{key}/attempts`
- **THEN** the system returns 200 with the existing attempt data (no new attempt created)

#### Scenario: Lesson has no published version
- **WHEN** a Student sends `POST /api/lessons/{key}/attempts` for a lesson with no published version
- **THEN** the system returns 404 Not Found

#### Scenario: Unauthenticated
- **WHEN** an unauthenticated client sends `POST /api/lessons/{key}/attempts`
- **THEN** the system returns 401 Unauthorized

### Requirement: Student can submit an answer

The system SHALL accept an answer submission for a question within an active attempt. The answer SHALL be stored as JSON (`answer_value`) in a single column. The system SHALL capture the question's prompt and type as snapshots at submission time. Submitting an answer for an already-answered question SHALL update the existing answer (upsert semantics).

#### Scenario: First answer for a question
- **WHEN** a Student submits an answer for a question they haven't answered in this attempt
- **THEN** a new `StudentAnswer` row is created with the submitted `answer_value`, `prompt_snapshot`, `question_type_snapshot`, and `submitted_at`

#### Scenario: Update existing answer
- **WHEN** a Student submits an answer for a question they already answered
- **THEN** the existing `StudentAnswer` row is updated with the new `answer_value`, and `submitted_at` is refreshed

#### Scenario: Attempt not in progress
- **WHEN** a Student submits an answer for a completed attempt
- **THEN** the system returns 400 Bad Request

#### Scenario: Question not in pinned version
- **WHEN** a Student submits an answer for a question_key not found in the pinned lesson version
- **THEN** the system returns 404 Not Found

#### Scenario: Attempt does not belong to student
- **WHEN** Student A submits an answer to Student B's attempt
- **THEN** the system returns 403 Forbidden

### Requirement: Sibling gating enforced at answer submission

When a node has `requires_prior_sibling_answered = true`, the system SHALL block answer submission for any question under that node until all questions in the immediately preceding sibling node's entire subtree have been answered in the current attempt.

#### Scenario: Gated node — prior sibling answered
- **WHEN** a Student submits an answer for a question under a gated node, and the immediately preceding sibling's subtree has all questions answered
- **THEN** the answer is accepted

#### Scenario: Gated node — prior sibling not fully answered
- **WHEN** a Student submits an answer for a question under a gated node, and the immediately preceding sibling's subtree has unanswered questions
- **THEN** the system returns 422 Unprocessable Entity with an error indicating which questions must be answered first

#### Scenario: Ungated node — no check
- **WHEN** a Student submits an answer for a question under a node with `requires_prior_sibling_answered = false`
- **THEN** no gating check is performed and the answer is accepted

#### Scenario: First node has gating enabled
- **WHEN** a node with `Order = 0` has `requires_prior_sibling_answered = true`
- **THEN** answers are accepted (no prior sibling exists to gate on)

### Requirement: Student can view attempt progress

The system SHALL return an attempt's status and the set of answered question keys for a given attempt.

#### Scenario: Get attempt with answers
- **WHEN** a Student sends `GET /api/attempts/{id}`
- **THEN** the response includes `id`, `lessonKey`, `status`, `startedAt`, and a list of answered `questionKey`s

#### Scenario: Attempt does not belong to student
- **WHEN** a Student requests another student's attempt
- **THEN** the system returns 403 Forbidden

### Requirement: Answer snapshot captures question state at submission time

The system SHALL store the question's `prompt_text` and `question_type` as snapshots (`prompt_snapshot`, `question_type_snapshot`) in the `StudentAnswer` row at the moment of first submission.

#### Scenario: Snapshot captured on first answer
- **WHEN** a Student first answers a question
- **THEN** the `StudentAnswer` row stores the current `prompt_text` and `question_type` from the question entity

#### Scenario: Snapshot not overwritten on update
- **WHEN** a Student updates an existing answer
- **THEN** the snapshot values are NOT refreshed (they capture the question at first-answer time)

