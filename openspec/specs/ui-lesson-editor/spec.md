# UI Lesson Editor

The admin lesson authoring experience at `/admin/editor`. Covers lesson selection, version lifecycle management, recursive tree editing (nodes + questions), slide-out panel editing, sibling gating, and tree enforcement rules (max depth 3, leaf-only questions).

## Requirements

### Requirement: Admin can select a lesson and manage its versions
The UI SHALL provide a lesson selector (searchable dropdown) and version list on `/admin/editor`. The version list SHALL show each version's number, status (Draft/Published/Retired), creation date, and actions (Create Draft, Publish). Only admin users SHALL access this page.

#### Scenario: Admin selects a lesson
- **WHEN** admin navigates to `/admin/editor` and selects a lesson from the dropdown
- **THEN** the version list SHALL display all versions of that lesson with status badges

#### Scenario: Admin creates a draft version
- **WHEN** admin clicks "Create Draft" on a published version
- **THEN** the system SHALL call `POST /api/lessons/:key/versions`, create a deep-cloned draft, and refresh the version list

#### Scenario: Admin publishes a draft version
- **WHEN** admin clicks "Publish" on a draft version with at least one top-level node
- **THEN** the system SHALL call `POST /api/lessons/:key/versions/:id/publish` and update the version status

#### Scenario: Publish blocked without top-level node
- **WHEN** admin clicks "Publish" on a draft with zero top-level nodes
- **THEN** the UI SHALL display a validation error from the API

### Requirement: Admin can edit the recursive node tree
The UI SHALL render the lesson tree with depth-based indentation (ml-0 at depth 1, ml-6 at depth 2, ml-12 at depth 3) and left border guides. Each TreeNode SHALL display title, description, depth, sibling gating toggle, and action buttons (Add Child, Add Question, Delete, Move Up, Move Down). Leaf-only enforcement SHALL be visible: nodes with children SHALL NOT show "Add Question" button; nodes with questions SHALL NOT show "Add Child" button.

#### Scenario: Tree renders with correct depth indentation
- **WHEN** a lesson with nodes at depths 1, 2, and 3 is loaded
- **THEN** the tree SHALL show increasing left margin and border guides at each depth level

#### Scenario: Admin adds a child node
- **WHEN** admin clicks "Add Child" on a node at depth 1 or 2
- **THEN** a new node editor SHALL open in the slide-out panel, and on save the tree SHALL refresh showing the new child

#### Scenario: Add child blocked at max depth
- **WHEN** a node is at depth 3 (maximum)
- **THEN** the "Add Child" button SHALL be hidden or disabled

#### Scenario: Admin adds a question to a leaf node
- **WHEN** admin clicks "Add Question" on a leaf node (depth 3 or node with no children)
- **THEN** a question editor SHALL open in the slide-out panel with type selector and prompt field

#### Scenario: Admin reorders a node
- **WHEN** admin clicks "Move Up" or "Move Down" on a node
- **THEN** the node SHALL move one position in sibling order, and the tree SHALL refresh

#### Scenario: Admin deletes a node with confirmation
- **WHEN** admin clicks "Delete" on a node with children
- **THEN** a confirmation dialog SHALL warn about deleting child nodes, and on confirm the node and subtree SHALL be removed

### Requirement: Question editor supports all 5 question types
The slide-out question editor SHALL include: a type selector dropdown (Essay, Yes/No, True/False, Fill in the Blank, Multiple Choice), a prompt text input, a type-specific configuration form (JSON metadata), and a reference_context textarea (guidance/expected answer — admin-only, never shown to students). The editor SHALL support create, update, and delete operations.

#### Scenario: Admin selects a question type
- **WHEN** admin selects "Yes/No" from the type dropdown
- **THEN** the configuration form SHALL adjust to show Yes/No-specific options (if any metadata fields exist for that type)

#### Scenario: Admin saves a question
- **WHEN** admin fills in prompt, config, and reference_context, then clicks Save
- **THEN** the question SHALL be created or updated via the API, and the tree SHALL refresh

#### Scenario: Admin deletes a question
- **WHEN** admin clicks "Delete" on a question in the editor
- **THEN** a confirmation dialog SHALL appear, and on confirm the question SHALL be removed from the node

### Requirement: Sibling gating can be toggled per node
Each TreeNode SHALL display a toggle/checkbox for `requires_prior_sibling_answered`. When toggled, the UI SHALL call the node update API. A visual indicator (lock icon) SHALL appear next to gated nodes.

#### Scenario: Admin toggles sibling gating on
- **WHEN** admin checks the gating checkbox on a node
- **THEN** the node SHALL show a lock icon and the API SHALL be called to persist the change

### Requirement: Admin can view and manage teacher-student assignments
The Admin page SHALL include an Assignments tab showing all teacher-student pairs with student name, teacher name, status (Active/Ended), and dates. Admin SHALL be able to reassign a student to a different teacher via a dialog.

#### Scenario: Assignments tab loads all pairs
- **WHEN** admin navigates to the Admin page and selects the Assignments tab
- **THEN** all teacher-student assignments SHALL display with status badges

#### Scenario: Admin reassigns a student
- **WHEN** admin clicks "Reassign" on an active assignment, selects a new teacher, and confirms
- **THEN** the old assignment SHALL be ended and a new assignment SHALL be created, displayed in the list
