# Teacher Assignment

## Purpose

1:1 Teacher↔Student pairing as a history-preserving table. Admin elevates users to Teacher, Teachers claim unassigned Students, and Admin can reassign. This is the prerequisite for live StudySession (Phase 3a-ii).

## ADDED Requirements

### Requirement: Admin can elevate a user to Teacher

The system SHALL allow an Admin to assign the Teacher role to any user. The assignment SHALL create a RoleAssignment row with role `Teacher`. A user SHALL be able to hold multiple roles (e.g., Admin + Teacher).

#### Scenario: Admin elevates user to Teacher
- **WHEN** an Admin sends `POST /admin/role-assignments` with `{ "userId": "<guid>", "role": "Teacher" }`
- **THEN** a RoleAssignment is created and the user's `application_role` claim becomes `Teacher` (or `Owner`/`Admin` if they hold a higher role)

#### Scenario: Non-Admin cannot elevate
- **WHEN** a Teacher or Student sends `POST /admin/role-assignments`
- **THEN** the system returns 403 Forbidden

### Requirement: Teacher can claim an unassigned Student

The system SHALL allow a Teacher to claim a Student who has no ACTIVE TeacherAssignment. Claiming SHALL create a new ACTIVE TeacherAssignment row.

#### Scenario: Teacher claims unassigned student
- **WHEN** a Teacher sends `POST /api/students/{userId}/claim` for a student with no ACTIVE assignment
- **THEN** a TeacherAssignment is created with `status = Active`, `primary_teacher_id` set to the calling Teacher

#### Scenario: Student already assigned
- **WHEN** a Teacher sends `POST /api/students/{userId}/claim` for a student with an existing ACTIVE assignment
- **THEN** the system returns 409 Conflict

#### Scenario: Non-Teacher cannot claim
- **WHEN** a non-Teacher sends `POST /api/students/{userId}/claim`
- **THEN** the system returns 403 Forbidden

### Requirement: Admin can reassign a Student to a different Teacher

The system SHALL allow an Admin to reassign a Student from one Teacher to another. The old assignment SHALL be ended (status = Ended) and a new ACTIVE row SHALL be created.

#### Scenario: Admin reassigns student
- **WHEN** an Admin sends `POST /api/admin/assignments/reassign` with `{ "studentId": "...", "newTeacherId": "..." }`
- **THEN** the existing ACTIVE assignment is ended (status = Ended, ended_at = now), and a new ACTIVE assignment is created pointing to the new Teacher

#### Scenario: Student has no ACTIVE assignment
- **WHEN** an Admin tries to reassign a student with no ACTIVE assignment
- **THEN** the system returns 404 Not Found (nothing to reassign from)

#### Scenario: New Teacher is the same as current
- **WHEN** an Admin tries to reassign to the same Teacher
- **THEN** the system returns 400 Bad Request

### Requirement: Assignments are history-preserving

The system SHALL NEVER delete or overwrite TeacherAssignment rows. Reassignment SHALL create a new row and end the old one. The full assignment history SHALL be queryable.

#### Scenario: Reassignment creates new row
- **WHEN** an Admin reassigns a student from Teacher A to Teacher B
- **THEN** both the old (Ended) and new (Active) rows exist in the database

#### Scenario: Querying history
- **WHEN** an Admin queries assignments for a student
- **THEN** all historical assignment rows are returned

### Requirement: Admin can list all assignments

The system SHALL allow an Admin to list all TeacherAssignments, optionally filtered by status.

#### Scenario: List all ACTIVE assignments
- **WHEN** an Admin sends `GET /api/admin/assignments?status=Active`
- **THEN** the response contains all ACTIVE assignments with student and teacher details

### Requirement: Teacher can list their own students

The system SHALL allow a Teacher to list only their own ACTIVE students.

#### Scenario: Teacher lists students
- **WHEN** a Teacher sends `GET /api/teacher/students`
- **THEN** the response contains only the calling Teacher's ACTIVE student assignments
