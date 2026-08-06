## Why

The Temple Courts UI currently uses raw Tailwind v4 defaults with no design system — the same utility classes are copy-pasted across 8 pages, `@heroicons/react` is installed but unused, and the visual identity is indistinguishable from a generic SaaS admin panel. As lesson authoring, student experiences, and teacher review UIs come online, the UI needs a cohesive visual language that reflects the app's purpose: quiet, reverent Bible study and community.

## What Changes

- Install shadcn/ui + Radix UI primitives for accessible, well-structured components
- Define a custom Tailwind v4 `@theme` with "Clean Sacred" design tokens: soft ivory/blue/gold palette, Inter + serif heading typography, generous spacing scale, refined border radii and shadows
- Create reusable component patterns: Button (bronze primary, olive secondary, ghost tertiary), Card (lesson, question, student), Input (form fields, textarea), Badge (status pills), Tab (admin dashboard), Dialog (shadcn dialog for modals)
- Redesign the sidebar layout with serif heading, icons, avatar area, and refined navigation
- Redesign all 8 existing pages (Home, Login, Register, Lessons, LessonDetail, Attempt, Teacher, Admin) to match the new design system
- Replace all text-based status indicators with Heroicons semantic icon vocabulary
- Add dark mode support via Tailwind v4 `dark:` variants using design tokens

## Capabilities

### New Capabilities
- `ui-design-system`: Design tokens (colors, typography, spacing, radii, shadows), layout templates, dark mode strategy, icon vocabulary, and accessibility requirements for The Temple Courts visual identity
- `ui-components`: Reusable component patterns (Button, Card, Input, Badge, Tab, Dialog) with variants, states, and composition rules

### Modified Capabilities
- `ui`: Fill the existing empty placeholder spec with page-level requirements for all 8 pages redesigned to the new system

## Impact

- **code**: All 8 pages in `src/pages/`, 2 components in `src/components/`, `src/layouts/AppLayout.tsx`, `src/index.css` (Tailwind v4 `@theme` block), `src/router.tsx` (no route changes)
- **dependencies**: New packages — `shadcn/ui` (via `npx shadcn@latest init`), `lucide-react` (shadcn default icon set), `class-variance-authority`, `tailwind-merge`, `clsx`
- **breaking**: None. Visual-only change. All existing routes, API calls, and Redux state remain untouched.
- **specs**: Creates `ui-design-system/spec.md`, `ui-components/spec.md`, and fills `ui/spec.md`
