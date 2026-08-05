## 1. Lesson Content

- [ ] 1.1 Add 5-question-type seed data (YesNo, TrueFalse, FillBlank, SelectEmbedded, Essay) to Lesson 1
- [ ] 1.2 Fill lessons 2-5 with depth-1 content (one top-level node + 2-3 questions each)
- [ ] 1.3 Fill lessons 6-10 with depth-2 content (nested nodes + questions)
- [ ] 1.4 Fill lessons 11-16 with mixed depth content (depth-1, 2, and 3 in same lesson)

## 2. Audit Log

- [ ] 2.1 Create AuditLog entity + migration
- [ ] 2.2 Wire audit entries in AssignRoleUseCase (elevation)
- [ ] 2.3 Wire audit entries in ReassignStudentUseCase (reassignment)

## 3. Security Tests

- [ ] 3.1 Test all Admin-gated endpoints return 403 for Teacher/Student roles
- [ ] 3.2 Test all Teacher-gated endpoints return 403 for Student role
- [ ] 3.3 Test reference_context is absent from every Student-role GET response
- [ ] 3.4 Test reference_context is present in Admin/Teacher GET responses

## 4. Regression Tests

- [ ] 4.1 Test: Student answers out of order, gets reassigned — history intact
- [ ] 4.2 Test: LessonVersion published mid-attempt — in-flight student unaffected
- [ ] 4.3 Test: Unresolved flag blocks next attempt; resolving unblocks
- [ ] 4.4 Test: Depth-3 lesson with mixed branches traverses and gates correctly

## 5. Polish

- [ ] 5.1 Build, test, validate
