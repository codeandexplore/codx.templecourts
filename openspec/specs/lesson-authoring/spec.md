# Lesson Authoring

## Purpose

Authoring of dynamic, recursive lesson trees: create/edit lessons, publish to new versions, and capture student attempts within a live `StudySession`. This is the teacher-side authoring surface of the product.
## Requirements
### Requirement: Teacher can author lessons as a dynamic recursive tree

A lesson SHALL be authored as a tree with a max depth of 3 levels. Only leaf nodes SHALL carry questions. A lesson SHALL have at least one top-level node before it can be published. Structure is traversed depth-first when the lesson is read or seeded for a student.

#### Scenario: Closed branch can hold child nodes
- **WHEN** a node is created at depth 1 or 2
- **THEN** it can hold child nodes down to a maximum depth of 3, and only nodes at depth 3 (leaves) may hold questions

#### Scenario: Publish requires at least one top-level node
- **WHEN** a lesson with zero top-level nodes is submitted for publish
- **THEN** the system rejects the publish with a validation error

#### Scenario: Read returns the tree
- **WHEN** an author reads a lesson
- **THEN** the response contains the nested node tree, traversed depth-first

### Requirement: Lesson edits produce draft versions, not destructive mutations

Editing an existing lesson SHALL NOT mutate its current version. Instead, edits SHALL create, or reuse, an in-progress draft version whose latest data is reflected. Publish SHALL finalize the draft into a new published version. A lesson's stable identity SHALL be `_key` (persists across versions); `id` SHALL be version-specific.

#### Scenario: Edits target a draft version
- **WHEN** an author edits a published lesson
- **THEN** the edits are recorded against a draft version, leaving the published version unchanged until publish

#### Scenario: Stable identity across versions
- **WHEN** a lesson is published as a new version
- **THEN** the `_key` remains constant while `id` changes to identify the specific version

#### Scenario: Attempts pin the version
- **WHEN** a student starts a lesson attempt
- **THEN** the attempt captures the `lesson_version_id` of the version in effect at start time

### Requirement: Orphaned bank nodes are flagged, never deleted

When an edit removes a node from the tree, that node is SHALL not be hard-deleted. Instead, it SHALL be flagged as orphaned. Reads SHALL filter out or flag orphaned nodes at read-time.

#### Scenario: Removed node becomes orphaned
- **WHEN** an author removes a node from the tree
- **THEN** the node is marked orphaned and preserved, and subsequent reads exclude it from the active tree

### Requirement: Teacher can capture student attempts in a live StudySession

A live `StudySession` SHALL allow a teacher to review a student's attempt in real time. Attempts are captured within the live session only; there is SHALL be no asynchronous attempt grading.

#### Scenario: Live review capture
- **WHEN** a teacher reviews a student's answer during a live `StudySession`
- **THEN** the answer is captured against the session in real time

#### Scenario: No asynchronous attempt grade
- **WHEN** a student submits an answer outside a live `StudySession`
- **THEN** no grade or review action is recorded for that answer

### Requirement: Student input never shows expected answers (`reference_context` strips for Student role)

The `reference_context` (expected-answer / guidance) SHALL NEVER be served to the Student role. The system SHALL strip `reference_context` at the response-serialization layer when the requester is a Student.

#### Scenario: Student request omits reference context
- **WHEN** a Student requests a question payload
- **THEN** the response omits `reference_context`

#### Scenario: Author request includes reference context
- **WHEN** a Teacher or Owner requests a question payload
- **THEN** the response includes `reference_context`

### Requirement: Admin can create a lesson

The system SHALL allow an Admin to create a new lesson with a number and title. The lesson SHALL be created with `Active` status and no published version. A stable `_key` SHALL be generated on creation.

#### Scenario: Successful lesson creation
- **WHEN** an Admin sends a POST request to `/api/lessons` with `number` and `title`
- **THEN** the system creates a new Lesson with `Active` status and returns the lesson with its `_key`

#### Scenario: Non-admin blocked
- **WHEN** a Teacher or Student sends a POST request to `/api/lessons`
- **THEN** the system returns 403 Forbidden

### Requirement: Admin can list all lessons

The system SHALL allow an Admin to retrieve a list of all lessons, including their status and current published version ID.

#### Scenario: List all lessons
- **WHEN** an Admin sends a GET request to `/api/lessons`
- **THEN** the system returns a list of all lessons with `_key`, `number`, `title`, `status`, and `currentPublishedVersionId`

### Requirement: Admin can get a lesson by key

The system SHALL allow an Admin to retrieve a single lesson by its stable `_key`, including all versions.

#### Scenario: Get existing lesson
- **WHEN** an Admin sends a GET request to `/api/lessons/{key}`
- **THEN** the system returns the lesson with its list of versions

#### Scenario: Lesson not found
- **WHEN** an Admin sends a GET request to `/api/lessons/{key}` for a non-existent key
- **THEN** the system returns 404 Not Found

### Requirement: Admin can update a lesson

The system SHALL allow an Admin to update a lesson's `number` and `title`.

#### Scenario: Successful lesson update
- **WHEN** an Admin sends a PUT request to `/api/lessons/{key}` with updated `number` and `title`
- **THEN** the system updates the lesson and returns the updated entity

### Requirement: Admin can archive a lesson

The system SHALL allow an Admin to archive a lesson, setting its status to `Archived`.

#### Scenario: Archive active lesson
- **WHEN** an Admin sends a DELETE or status-change request for an active lesson
- **THEN** the system sets the lesson status to `Archived` and returns the updated lesson

