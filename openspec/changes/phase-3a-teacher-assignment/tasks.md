## 1. Domain Entity & Enum

- [x] 1.1 Create `TeacherAssignmentStatus` enum (`Active`, `Ended`) in `Domain/Enums/`
- [x] 1.2 Create `TeacherAssignment` entity: `Id`, `StudentId`, `PrimaryTeacherId`, `AssignedById`, `Status`, `AssignedAt`, `EndedAt` + nav props

## 2. Domain Tests

- [x] 2.1 Test `TeacherAssignment` factory creates valid entity
- [x] 2.2 Test `End()` transitions Active → Ended and sets EndedAt
- [x] 2.3 Test `End()` throws when already Ended

## 3. Application — Abstractions + Config

- [x] 3.1 Add `DbSet<TeacherAssignment>` to `IAppDbContext`
- [x] 3.2 Create `TeacherAssignmentConfiguration` with snake_case columns, FK to User (student/teacher/assigner), index on `(StudentId)` where Status = Active
- [x] 3.3 Add DbSet to `AppDbContext`, apply config
- [x] 3.4 Create EF migration

## 4. Application — DTOs

- [x] 4.1 Add `ReassignStudentRequest` and `TeacherAssignmentDto` to `DTOs/Admin/AdminDtos.cs`

## 5. Application — Use Cases

- [x] 5.1 Implement `ClaimStudentUseCase` — Teacher claims unassigned student, creates ACTIVE assignment
- [x] 5.2 Implement `ReassignStudentUseCase` — Admin ends old assignment, creates new one
- [x] 5.3 Implement `ListTeacherAssignmentsUseCase` — Admin lists all assignments, optional status filter
- [x] 5.4 Implement `GetTeacherStudentsUseCase` — Teacher lists their ACTIVE students

## 6. API — Controllers

- [x] 6.1 Create `TeacherAssignmentsController` with endpoints: `POST /api/students/{userId}/claim`, `GET /api/teacher/students`
- [x] 6.2 Add `POST /api/admin/assignments/reassign` and `GET /api/admin/assignments` to AdminController or new controller

## 7. Application Tests

- [x] 7.1 Test `ClaimStudentUseCase` — creates assignment for unassigned student
- [x] 7.2 Test `ClaimStudentUseCase` — 409 when student already assigned
- [x] 7.3 Test `ReassignStudentUseCase` — ends old, creates new
- [x] 7.4 Test `ReassignStudentUseCase` — 404 when no ACTIVE assignment
- [x] 7.5 Test `ListTeacherAssignmentsUseCase` — returns filtered results
- [x] 7.6 Test `GetTeacherStudentsUseCase` — returns own students only

## 8. API Tests

- [x] 8.1 Test claim endpoint returns 200 as Teacher
- [x] 8.2 Test claim endpoint returns 403 as Student
- [x] 8.3 Test reassign endpoint returns 200 as Admin
- [x] 8.4 Test list assignments endpoint returns 200 as Admin

## 9. Integration Tests

- [x] 9.1 End-to-end: Admin elevates user to Teacher, Teacher claims student, Admin reassigns

## 10. Polish

- [x] 10.1 Run `dotnet build` — zero errors
- [x] 10.2 Run `dotnet test` — all tests pass
- [x] 10.3 Validate: `openspec validate phase-3a-teacher-assignment --type change`
