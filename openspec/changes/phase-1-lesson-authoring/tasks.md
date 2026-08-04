## 1. Domain Layer — Notification Entity & Enums

- [x] 1.1 Create `NotificationType` enum (NEW_THREAD_MESSAGE, ANSWER_FLAGGED, TEACHER_ASSIGNED, APPOINTMENT_CREATED, APPOINTMENT_REMINDER, SESSION_STARTED) in `Domain/Enums/`
- [x] 1.2 Create `DeliveryChannel` enum (IN_APP, EMAIL) in `Domain/Enums/`
- [x] 1.3 Create `Notification` entity (id, recipient_id, type, reference_type, reference_id, read_at, delivery_channel, created_at) in `Domain/Entities/`

## 2. Infrastructure — Notification Persistence

- [x] 2.1 Create `NotificationConfiguration` (IEntityTypeConfiguration<Notification>) — PK, FK to User, enums as string, index on recipient_id in `Infrastructure/Data/Configurations/`
- [x] 2.2 Add `DbSet<Notification>` to `AppDbContext`
- [x] 2.3 Create EF Core migration for Notification table
- [x] 2.4 Apply migration against local PostgreSQL and verify table

## 3. Application Layer — DTOs

- [x] 3.1 Create `CreateLessonRequest` DTO (number, title) in `Application/DTOs/Lessons/`
- [x] 3.2 Create `UpdateLessonRequest` DTO (number, title) in `Application/DTOs/Lessons/`
- [x] 3.3 Create `LessonDto` DTO (id, key, number, title, status, currentPublishedVersionId) in `Application/DTOs/Lessons/`
- [x] 3.4 Create `CreateLessonVersionRequest` DTO (changeNotes) in `Application/DTOs/Lessons/`
- [x] 3.5 Create `LessonVersionDto` DTO (id, lessonId, versionNumber, status, changeNotes, publishedAt, createdAt, nodes) in `Application/DTOs/Lessons/`
- [x] 3.6 Create `CreateLessonNodeRequest` DTO (parentNodeKey, title, description, requiresPriorSiblingAnswered, order) in `Application/DTOs/Lessons/`
- [x] 3.7 Create `UpdateLessonNodeRequest` DTO (title, description, requiresPriorSiblingAnswered) in `Application/DTOs/Lessons/`
- [x] 3.8 Create `ReorderNodesRequest` DTO (parentNodeKey, orderedKeys) in `Application/DTOs/Lessons/`
- [x] 3.9 Create `LessonNodeDto` DTO (id, key, lessonVersionId, parentNodeId, depth, order, title, description, requiresPriorSiblingAnswered, children, questions) in `Application/DTOs/Lessons/`
- [x] 3.10 Create `CreateQuestionRequest` DTO (questionType, promptText, metadata, referenceContext, order) in `Application/DTOs/Lessons/`
- [x] 3.11 Create `UpdateQuestionRequest` DTO (promptText, metadata, referenceContext) in `Application/DTOs/Lessons/`
- [x] 3.12 Create `ReorderQuestionsRequest` DTO (orderedKeys) in `Application/DTOs/Lessons/`
- [x] 3.13 Create `QuestionDto` DTO (id, key, lessonNodeId, order, questionType, promptText, metadata, referenceContext) in `Application/DTOs/Lessons/`

## 4. Application Layer — Lesson Use Cases

- [x] 4.1 Create `CreateLessonUseCase` — validate uniqueness, create Lesson, save, return LessonDto
- [x] 4.2 Create `ListLessonsUseCase` — return all lessons as LessonDto list
- [x] 4.3 Create `GetLessonUseCase` — find by _key, include versions, return LessonDto with version list
- [x] 4.4 Create `UpdateLessonUseCase` — find by _key, update number/title, save, return LessonDto
- [x] 4.5 Create `ArchiveLessonUseCase` — find by _key, set status to Archived, save

## 5. Application Layer — Lesson Version Use Cases

- [x] 5.1 Create `CreateDraftVersionUseCase` — find lesson by _key, find current published version with full tree (nodes→questions), deep-clone recursively preserving _keys, set as Draft, return LessonVersionDto
- [x] 5.2 Create `ListVersionsUseCase` — find lesson by _key, return all versions ordered
- [x] 5.3 Create `PublishVersionUseCase` — find version, validate at least 1 top-level node (ParentNodeId IS NULL), validate version is Draft, retire previous published version, set version to Published, update Lesson.CurrentPublishedVersionId

## 6. Application Layer — Lesson Node Use Cases

- [x] 6.1 Create `AddLessonNodeUseCase` — find parent node (by _key) or root for top-level, validate depth < 3, validate parent has no questions (leaf-only check), compute order, create node, save
- [x] 6.2 Create `UpdateLessonNodeUseCase` — find node by _key, update title/description/requiresPriorSiblingAnswered, save
- [x] 6.3 Create `DeleteLessonNodeUseCase` — find node by _key including descendants, validate version is Draft, remove node (cascade handles children/questions), save
- [x] 6.4 Create `ReorderNodesUseCase` — find parent node, update Order on each sibling matching the provided key list

## 7. Application Layer — Question Use Cases

