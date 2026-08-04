# Lesson Reading

## Purpose

Student-side consumption of published lesson versions: list lessons, start a lesson attempt against a pinned version, and record answers without any auto-grading.
## Requirements
### Requirement: Student can list published lessons for a tenant

The system SHALL expose a list of published lessons scoped to the student's Tenant. The list SHALL show only the latest published version of each lesson.

#### Scenario: List shows latest published versions
- **WHEN** a Student requests the lesson list for their Tenant
- **THEN** the response contains the latest published version of each lesson in that Tenant

#### Scenario: Drafts are excluded
- **WHEN** a lesson has an unpublished draft version
- **THEN** the list still returns the latest published version and does not surface the draft

### Requirement: Student starts a lesson attempt pinned to a version

The system SHALL create a lesson attempt when a Student begins a lesson. The attempt SHALL pin `lesson_version_id` to the published version the student starts from. Later version updates SHALL NOT change the attempt's pinned version.

#### Scenario: Attempt pins current published version
- **WHEN** a Student starts a lesson attempt
- **THEN** a `LessonAttempt` is created with `lesson_version_id` set to the version in effect

#### Scenario: Version publishes after start do not affect open attempt
- **WHEN** a new version is published after a Student started an attempt
- **THEN** the open attempt continues against its originally pinned `lesson_version_id`

### Requirement: Student answers are recorded, never graded

The system SHALL record a Student's answer for a question. The system SHALL NOT compute a correct/incorrect result, and SHALL expose no grade or review state on the attempt until a live teacher review action occurs.

#### Scenario: Answer recorded
- **WHEN** a Student submits an answer during a live `StudySession`
- **THEN** the answer is stored against the question in the attempt, with no grade assigned

#### Scenario: No auto-grade
- **WHEN** an answer is recorded
- **THEN** no correct/incorrect determination is made by the system

### Requirement: Student can read a published lesson by key

The system SHALL allow an authenticated Student to retrieve a published lesson by its stable `_key`. The response SHALL include the full tree structure (version → nodes → questions) from the current published version. The `reference_context` field on questions SHALL be stripped from the response for Student-role users.

#### Scenario: Retrieve published lesson as Student
- **WHEN** a Student sends a GET request to `/api/lessons/{key}` for a lesson with a published version
- **THEN** the system returns the published version with all top-level nodes, child nodes, and questions, with `reference_context` excluded from all question objects

#### Scenario: Teacher can also read lessons
- **WHEN** a Teacher sends a GET request to `/api/lessons/{key}` for a lesson with a published version
- **THEN** the system returns the published version with all nodes and questions, with `reference_context` INCLUDED in question objects

#### Scenario: Lesson has no published version
- **WHEN** a Student or Teacher sends a GET request to `/api/lessons/{key}` for a lesson with no published version
- **THEN** the system returns 404 Not Found

#### Scenario: Unauthenticated request
- **WHEN** an unauthenticated client sends a GET request to `/api/lessons/{key}`
- **THEN** the system returns 401 Unauthorized

