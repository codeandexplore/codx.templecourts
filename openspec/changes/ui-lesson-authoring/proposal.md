## Why

The backend supports full lesson authoring — recursive tree editing up to depth 3, version management (draft → publish), question CRUD across 5 types, sibling gating, node reordering, and teacher assignment oversight. None of this has any UI surface. The admin can create bare lessons (number + title) but can't add nodes, questions, versions, or publish. This blocks all future content creation and makes the existing 16 seed lessons uneditable from the UI.

## What Changes

- New page `/admin/editor` — full recursive tree editor for lesson authoring
- Version management: list versions, create draft (deep-clone from published), publish with validation
- Recursive tree editor: add/delete/reorder nodes at any depth (max 3), sibling gating toggle, depth-first visual hierarchy
- Question editor: type selector (5 types), prompt editor, per-type config, reference_context authoring
- Admin Assignments tab: view all teacher-student pairs, reassign students
- Sidebar additions: "Lesson Editor" and "Assignments" links under Admin section
- New RTK Query API services: versionsApi, nodesApi, questionsApi
- Expand existing adminApi with assignment list and reassignment mutations

## Capabilities

### New Capabilities
- `ui-lesson-editor`: Full admin UI for lesson authoring — version lifecycle (draft/publish), recursive tree editor with depth-3 enforcement, leaf-only question placement, 5-type question editor with reference_context, sibling gating toggle, node reordering, and orphaned node handling.

### Modified Capabilities
- `ui`: Add `/admin/editor` page for lesson authoring and `/admin` Assignments tab for teacher-student oversight. Sidebar gains "Lesson Editor" and "Assignments" links (Admin-only).

## Impact

- **New pages**: `src/pages/EditorPage.tsx` (lesson editor), `src/pages/admin/AssignmentsTab.tsx` (new tab)
- **New components**: TreeEditor, TreeNode, QuestionEditor, VersionManager, LessonSelector, ConfirmDialog, SlideOutPanel
- **New services**: `src/services/versionsApi.ts`, `src/services/nodesApi.ts`, `src/services/questionsApi.ts`
- **Modified files**: `src/pages/AdminPage.tsx` (add Assignments tab), `src/services/adminApi.ts` (add assignments + reassign), `src/services/lessonsApi.ts` (add update/archive), `src/layouts/AppLayout.tsx` (add sidebar links), `src/router.tsx` (add /admin/editor, /admin/assignments routes)
- **New dependency**: `@dnd-kit/core` + `@dnd-kit/sortable` for drag-to-reorder (or manual up/down buttons)
- **No backend changes required** — all 46 endpoints are already built and deployed
