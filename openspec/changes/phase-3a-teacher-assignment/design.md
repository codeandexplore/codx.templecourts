## Context

Phase 2 shipped student experience (LessonAttempt, StudentAnswer, StudentQuestionNote). Phase 3a splits into two sequential changes: teacher assignment (this change) and live review (follow-up). Without TeacherAssignment, StudySession has no teacher context.

The existing RoleAssignment table links users to roles. A user getting the Teacher role from Admin doesn't yet imply any student pairing — that's what TeacherAssignment adds.

## Goals / Non-Goals

**Goals:**
- Admin elevates any user to Teacher role
- Teacher claims an unassigned Student, creating the 1:1 ACTIVE assignment
- Admin reassigns a Student to a different Teacher (history-preserving)
- Admin lists all assignments; Teacher lists their own students
- Only ACTIVE assignments gate study sessions (Phase 3a-ii)

**Non-Goals:**
- Co-teacher, group/class studies (MVP is strictly 1:1)
- Student-initiated teacher selection
- Notification triggers for assignment events (Phase 4)
- StudySession / AnswerFlag / reviewed toggle (Phase 3a-ii)

## Decisions

### TeacherAssignment as history table

**Decision:** `TeacherAssignment` is a history table with STATUS (ACTIVE/ENDED). Reassigning creates a new row; never mutates the old row in place. Each student can have at most one ACTIVE assignment at a time.

**Rationale:** Per architecture doc §5.10: "TeacherAssignment is a history table, never a plain foreign key on Student." Preserves who taught whom and when.

### Claim: Teacher self-service

**Decision:** `POST /api/students/{studentId}/claim` — Teacher claims any student with no ACTIVE assignment. Teacher owns their own pairings without requiring Admin intervention.

**Rationale:** Per the roles table: "Teacher can claim unassigned student only." Reduces Admin bottleneck.

### Reassign: Admin-only

**Decision:** `POST /api/admin/assignments/reassign` — Admin ends current ACTIVE assignment and creates a new one with a different Teacher.

**Rationale:** Student can't self-reassign; Teacher can't poach another Teacher's student. Admin is the gatekeeper for transfers.

### Entity: uses stable UserId (Guid)

**Decision:** `TeacherAssignment` references `StudentId` (FK to User) and `PrimaryTeacherId` (FK to User), plus `AssignedById` (FK to User — the Admin who created it).

**Rationale:** No separate "Student" or "Teacher" entity exists — both are Users with RoleAssignments. The FK chain is User → RoleAssignment → Role.

### No separate DTO folder for admin/teacher

**Decision:** Add DTOs to existing `DTOs/Admin/AdminDtos.cs` file.

**Rationale:** The admin DTOs file already exists and handles role management. TeacherAssignment is an admin-scoped concern.

## Risks / Trade-offs

- **Circular reassignment**: A Teacher reassigned away from their only student loses access to that student's study sessions. Mitigation: existing sessions remain, but the Teacher can't start new ones.
- **Claim race**: Two Teachers could attempt to claim the same unassigned student. Mitigation: unique constraint on ACTIVE assignment per student; second claim wins the race with a 409 Conflict.
