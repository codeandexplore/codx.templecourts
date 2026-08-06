## 1. Setup — API Services & Routes

- [x] 1.1 Create `src/services/versionsApi.ts` with RTK Query endpoints: getVersions, createDraft, publishVersion
- [x] 1.2 Create `src/services/nodesApi.ts` with RTK Query endpoints: createNode, updateNode, deleteNode, reorderNodes
- [x] 1.3 Create `src/services/questionsApi.ts` with RTK Query endpoints: createQuestion, updateQuestion, deleteQuestion, reorderQuestions
- [x] 1.4 Expand `src/services/adminApi.ts` with getAssignments (list), reassignStudent (POST /admin/assignments/reassign)
- [x] 1.5 Expand `src/services/lessonsApi.ts` with updateLesson (PUT), archiveLesson (DELETE)
- [x] 1.6 Add `/admin/editor` and `/admin/assignments` routes to `src/router.tsx` under RequireRole("Admin")
- [x] 1.7 Add "Lesson Editor" and "Assignments" links to sidebar in `src/layouts/AppLayout.tsx`

## 2. Shared Components

- [x] 2.1 Create `src/components/ConfirmDialog.tsx` — shadcn Dialog wrapper for destructive action confirmations
- [x] 2.2 Create `src/components/SlideOutPanel.tsx` — shadcn Sheet wrapper for right-side editing panel
- [x] 2.3 Create `src/components/LessonSelector.tsx` — searchable dropdown to select a lesson for editing (uses useListLessonsQuery)
- [x] 2.4 Generate shadcn Sheet component: `npx shadcn@latest add sheet`

## 3. Lesson Editor Page — Shell

- [x] 3.1 Create `src/pages/EditorPage.tsx` — main page with lesson selector, version manager, and tree editor layout
- [x] 3.2 Create `src/pages/EditorPage.tsx` — VersionManager section: version list with status badges, Create Draft and Publish buttons
- [x] 3.3 Create `src/pages/EditorPage.tsx` — Back to Admin breadcrumb and serif page heading

## 4. Tree Editor Components

- [x] 4.1 Create `src/components/TreeEditor.tsx` — recursive container that fetches version tree and renders TreeNode children
- [x] 4.2 Create `src/components/TreeNode.tsx` — displays title, description, depth indent, border guides, gating toggle, action buttons
- [x] 4.3 Implement Add Child button logic: hidden at depth 3 or when node already has questions
- [x] 4.4 Implement Add Question button logic: hidden when node already has children (leaf-only enforcement)
- [x] 4.5 Implement Delete button with ConfirmDialog for nodes having children
- [x] 4.6 Implement Move Up / Move Down reorder buttons calling nodesApi.reorderNodes

## 5. Question Editor (Slide-Out Panel)

- [x] 5.1 Create `src/components/QuestionEditor.tsx` — SlideOutPanel with type selector, prompt, config, and reference_context fields
- [x] 5.2 Implement type selector with 5 options: Essay, YesNo, TrueFalse, FillBlank, SelectEmbedded
- [x] 5.3 Add prompt text input (shadcn Input/Textarea)
- [x] 5.4 Add reference_context textarea (admin-only, marked as guidance for teachers)
- [x] 5.5 Wire Save (create/update) and Delete to questionsApi mutations via RTK Query
- [x] 5.6 Integrate QuestionEditor into TreeNode "Add Question" click handler

## 6. Node Editor (Slide-Out Panel)

- [x] 6.1 Implement node editing via SlideOutPanel: title, description, gating checkbox
- [x] 6.2 Wire save to nodesApi.updateNode, delete to nodesApi.deleteNode
- [x] 6.3 Integrate into TreeNode click handler (click node opens editor)

## 7. Admin Assignments Tab

- [x] 7.1 Create `src/components/admin/AssignmentsTab.tsx` with assignment list using shadcn Card + Badge
- [x] 7.2 Implement Reassign dialog: teacher selector dropdown, confirm/cancel, calls adminApi.reassignStudent
- [x] 7.3 Add Assignments tab to `src/pages/AdminPage.tsx` alongside existing Lessons and Roles tabs

## 8. Verification and Polish

- [x] 8.1 Run `pnpm type-check` and fix any type errors
- [x] 8.2 Run `pnpm lint` and fix any lint issues
- [x] 8.3 Run `pnpm build` and verify production build succeeds
- [ ] 8.4 Manually verify: select lesson → create draft → add node → add child node → add question → publish
- [ ] 8.5 Manually verify: lesson selector shows all lessons, version list shows correct statuses, tree renders depth correctly
- [ ] 8.6 Manually verify: sibling gating toggle persists, reorder works, delete with children shows confirmation
- [ ] 8.7 Manually verify: Assignments tab shows all pairs, reassign creates new assignment and ends old one
- [x] 8.8 Run `pnpm test` (Vitest) and verify existing tests pass
