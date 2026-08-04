## Context

Phase 1 builds on the foundation of Phase 0 (auth, roles, core schema). The `Lesson`, `LessonVersion`, `LessonNode`, and `Question` entities and their database tables already exist. What's missing is the API surface that lets an admin construct and publish a recursive lesson tree — the primary content-authoring domain of The Temple Courts.

The workspace uses PostgreSQL 16, EF Core 10, Clean Architecture (Domain → Application → Infrastructure → API), and the custom concrete-use-case pattern established in Phase 0.

## Goals / Non-Goals

**Goals:**
- Admin CRUD for Lessons, LessonVersions, LessonNodes, and Questions
- Lesson versioning: create draft by deep-cloning published version; publish with structural validation; retire old versions
- Recursive tree authoring with structural rules enforced (max depth 3, leaf-only questions, min 1 top-level node before publish)
- Sibling gating flag (`requires_prior_sibling_answered`) persisted and queryable
- Student-facing `GET /lessons/{key}` endpoint returning the published tree with `reference_context` stripped
- Notification entity + database table (schema only, no endpoints)
- Seed 16 placeholder lessons

**Non-Goals:**
- In-place patch editing of published versions (typo fixes) — deferred; Phase 1 uses only the version-bump workflow
- Notification delivery or endpoints
- Student answer submission (Phase 2)
- UI or E2E — Phase 1 is API-only
- Lesson attempt tracking (Phase 2)
- Teacher/student assignment or study sessions (Phase 3a)

## Decisions

### D1: Deep clone for draft versions

**Choice:** When creating a new draft version from a published version, deep-clone the entire node tree recursively using EF Core. Load the published version with `.Include(lv => lv.Nodes).ThenInclude(n => n.Questions)`, then iterate depth-first to create new entities preserving `_key` values but generating new `Id` values. Save all new entities in a single `SaveChangesAsync` call.

**Rationale:** EF Core's change tracker handles the cascade of new FK relationships automatically when new entities are added. Preserving `_key` ensures stable identity references survive version bumps. Generating new `Id` values ensures the new version is fully independent.

**Alternatives considered:**
- Raw SQL bulk insert: faster but bypasses domain entity construction and makes key-preservation harder to verify.
- JSON clone + deserialize: risks deserialization issues with private setters and navigation properties.
- Only shallow-copying the LessonVersion: would leave nodes and questions shared between versions — breaks the model.

### D2: Controller and endpoint structure

**Choice:** Four REST controllers under `/api/`:
- `LessonsController` — `GET /api/lessons`, `GET /api/lessons/{key}`, `POST /api/lessons`, `PUT /api/lessons/{key}`, `DELETE /api/lessons/{key}`
- `LessonVersionsController` — `GET /api/lessons/{lessonKey}/versions`, `POST /api/lessons/{lessonKey}/versions`, `POST /api/lessons/{lessonKey}/versions/{versionId}/publish`
- `LessonNodesController` — `POST /api/lesson-versions/{versionId}/nodes`, `PUT /api/lesson-versions/{versionId}/nodes/{nodeKey}`, `DELETE /api/lesson-versions/{versionId}/nodes/{nodeKey}`, `PUT /api/lesson-versions/{versionId}/nodes/reorder`
- `QuestionsController` — `POST /api/lesson-nodes/{nodeKey}/questions`, `PUT /api/lesson-nodes/{nodeKey}/questions/{questionKey}`, `DELETE /api/lesson-nodes/{nodeKey}/questions/{questionKey}`, `PUT /api/lesson-nodes/{nodeKey}/questions/reorder`

All admin endpoints use `[RequireRole("Admin")]`. The student read endpoint (`GET /api/lessons/{key}`) uses `[Authorize]`.

**Rationale:** Separate controllers by domain concept keeps each controller focused and follows REST conventions. Admin vs student distinction is at the method level on `LessonsController`.

### D3: Use case pattern (same as Phase 0)

**Choice:** Concrete use case classes (non-sealed, virtual methods for Moq), injected into controllers via `[FromServices]`. No interfaces for use cases. Each use case accesses `IAppDbContext` directly. Follows D8 from Phase 0.

**Rationale:** Consistency with the existing codebase. EF Core's `DbSet<T>` is already a repository pattern. No value in adding indirection.

### D4: Structural validation

**Choice:** Business rules enforced in the use case layer before persistence:
- **Max depth 3**: When adding a node, check `parentNode.Depth < 3`. Reject with `ConflictException` if at ceiling.
- **Leaf-only questions**: When adding a question, verify the node has no child nodes. When adding a child node, verify the node has no questions.
- **Min 1 top-level node**: Checked at publish time. Count nodes where `ParentNodeId IS NULL`. Reject with `ConflictException` if zero.

**Rationale:** Use cases are the natural boundary for domain rules. Keeping validation close to persistence avoids split-brain between validators and EF state.

### D5: Publish workflow

**Choice:** Three-step workflow:
1. Admin creates draft via clone (D1)
2. Admin edits nodes/questions freely on the draft
3. Admin publishes: validate structure → set draft to `Published` → retire previous published version → update `Lesson.CurrentPublishedVersionId`

No in-place editing of published versions in Phase 1.

**Rationale:** The architecture doc specifies two update paths: in-place patch and version-bump. Phase 1 implements only version-bump. The patch workflow is simpler and can be added later without architectural change.

### D6: Student read endpoint

**Choice:** `GET /api/lessons/{key}` loads the lesson's current published version with all nodes and questions, shaped into a DTO tree. `reference_context` is automatically stripped by the `RoleAwareJsonTypeResolver` added in Phase 0 — no code change needed.

**Rationale:** The serialization layer already enforces the constraint. The read endpoint just returns the entity graph and the resolver handles the rest.

### D7: Notification entity design

**Choice:** Simple entity with polymorphic reference (type + id), no FK constraints. Fields: `Id`, `RecipientId`, `Type` (enum), `ReferenceType`, `ReferenceId`, `ReadAt`, `DeliveryChannel` (enum), `CreatedAt`. No navigation properties to referenced entities.

**Rationale:** Notifications reference many different entity types. Loose coupling via polymorphic pointers avoids a tangle of FKs and migrations every time a new notification type is added. Delivery wiring comes in Phase 4.

### D8: Seed data strategy

**Choice:** Extend the existing `DataSeeder` with `SeedLessonsAsync`. Create 16 lessons with Lesson 1 being fully structured (2 sections, 2 questions per section) and Lessons 2–16 being title-only stubs with a single top-level node. All versions published. Admin-only safe — runs only if no lessons exist.

**Rationale:** A fully-structured Lesson 1 gives the student read endpoint something concrete to return. Stubs for 2–16 prove the data model works at scale without spending effort on placeholder content.

## Risks / Trade-offs

- **Deep clone performance**: Loading the full published tree (nodes + questions) into memory and creating new entities could be slow for very large lessons. Mitigated by the 3-level depth cap and EF Core's `AsSplitQuery()` — total entities per lesson is bounded.
- **Concurrent version creation**: Two admins could both create drafts from the same published version simultaneously. Phase 1 does not implement optimistic concurrency. Acceptable for single-admin or low-concurrency scenarios.
- **No soft delete for nodes/questions**: Deletion is hard cascade (via EF cascade). If a node is accidentally removed, recovery requires the previous version's clone. Acceptable for MVP with small admin team.
- **Notification table unused until Phase 4**: The migration creates the table but no code references it. Safe — EF Core doesn't query unused tables.

## Open Questions

<!-- None — all design decisions are resolved -->