- [x] 7.1 Create `AddQuestionUseCase` — find node by _key, validate node has no children (leaf-only check), validate version is Draft, compute order, create question, save
- [x] 7.2 Create `UpdateQuestionUseCase` — find question by _key, update promptText/metadata/referenceContext, save
- [x] 7.3 Create `DeleteQuestionUseCase` — find question by _key, validate version is Draft, remove question, save
- [x] 7.4 Create `ReorderQuestionsUseCase` — find node by _key, update Order on each question matching the provided key list

## 8. API Layer — Controllers

- [x] 8.1 Create `LessonsController` with `[RequireRole("Admin")]` at class level — POST/PUT/DELETE endpoints, and `[Authorize]` on GET by key for student read
- [x] 8.2 Wire POST `/api/lessons` → CreateLessonUseCase
- [x] 8.3 Wire GET `/api/lessons` → ListLessonsUseCase (Admin)
- [x] 8.4 Wire GET `/api/lessons/{key}` → GetLessonUseCase (Admin detail) / student-friendly response (Authorize)
- [x] 8.5 Wire PUT `/api/lessons/{key}` → UpdateLessonUseCase
- [x] 8.6 Wire DELETE `/api/lessons/{key}` → ArchiveLessonUseCase
- [x] 8.7 Create `LessonVersionsController` with `[RequireRole("Admin")]`
- [x] 8.8 Wire POST `/api/lessons/{lessonKey}/versions` → CreateDraftVersionUseCase
- [x] 8.9 Wire GET `/api/lessons/{lessonKey}/versions` → ListVersionsUseCase
- [x] 8.10 Wire POST `/api/lessons/{lessonKey}/versions/{id}/publish` → PublishVersionUseCase
- [x] 8.11 Create `LessonNodesController` with `[RequireRole("Admin")]`
- [x] 8.12 Wire POST `/api/lesson-versions/{versionId}/nodes` → AddLessonNodeUseCase
- [x] 8.13 Wire PUT `/api/lesson-versions/{versionId}/nodes/{nodeKey}` → UpdateLessonNodeUseCase
- [x] 8.14 Wire DELETE `/api/lesson-versions/{versionId}/nodes/{nodeKey}` → DeleteLessonNodeUseCase
- [x] 8.15 Wire PUT `/api/lesson-versions/{versionId}/nodes/reorder` → ReorderNodesUseCase
- [x] 8.16 Create `QuestionsController` with `[RequireRole("Admin")]`
- [x] 8.17 Wire POST `/api/lesson-nodes/{nodeKey}/questions` → AddQuestionUseCase
- [x] 8.18 Wire PUT `/api/lesson-nodes/{nodeKey}/questions/{questionKey}` → UpdateQuestionUseCase
- [x] 8.19 Wire DELETE `/api/lesson-nodes/{nodeKey}/questions/{questionKey}` → DeleteQuestionUseCase
- [x] 8.20 Wire PUT `/api/lesson-nodes/{nodeKey}/questions/reorder` → ReorderQuestionsUseCase

## 9. Seed Data

- [x] 9.1 Extend `DataSeeder` with `SeedLessonsAsync` — only runs if no lessons exist
- [x] 9.2 Seed Lesson 1 with full structure: 2 top-level nodes, 2 depth-2 nodes each, 2 questions per leaf node, all published
- [x] 9.3 Seed Lessons 2–16 as stubs: title only, one top-level node with placeholder title/description, published

## 10. Tests — Domain

- [x] 10.1 Create `NotificationTests` — validate entity creation with all fields

## 11. Tests — Application Use Cases

- [x] 11.1 Create `CreateLessonUseCaseTests` — test successful creation, duplicate number scenario
- [x] 11.2 Create `CreateDraftVersionUseCaseTests` — test clone from published, reject when no published version
- [x] 11.3 Create `PublishVersionUseCaseTests` — test successful publish, reject when no top-level nodes
- [x] 11.4 Create `AddLessonNodeUseCaseTests` — test top-level node, child at depth 2, reject at depth 3, reject on leaf with questions
- [x] 11.5 Create `AddQuestionUseCaseTests` — test add to leaf, reject on non-leaf node
- [x] 11.6 Create `DeleteLessonNodeUseCaseTests` — test cascade delete of children and questions

## 12. Tests — API Controller

- [x] 12.1 Create `LessonsControllerTests` — test each admin endpoint returns correct status codes, test student GET
- [x] 12.2 Create `LessonVersionsControllerTests` — test clone and publish endpoints
- [x] 12.3 Create `LessonNodesControllerTests` — test create/update/delete/reorder
- [x] 12.4 Create `QuestionsControllerTests` — test create/update/delete/reorder

## 13. Tests — Integration

- [x] 13.1 Create integration test: Admin creates lesson, creates draft, adds nodes, publishes — full authoring workflow
- [x] 13.2 Create integration test: Student gets published lesson, reference_context is stripped
- [x] 13.3 Create integration test: Admin deep-clone preserves _keys and creates independent copy
- [x] 13.4 Create integration test: Structural rules enforced (max depth 3 rejected, leaf-only question rejected, publish with no nodes rejected)

## 14. Polish & Validation

- [x] 14.1 Run `dotnet build` — zero errors
- [x] 14.2 Run `dotnet test` — all tests pass (including existing Phase 0 tests)
- [x] 14.3 Verify Swagger shows all new endpoints with correct route documentation
- [x] 14.4 Bump API version to `0.3.0` (minor — new endpoints + schema)
