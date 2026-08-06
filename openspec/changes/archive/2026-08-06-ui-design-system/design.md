## Context

The Temple Courts UI currently has no design system — raw Tailwind v4 utility classes are copy-pasted across pages. Tailwind v4 uses CSS-first configuration (`@import "tailwindcss"` with optional `@theme` blocks), which is the platform for custom design tokens. shadcn/ui sits on Radix UI primitives and generates Tailwind classes — it integrates with our existing Tailwind v4 setup, not against it.

The UI currently supports 8 pages across 4 roles (public, Student, Teacher, Admin) with a sidebar layout, auth forms, lesson browsing, and attempt running.

## Goals / Non-Goals

**Goals:**
- Establish a visual identity that reflects "Clean Sacred" — calm, light, airy, with soft blues and golds
- Create a reusable component library (shadcn/ui + custom) so pages share consistent patterns
- Define Tailwind v4 `@theme` tokens for colors, typography, spacing, radii, and shadows
- Redesign all 8 existing pages to match the new system
- Integrate `@heroicons/react` (already installed) and `lucide-react` (shadcn default) for semantic icon vocabulary

**Non-Goals:**
- No route changes, API changes, or state management changes
- No new pages — this is visual/structural redesign only
- No responsive/mobile design overhaul (existing basic responsiveness stays)
- No animation/motion design system (kept minimal)
- No accessibility audit beyond what shadcn/ui provides out of the box

## Decisions

### D1: shadcn/ui + Radix over hand-built or other libraries

**Choice**: Install shadcn/ui (Radix primitives + Tailwind).

**Rationale**: shadcn/ui provides accessible, well-tested components (Dialog, Tabs, Dropdown) that we need immediately. Unlike MUI/Ant, it generates code into our codebase rather than wrapping as a dependency — we own and customize the output. It's Tailwind-native, which matches our existing stack.

**Alternatives considered**:
- Headless UI: lighter, but fewer primitives (no Tabs, no Dialog animations)
- MUI/Ant Design: heavy, opinionated, conflicts with Tailwind
- Hand-build all: more control, but we'd need to reimplement accessible Dialog, Tabs, Dropdown from scratch

### D2: Tailwind v4 `@theme` over CSS custom properties or config file

**Choice**: Define design tokens in `src/index.css` using Tailwind v4's `@theme` directive.

**Rationale**: Tailwind v4 replaces `tailwind.config.js` with CSS-first config. `@theme` blocks define custom values that flow into utility classes automatically. No separate token file needed.

```
@theme {
  --color-parchment: #FBF7F0;
  --color-cerulean-50: #F0F7FF;
  --color-cerulean-500: #3B82F6;
  --font-family-serif: 'Georgia', 'Merriweather', serif;
  --spacing-page: 2rem;
}
```

### D3: Dual icon libraries — lucide-react (shadcn default) + @heroicons/react (existing)

**Choice**: Keep both. Use `lucide-react` for shadcn components (it's their default, auto-integrated), use `@heroicons/react` for custom UI elements. No conflict — both are SVG icon sets.

### D4: "Clean Sacred" color strategy

**Choice**: Three color layers:

| Layer | Colors | Usage |
|-------|--------|-------|
| **Parchment** | `#FBF7F0`, `#F5F1EB`, `#EBE4D6`, `#D6CCB8` | Page backgrounds, cards, borders — warm ivory/stone, creates the "airy" feel |
| **Cerulean** | `#F0F7FF` → `#1D4ED8` scale | Primary action/links — a soft blue that evokes sky/stained glass |
| **Gold** | `#FFF8E7` → `#B8860B` scale | Accent/highlights — warm golden tones for important UI moments |
| **Slate** | Tailwind's built-in slate | Utility/admin areas, secondary text — kept from defaults for familiarity |
| **Emerald** | `#ECFDF5`, `#059669` | Success/answered states |

### D5: Typography — Inter + serif headings

**Choice**: System sans-serif (Inter preferred) for body text. Georgia/Merriweather stack for headings (`.font-serif` via Tailwind v4's `@theme`).

**Rationale**: Serif headings evoke tradition, Scripture, and scholarly reading. Sans-serif body keeps long-form reading comfortable. Falls back gracefully to system fonts if web fonts don't load.

### D6: Component architecture — shadcn-generated + custom wrappers

**Choice**: shadcn/ui components placed in `src/components/ui/` (standard shadcn convention). Custom app components (LessonCard, QuestionCard, StudentCard) in `src/components/` as wrappers that compose shadcn/ui primitives.

```
src/components/
  ui/                     ← shadcn/ui generated (button, card, input, badge, tabs, dialog)
  LessonCard.tsx          ← composes Card, Badge
  QuestionCard.tsx        ← composes Card, Textarea
  StudentCard.tsx         ← composes Card
  NodeRenderer.tsx        ← recursive tree (existing, restyled)
  ProtectedRoute.tsx      ← existing, restyled
  RequireRole.tsx         ← existing, restyled
```

### D7: Shadcn badge over hand-built pill spans

**Choice**: Use shadcn/ui `<Badge>` with variants matching our semantic states, instead of the current `rounded-full px-2 py-1 text-xs` spans.

Variants needed: `default` (gray/info), `secondary` (neutral), `success` (published/answered), `warning` (draft/pending), `destructive` (unresolved flags).

## Risks / Trade-offs

- **[Risk] Shadcn init may conflict with existing Tailwind v4 setup** → Mitigation: Run `npx shadcn@latest init` with Tailwind v4 option. If conflicts arise, manually configure `components.json`. Test build after each shadcn component add.
- **[Risk] Visual regression on existing pages during redesign** → Mitigation: Create tokens and components first, then migrate pages one at a time. Playwright E2E tests will catch broken flows.
- **[Risk] Shadcn adds bundle weight** → Mitigation: shadcn treeshakes (only used components are added). Radix primitives are small. Expected bundle increase: ~15-20KB gzipped.
- **[Trade-off] Two icon libraries** → lucide-react comes with shadcn by default; @heroicons/react was already installed. Keeping both avoids rework on either side. If bundle size becomes a concern, migrate to one in a follow-up.
