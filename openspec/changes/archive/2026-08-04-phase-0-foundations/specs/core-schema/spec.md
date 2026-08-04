## ADDED Requirements

### Requirement: User entity stores identity and authentication data

The system SHALL have a `User` entity with columns: `id` (Guid, PK), `email` (unique, required), `password_hash` (nullable — null for Google-only accounts), `google_id` (nullable — null for password-only accounts), `display_name` (required), `refresh_token_hash` (nullable), `refresh_token_expires_at` (nullable), `status` (enum: ACTIVE, INACTIVE), `created_at`.

#### Scenario: User created with email and password
- **WHEN** a user registers with email and password
- **THEN** the `password_hash` is set to a BCrypt hash, `google_id` is null, `status` is ACTIVE

#### Scenario: User created with Google OAuth
- **WHEN** a user registers via Google OAuth
- **THEN** the `google_id` is set to the Google account ID, `password_hash` is null, `status` is ACTIVE

### Requirement: RoleAssignment entity maps users to roles

The system SHALL have a `RoleAssignment` entity with columns: `id` (Guid, PK), `user_id` (FK → User), `role` (enum: ADMIN, TEACHER, STUDENT), `assigned_by` (Guid, FK → User), `assigned_at` (DateTimeOffset). A composite unique constraint SHALL prevent duplicate `(user_id, role)` pairs.

#### Scenario: Role assignment created
- **WHEN** an admin assigns TEACHER role to a user
- **THEN** a RoleAssignment record is persisted with `assigned_by` set to the admin's user ID and `assigned_at` set to the current timestamp

#### Scenario: Duplicate role assignment rejected
- **WHEN** an admin attempts to assign a role that the user already has
- **THEN** the database rejects the insert with a unique constraint violation

### Requirement: Lesson entity represents a lesson with versioning support

The system SHALL have a `Lesson` entity with columns: `id` (Guid, PK), `_key` (Guid, stable identity surviving version bumps), `number` (integer), `title` (required), `current_published_version_id` (nullable FK → LessonVersion), `status` (enum: ACTIVE, ARCHIVED).

#### Scenario: Lesson created as skeleton
- **WHEN** an admin creates a new lesson with a number and title
- **THEN** the Lesson is persisted with a generated `_key`, `current_published_version_id` null, and status ACTIVE

### Requirement: LessonVersion entity captures a specific version of a lesson

The system SHALL have a `LessonVersion` entity with columns: `id` (Guid, PK), `lesson_id` (FK → Lesson), `version_number` (integer, sequential per lesson), `status` (enum: DRAFT, PUBLISHED, RETIRED), `cloned_from_version_id` (nullable FK → LessonVersion), `change_notes`, `published_at` (nullable), `created_at`.

#### Scenario: First draft version created
- **WHEN** a LessonVersion is created for a lesson as its first version
- **THEN** the `version_number` is 1, `status` is DRAFT, `cloned_from_version_id` is null

### Requirement: LessonNode entity forms the recursive lesson structure tree

The system SHALL have a `LessonNode` entity with columns: `id` (Guid, PK), `_key` (Guid, stable identity), `lesson_version_id` (FK → LessonVersion), `parent_node_id` (nullable FK → LessonNode, self-referencing — null for top-level nodes), `depth` (integer, 1–3 enforced), `order` (integer, sibling ordering), `title` (required), `description` (required, free text including verse references), `requires_prior_sibling_answered` (boolean, default false).

#### Scenario: Top-level node created
- **WHEN** a LessonNode is created with `parent_node_id` null under a LessonVersion
- **THEN** the node is persisted with `depth` = 1 and `order` set to the next available sibling position

#### Scenario: Nested node created
- **WHEN** a LessonNode is created with `parent_node_id` pointing to a depth-1 node
- **THEN** the node is persisted with `depth` = 2

#### Scenario: Depth limit enforced
- **WHEN** a LessonNode is created with `parent_node_id` pointing to a depth-3 node
- **THEN** the system rejects the operation — maximum depth is 3

#### Scenario: Sibling gating configured
- **WHEN** a LessonNode is created with `requires_prior_sibling_answered` set to true
- **THEN** the flag is persisted and will be enforced when students answer questions in that node's subtree

### Requirement: Question entity attaches to leaf LessonNodes

The system SHALL have a `Question` entity with columns: `id` (Guid, PK), `_key` (Guid, stable identity), `lesson_node_id` (FK → LessonNode, required — Question always belongs to a leaf LessonNode), `order` (integer, ordering within the node), `question_type` (enum: YES_NO, TRUE_FALSE, FILL_BLANK, SELECT_EMBEDDED, ESSAY), `prompt_text` (required), `metadata` (JSON, type-specific config), `reference_context` (JSON, nullable — guidance/expected answer for Admin/Teacher only).

#### Scenario: Question created on a leaf node
- **WHEN** a Question is created with `lesson_node_id` pointing to a LessonNode that has no children
- **THEN** the Question is persisted with its `_key` generated and all fields set

#### Scenario: Question blocked on non-leaf node
- **WHEN** a Question is created with `lesson_node_id` pointing to a LessonNode that has existing child nodes
- **THEN** the system rejects the operation — questions can only attach to leaf nodes

### Requirement: Core entities are mapped via EF Core configuration

The system SHALL use `IEntityTypeConfiguration<T>` classes in the Infrastructure layer for all six entities (`User`, `RoleAssignment`, `Lesson`, `LessonVersion`, `LessonNode`, `Question`). The `AppDbContext` SHALL have `DbSet<T>` properties for all six entities. The first EF Core migration SHALL create all six tables with correct constraints, indexes, and relationships.

#### Scenario: Migration creates all tables
- **WHEN** the first EF Core migration is applied
- **THEN** the database contains `users`, `role_assignments`, `lessons`, `lesson_versions`, `lesson_nodes`, and `questions` tables with all columns, foreign keys, and unique constraints

#### Scenario: Migration is reversible
- **WHEN** the first EF Core migration is rolled back
- **THEN** all six tables are dropped cleanly without errors
