## Context

The backend exposes 15 admin-only endpoints for lesson authoring across 4 controllers: LessonsController (update/archive), LessonVersionsController (list/draft/publish), LessonNodesController (CRUD/reorder), and QuestionsController (CRUD/reorder). The UI currently only uses `POST /api/lessons` and `GET /api/lessons` — basic create and list. The design system (`ui-design-system` change) provides shadcn/ui components (Button, Card, Input, Badge, Tabs, Dialog) and design tokens (parchment/cerulean palette, serif headings, generous spacing).

The architecture doc flags this explicitly: "Admin authoring UX for deep trees — a 3-level recursive tree editor is a heavier UI lift than a fixed two-level form; worth a dedicated design pass in Phase 1."

## Goals / Non-Goals

**Goals:**
- Full recursive tree editor page at `/admin/editor` for Admin role
- Version lifecycle: select lesson → view versions → create draft (deep clone) → edit → publish
- Tree editing: add/delete/reorder nodes at any valid depth, with leaf-only and depth-3 enforcement
- Question editing: type selector, prompt editor, per-type config, reference_context field
- Sibling gating toggle per node
- Admin Assignments tab: view all teacher-student pairs, reassign students
- Slide-out panel pattern for question/node editing (keeps tree visible)

**Non-Goals:**
- Drag-and-drop reordering (use up/down buttons in MVP; dnd-kit deferred)
- Inline editing on the tree itself (use slide-out panel for focused editing)
- Real-time collaborative editing (single admin)
- Question preview/rendering in the editor (edit only; preview is on the lesson detail page)
- Version diff/comparison view
- Bulk operations (delete all children, clone branch)

## Decisions

### D1: Separate `/admin/editor` page rather than expanding the AdminPage tabs

**Choice**: New route `/admin/editor` with its own page component, not another tab on `/admin`.

**Rationale**: The tree editor is a full-page, immersive tool with its own internal navigation (lesson selector, version list, tree, question panel). Cramming it into a tab alongside lessons/roles/assignments would create a cramped nested-tabs UX. A dedicated route gives it full viewport width, a clean back-link to `/admin`, and room for the slide-out question panel.

**Alternative considered**: Tabs on `/admin` — rejected because the editor needs full horizontal space for the recursive tree indentation and slide-out panel.

### D2: Slide-out panel (shadcn Sheet) for question/node editing

**Choice**: When the admin clicks a node or question, a right-side Sheet slides in with the editor form. The tree remains visible underneath with the clicked item highlighted.

**Rationale**: Modals block the tree context; inline expansion clutters the tree layout. A slide-out panel keeps the tree in view for spatial orientation while providing a focused editing surface. The Sheet can show the current item's position in the tree (breadcrumb: "Lesson 1 > The Beginning > Question 3").

**Alternative considered**: Inline expansion (click to reveal editor below the node) — rejected because it pushes sibling nodes out of view, breaking spatial orientation in deep trees.

### D3: Up/down reorder buttons over drag-and-drop

**Choice**: Each TreeNode has ⬆️⬇️ buttons for reordering, rather than drag handles.

**Rationale**: Drag-and-drop libraries add bundle weight (~8KB gzipped for dnd-kit) and complexity (collision detection, scroll containers, touch support). For a tree editor where individual nodes are reordered occasionally by a single admin, up/down buttons are simpler, accessible, and provide clear predictable behavior. Drag-and-drop can be added later as an enhancement.

**Alternative considered**: dnd-kit sortable tree — rejected for MVP scope; preserved as a future enhancement.

### D4: Deep clone on "Create Draft"

**Choice**: When the admin clicks "Create Draft" on a published version, the UI calls `POST /api/lessons/:key/versions` which the backend handles as a deep clone. The UI then refreshes the version list and auto-selects the new draft.

**Rationale**: The cloning logic is server-side (ensures all `_key` fields are preserved, nested children cloned correctly, version_number incremented). The UI just triggers it and reflects the result. No client-side tree cloning needed.

### D5: Optimistic local state for tree mutations

**Choice**: After successful API calls for node/question CRUD, invalidate the RTK Query cache for the version tree and refetch, rather than maintaining optimistic local state.

**Rationale**: The tree structure is complex (nested nodes, depth enforcement, sibling ordering, question placement). Optimistic updates risk desync with the actual server state especially on validation failures. Cache invalidation + refetch is simpler and correct, at the cost of a brief loading flash.

**Alternative considered**: Manual cache manipulation — rejected for complexity; the tree refetch is a single API call and fast enough on localhost.

### D6: Route structure — nested under RequireRole

**Choice**: `/admin/editor` and admin routes live under `<RequireRole role="Admin">` in the router, same pattern as existing `/admin`.

**Rationale**: Consistent with existing role-guard pattern. No new middleware needed.

## Risks / Trade-offs

- **[Risk] Tree refetch after every mutation could feel sluggish on slow connections** → Mitigation: The version tree endpoint is lightweight (single SQL query with Include for children). For MVP with 16 lessons, this is negligible. Add optimistic updates later if needed.
- **[Risk] Slide-out panel might feel cramped on smaller screens** → Mitigation: The Sheet uses a responsive width (default w-[400px], max w-[500px]). Admin users are on desktop. Can add responsive breakpoints later.
- **[Trade-off] No drag-and-drop reordering** → Up/down buttons require multiple clicks for large reorders. Acceptable trade-off for MVP; dnd-kit can be added as a follow-up enhancement.
- **[Risk] Deep tree (3 levels) might be hard to scan at a glance** → Mitigation: Use clear depth-based indentation (ml-0, ml-6, ml-12) with left border guides and alternating node backgrounds to visually separate tree levels.
