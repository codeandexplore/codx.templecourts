## Why

The API has no Teacher role activation or student↔teacher pairing mechanism. Without TeacherAssignment, the StudySession (Phase 3a-ii) has no concept of "which Teacher runs this session." This change establishes the 1:1 Teacher↔Student link as a history-preserving table, plus Admin endpoints for role elevation and assignment management — a prerequisite for the live review workflow.

## What Changes

- Add `TeacherAssignment` entity as a history table (ACTIVE/ENDED), never overwritten in place.
- Admin can elevate a user to Teacher by assigning the Teacher role.
- Teacher can claim an unassigned Student.
- Admin can reassign a Student from one Teacher to another (ends the old row, creates a new one).
- Admin can list all assignments; Teacher can list their own students.

## Capabilities

### New Capabilities
- `teacher-assignment`: TeacherAssignment history entity, Admin elevation, Teacher claim, Admin reassign, listing.

### Modified Capabilities
<!-- None. Existing specs unaffected. -->

## Impact

- **Domain**: New `TeacherAssignment` entity + `TeacherAssignmentStatus` enum (Active, Ended).
- **Application**: New use cases `AssignRoleUseCase` modified to handle Teacher role with assignment context; new `ClaimStudentUseCase`, `ReassignStudentUseCase`, `ListTeacherAssignmentsUseCase`, `GetTeacherStudentsUseCase`.
- **Infrastructure**: `IAppDbContext` extended with `DbSet<TeacherAssignment>`. `AppDbContext` updated. New EF migration.
- **API**: New `TeacherAssignmentsController` with Admin/Teacher-scoped endpoints.
- **No breaking changes** to existing endpoints.
