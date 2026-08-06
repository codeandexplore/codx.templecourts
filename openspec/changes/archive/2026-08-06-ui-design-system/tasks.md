## 1. Setup and Dependencies

- [x] 1.1 Install shadcn/ui with `npx shadcn@latest init` (Tailwind v4, TypeScript, src/ base, CSS variables: no, color: neutral)
- [x] 1.2 Verify `components.json` created and `src/lib/utils.ts` generated with `cn()` helper
- [x] 1.3 Install additional dependencies: `pnpm add @heroicons/react` (verify already installed), ensure `lucide-react` added by shadcn
- [x] 1.4 Run `pnpm type-check` and verify no import conflicts between existing and new dependencies

## 2. Design Tokens

- [x] 2.1 Define Tailwind v4 `@theme` block in `src/index.css` with parchment palette (`--color-parchment-*`), cerulean palette (`--color-cerulean-*`), gold palette (`--color-gold-*`), and serif font family
- [x] 2.2 Add custom spacing scale (`--spacing-page: 2rem`), border radii (`--radius-xl: 0.75rem`), and shadow tokens
- [x] 2.3 Set up dark mode token overrides so `dark:` variants use design tokens (e.g., `dark:bg-slate-950`, `dark:text-parchment-100`)
- [x] 2.4 Run `pnpm dev` and verify custom utility classes (e.g., `bg-parchment`, `text-cerulean-500`, `font-serif`) resolve in browser

## 3. shadcn/ui Components

- [x] 3.1 Generate Button component: `npx shadcn@latest add button` â€” customize with cerulean primary, olive secondary, ghost variants
- [x] 3.2 Generate Card component: `npx shadcn@latest add card` â€” customize with parchment bg, soft border, padding tokens
- [x] 3.3 Generate Input + Textarea components: `npx shadcn@latest add input textarea` â€” customize focus rings to cerulean
- [x] 3.4 Generate Badge component: `npx shadcn@latest add badge` â€” add variants: default, success (emerald), warning (gold), destructive
- [x] 3.5 Generate Tabs component: `npx shadcn@latest add tabs` â€” customize active tab styling to cerulean
- [x] 3.6 Generate Dialog component: `npx shadcn@latest add dialog` â€” configure as persistent modal (no click-outside-to-close)

## 4. Layout Redesign

- [x] 4.1 Redesign `src/layouts/AppLayout.tsx` sidebar: serif "The Temple Courts" heading, divider, Heroicon nav links
- [x] 4.2 Add `BookOpenIcon`, `AcademicCapIcon`, `UserGroupIcon`, `Cog6ToothIcon` from @heroicons/react to sidebar nav
- [x] 4.3 Add user area at sidebar bottom with display name and ghost "Sign Out" button (keep existing sign-out logic)
- [x] 4.4 Update main content area to use `p-8` and parchment background

## 5. Page Redesign â€” Auth Pages

- [x] 5.1 Redesign `src/pages/LoginPage.tsx`: serif heading, shadcn Inputs, shadcn Button (primary), centered card with generous padding
- [x] 5.2 Redesign `src/pages/RegisterPage.tsx`: same pattern as Login, add displayName field, consistent error display
- [x] 5.3 Add `EnvelopeIcon`, `LockClosedIcon`, `UserIcon` (heroicons) as leading icons in auth form inputs

## 6. Page Redesign â€” Lesson Pages

- [x] 6.1 Redesign `src/pages/LessonsPage.tsx`: responsive grid of shadcn Cards, each with lesson number, title, Badge status, hover feedback
- [x] 6.2 Add `BookOpenIcon` and Badge (Published/Draft with icon) to each lesson card
- [x] 6.3 Redesign `src/pages/LessonDetailPage.tsx`: lesson tree nodes with depth indentation, `LockClosedIcon` for gated nodes, question type labels
- [x] 6.4 Replace "Start Lesson" / "Continue Lesson" buttons with shadcn Button (primary)
- [x] 6.5 Add `QuestionMarkCircleIcon` or type-appropriate icons for each question type

## 7. Page Redesign â€” Attempt Runner

- [x] 7.1 Redesign `src/pages/AttemptPage.tsx`: question cards with shadcn Card, shadcn Textarea, shadcn Button (submit)
- [x] 7.2 Add progress counter styled with Badge (N/M answered)
- [x] 7.3 Add answered/unanswered visual distinction: emerald left border + `CheckBadgeIcon` for answered
- [x] 7.4 Style the "Edit" link as a ghost button with `PencilIcon`

## 8. Page Redesign â€” Teacher Dashboard

- [x] 8.1 Redesign `src/pages/TeacherPage.tsx`: student list as shadcn Cards with `AcademicCapIcon` per student
- [x] 8.2 Style empty state (no students) with parchment Card and guiding text

## 9. Page Redesign â€” Admin Dashboard

- [x] 9.1 Redesign `src/pages/AdminPage.tsx` tabs using shadcn Tabs component (replace current inline tab buttons)
- [x] 9.2 Update Lessons tab: table/list of lessons with Badge status, "Create Lesson" button opens shadcn Dialog
- [x] 9.3 Create Lesson dialog with shadcn Input fields (number, title) and submit/cancel Buttons
- [x] 9.4 Update Roles tab: role assignment form with shadcn Input (user ID), select dropdown, and submit Button

## 10. Component Library â€” Custom Wrappers

- [x] 10.1 Create `src/components/LessonCard.tsx` composing shadcn Card + Badge with interactive variant
- [x] 10.2 Create `src/components/QuestionCard.tsx` composing shadcn Card + Textarea + Button with answered variant
- [x] 10.3 Create `src/components/StudentCard.tsx` composing shadcn Card with icon and info layout
- [x] 10.4 Update `src/components/NodeRenderer.tsx` to use shadcn Card, spacing tokens, and gating icons
- [x] 10.5 Update `src/components/ProtectedRoute.tsx` and `RequireRole.tsx` to use shadcn styling (no functional change)

## 11. Verification and Polish

- [x] 11.1 Run `pnpm type-check` and fix any type errors
- [x] 11.2 Run `pnpm lint` and fix any lint issues
- [x] 11.3 Run `pnpm dev` and manually verify all 8 pages render correctly in light and dark mode
- [x] 11.4 Run `pnpm test` (Vitest) and verify all existing tests pass
- [x] 11.5 Run E2E tests (`pnpm test` in e2e project) and verify auth and lesson flows still work
- [x] 11.6 Verify no visual regressions on: login flow, register flow, lesson listing, lesson detail, attempt creation and answering, teacher page, admin page (both tabs)