### Requirement: Admin can create a draft version by cloning a published version

The system SHALL allow an Admin to create a new draft version by deep-cloning the current published version of a lesson. All nodes and questions SHALL be recursively copied, preserving `_key` values and generating new `Id` values. The new version SHALL start in `Draft` status.

#### Scenario: Clone published version
- **WHEN** an Admin sends a POST request to `/api/lessons/{lessonKey}/versions` to create a draft from the published version
- **THEN** the system deep-clones the entire tree (nodes + questions), creates a new Draft version with incremented version number, and returns the new version

#### Scenario: Clone with no published version
- **WHEN** an Admin sends a POST request to create a draft for a lesson with no published version
- **THEN** the system returns 400 Bad Request

### Requirement: Admin can publish a draft version

The system SHALL allow an Admin to publish a draft version. The publish operation SHALL validate that at least one top-level node exists. The previously published version SHALL be retired. The lesson's `currentPublishedVersionId` SHALL be updated.

#### Scenario: Successful publish
- **WHEN** an Admin sends a POST request to `/api/lessons/{lessonKey}/versions/{versionId}/publish` for a Draft version with at least one top-level node
- **THEN** the system sets the version to `Published`, retires the previously published version, updates the lesson's current version pointer, and returns the published version

#### Scenario: Publish fails — no top-level nodes
- **WHEN** an Admin attempts to publish a Draft version with zero top-level nodes
- **THEN** the system returns 409 Conflict with a message indicating at least one top-level node is required

### Requirement: Admin can list versions of a lesson

The system SHALL allow an Admin to list all versions of a lesson.

#### Scenario: List versions
- **WHEN** an Admin sends a GET request to `/api/lessons/{lessonKey}/versions`
- **THEN** the system returns all versions for that lesson, ordered by creation date

### Requirement: Admin can add a node to a draft version

The system SHALL allow an Admin to add a LessonNode to a draft version. The node SHALL be attached to a parent node (or null for top-level). The system SHALL enforce a maximum depth of 3. The system SHALL reject adding a child node to a node that already has questions (leaf-only constraint).

#### Scenario: Add top-level node
- **WHEN** an Admin sends a POST request to `/api/lesson-versions/{versionId}/nodes` with `parentNodeKey: null`, `title`, and `description`
- **THEN** the system creates a top-level node with `depth: 1` and returns it

#### Scenario: Add child node at depth 1
- **WHEN** an Admin creates a node with `parentNodeKey` pointing to a depth-1 node
- **THEN** the system creates a node with `depth: 2`

#### Scenario: Reject at max depth
- **WHEN** an Admin attempts to create a child node under a depth-3 node
- **THEN** the system returns 409 Conflict

#### Scenario: Reject child on leaf node with questions
- **WHEN** an Admin attempts to add a child node to a node that already has questions
- **THEN** the system returns 409 Conflict

### Requirement: Admin can update a node

The system SHALL allow an Admin to update a node's `title`, `description`, and `requires_prior_sibling_answered` flag.

#### Scenario: Update node fields
- **WHEN** an Admin sends a PUT request to `/api/lesson-versions/{versionId}/nodes/{nodeKey}` with updated fields
- **THEN** the system updates the node and returns it

### Requirement: Admin can delete a node

The system SHALL allow an Admin to delete a node from a draft version. Deleting a node SHALL cascade to delete all child nodes and their questions.

#### Scenario: Delete node with children
- **WHEN** an Admin sends a DELETE request for a node that has child nodes
- **THEN** the system deletes the node and all descendants (cascading via EF Core)

### Requirement: Admin can reorder nodes

The system SHALL allow an Admin to reorder sibling nodes by providing an ordered list of node keys for a given parent.

#### Scenario: Reorder siblings
- **WHEN** an Admin sends a PUT request to `/api/lesson-versions/{versionId}/nodes/reorder` with a parent key and ordered node key list
- **THEN** the system updates the `order` field of each sibling to match the provided sequence

### Requirement: Admin can add a question to a leaf node

The system SHALL allow an Admin to add a question to a node. The system SHALL reject adding a question to a node that has child nodes (leaf-only constraint).

#### Scenario: Add question to leaf node
- **WHEN** an Admin sends a POST request to `/api/lesson-nodes/{nodeKey}/questions` with `questionType`, `promptText`, and optional `metadata` and `referenceContext`
- **THEN** the system creates a question attached to the node and returns it

#### Scenario: Reject question on non-leaf node
- **WHEN** an Admin attempts to add a question to a node that has child nodes
- **THEN** the system returns 409 Conflict

### Requirement: Admin can update a question

The system SHALL allow an Admin to update a question's `promptText`, `metadata`, and `referenceContext`.

#### Scenario: Update question
- **WHEN** an Admin sends a PUT request to `/api/lesson-nodes/{nodeKey}/questions/{questionKey}` with updated fields
- **THEN** the system updates the question and returns it

### Requirement: Admin can delete a question

The system SHALL allow an Admin to delete a question from a draft version.

#### Scenario: Delete question
- **WHEN** an Admin sends a DELETE request for a question
- **THEN** the system removes the question

### Requirement: Admin can reorder questions

The system SHALL allow an Admin to reorder questions on a node by providing an ordered list of question keys.

#### Scenario: Reorder questions
- **WHEN** an Admin sends a PUT request to `/api/lesson-nodes/{nodeKey}/questions/reorder` with an ordered list of question keys
- **THEN** the system updates the `order` field of each question to match the provided sequence

