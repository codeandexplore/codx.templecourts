## ADDED Requirements

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
