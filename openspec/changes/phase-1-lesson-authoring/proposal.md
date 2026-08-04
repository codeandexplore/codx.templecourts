## Why

Phase 0 established authentication, roles, and the core database schema — but lessons are empty shells. No admin can build a lesson, no student can read one. Phase 1 delivers the authoring API that lets admins construct the recursive lesson tree (nodes + questions up to depth 3), manage versioning (draft → clone → publish → retire), and exposes the first student-facing read endpoint. The Notification table lands as schema-only here to unblock later communication phases.

## What Changes

- **Lesson CRUD**: Admin-only endpoints to create, read, update, and archive lessons
- **Lesson versioning**: Create draft versions by deep-cloning published trees; publish with structural validation; retire old versions
- **Recursive tree authoring**: Add/remove/reorder LessonNodes (up to depth 3) and Questions (leaf-only) within a draft version
- **5 question types**: YesNo, TrueFalse, FillBlank, SelectEmbedded, Essay — each with type-specific metadata
- **Sibling gating**: `requires_prior_sibling_answered` flag on nodes, persisted and returned by the API
- **Publish validation**: Block publish if no top-level nodes exist in the version
- **Student read endpoint**: `GET /lessons/{key}` returns the published lesson tree with `reference_context` stripped for Student role (serialization layer already enforces this from Phase 0)
- **Notification schema**: Entity + migration for the `Notification` table (no endpoints or delivery wiring)
- **Seed data**: 16 placeholder lessons with titles and basic structure

## Capabilities

### New Capabilities
- `lesson-authoring`: Admin CRUD for lessons, lesson versions, lesson nodes, and questions — the full recursive tree authoring API with versioning and publish workflow
- `lesson-reading`: Student-facing GET endpoint that returns a published lesson's full tree structure, with reference_context already stripped for the Student role
- `notification-schema`: Notification entity and database migration (table only, no endpoints or delivery logic)

### Modified Capabilities
<!-- No existing specs to modify — all capabilities are new -->

## Impact

- **API project** (`codx.temple-api`): New controllers (Lessons, LessonVersions, LessonNodes, Questions), ~15 use cases, DTOs for request/response shapes, validators for structural rules (max depth, leaf-only, min 1 node before publish)
- **Domain layer**: New `Notification` entity; no changes to existing entities
- **Infrastructure layer**: Notification EF configuration, new migration for Notification table, additional `AppDbContext` DbSet
- **Database**: One new table (`Notifications`), no schema changes to existing tables
- **Seed data**: 16 placeholder lessons inserted by the existing `DataSeeder`
- **Tests**: Domain tests for Notification, unit tests for all new controllers and use cases, integration tests for the authoring workflow and student read endpoint
- **No UI or E2E impact**: Phase 1 is API-only (student read endpoint is consumed by Phase 2 UI)
