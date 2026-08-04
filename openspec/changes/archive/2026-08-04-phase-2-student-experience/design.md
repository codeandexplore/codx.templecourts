## Context

Phase 1 delivered lesson authoring (Admin CRUD on recursive tree, versioning, question types). Phase 2 adds the student-facing side: lesson runner, answer submission, and question notes. The existing codebase follows Clean Architecture with concrete `UseCase` classes (no MediatR), EF Core with `IAppDbContext` as sole data access, and FluentValidation for input validation.

No tenant concept exists yet — all operations are tenant-agnostic for now. No StudySession, AnswerFlag, or thread entities exist; those are Phase 3.

## Goals / Non-Goals

**Goals:**
- Student can list published lessons and read their full tree structure
- Student can start a lesson attempt (one per student per lesson), pinned to the version at start time
- Student can submit and update answers (JSON `answer_value`, one column for all 5 question types)
- Sibling gating enforced at submit-time: node's questions locked until immediately preceding sibling's entire question subtree is answered
- `reference_context` stripped from all question payloads when caller holds Student role
- Student can create/update/delete per-question notes (keyed on stable `question_key`)

**Non-Goals:**
- No grading, no correct/incorrect — zero grading logic anywhere
- No StudySession, AnswerFlag, AnswerThread, TeacherCheckQuestion — Phase 3/3b
- No `reviewed`/`reviewed_by` fields on answers — teacher review is Phase 3a
- No tenant scoping (data model has no tenant entity)
- No SignalR/real-time — POST-based only
- No UI / E2E — API-only phase

## Decisions

### Answer storage: single JSON column

**Decision:** Store all 5 question types in a single `answer_value` JSONB column.

**Rationale:** Per architecture doc §5.1. Adding a new question type later requires zero schema changes. Validation is per-type at the application layer.

### One attempt per student per lesson

**Decision:** Enforce exactly one `LessonAttempt` per (student, lesson) pair. Starting a second attempt returns the existing one.

**Rationale:** "Multiple lesson attempts per student" is explicitly out of MVP scope. When multi-attempt is needed later, simply relax the constraint.

### Sibling gating: immediately preceding sibling only

**Decision:** When a node has `requires_prior_sibling_answered = true`, check only the sibling with `Order = current.Order - 1` (not all prior siblings).

**Rationale:** Per architecture doc §3 wording: "immediately preceding sibling (same parent_node_id, order - 1)". Only one predecessor, not cumulative.

**Algorithm:**
```
Given: question_key, lesson_version_id, lesson_attempt_id
1. Load the question's node
2. If node.RequiresPriorSiblingAnswered == false → allow
3. If true:
   a. Find sibling with same ParentNodeId and Order = node.Order - 1
   b. Collect ALL question_keys in prior sibling's subtree (recursive, max depth 3)
   c. Query StudentAnswers for this attempt with keys in that set
   d. If count of answered keys < count of required keys → reject
```

### reference_context stripping: controller/post-processing

**Decision:** Strip `reference_context` in the controller/mapping layer, not the use case.

**Rationale:** The use case stays role-agnostic (receives `includeReferenceContext: bool` flag). The controller reads the `application_role` claim and passes the flag. This keeps use cases testable without auth plumbing and follows the architecture doc's "stripped at the response-serialization layer" guidance.

### Entity identifiers: stable keys

**Decision:** `StudentAnswer` references `question_key` (stable Guid) rather than `question_id` (version-specific). `StudentQuestionNote` keyed on `question_key`.

**Rationale:** Per architecture doc §5.9: stable identity survives version bumps. Notes and answers survive into new lesson versions without migration. The `prompt_snapshot` and `question_type_snapshot` fields capture the question's state at answer time for audit trail.

### No repository interfaces

**Decision:** Follow existing pattern — use `IAppDbContext` directly with EF Core LINQ queries in use cases.

**Rationale:** The codebase has no repository layer. Adding one now for new entities would create an inconsistent pattern. Query directly against `DbSet<T>` properties.

### Controller organization

**Decision:** Add endpoints to the existing `LessonsController` (list, get tree) and create a new `StudentAnswersController` (attempt, answers) and `StudentNotesController` (notes).

**Rationale:** Lesson listing and reading fit the existing controller's noun. Attempts/answers/notes are distinct enough for separate controllers. Avoids a monolithic controller.

## Risks / Trade-offs

- **DTO size for deep trees**: A lesson with 3 levels and many nodes could produce a large JSON response. Mitigation: max depth is 3 (enforced at authoring), so worst-case is bounded. Future: consider pagination or lazy-loading children.
- **Sibling gating query complexity**: Requires recursive subtree traversal in EF Core. Mitigation: depth is capped at 3, so a single Include chain loads the entire tree in 1-2 queries.
- **Answer upsert semantics**: POST creates or updates an answer (idempotent by attempt+question_key). Mitigation: documented in API contract; no silent overwrite without the user's intent.
- **No session context for answers**: Answers in Phase 2 have no StudySession linkage. Phase 3a adds `StudySession` linkage and `reviewed` flag — answers from Phase 2 will have `reviewed = false` and no session_id, which is valid (lesson completed, answers un-reviewed).

## Open Questions

- None at proposal time. All decisions are resolvable from existing architecture doc and codebase patterns.
