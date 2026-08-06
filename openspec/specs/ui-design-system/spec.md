# UI Design System

The "Clean Sacred" visual identity for The Temple Courts. Defines design tokens (colors, typography, spacing, radii), dark mode support, icon vocabulary, and accessibility baseline. All tokens are defined in a Tailwind v4 `@theme` block.

## Requirements

### Requirement: Design token definitions
The UI SHALL define a Tailwind v4 `@theme` block in `src/index.css` providing custom design tokens for the "Clean Sacred" visual identity. Tokens SHALL include parchment (warm ivory) backgrounds, cerulean (soft blue) primaries, gold accents, emerald success states, serif heading fonts, and a generous spacing scale.

#### Scenario: Tailwind compiles with custom tokens
- **WHEN** the Vite dev server or production build runs
- **THEN** Tailwind v4 SHALL generate utility classes for all custom tokens (e.g., `bg-parchment`, `text-cerulean-500`, `font-serif`, `rounded-xl`)

#### Scenario: Dark mode tokens resolve correctly
- **WHEN** the user's system preference is dark mode
- **THEN** all `dark:` variants using custom tokens SHALL render appropriate dark values (e.g., `dark:bg-slate-900`, `dark:text-parchment-100`)

### Requirement: Two-font typography system
The UI SHALL use system sans-serif (Inter) for body text and a serif stack (Georgia, Merriweather) for headings. Headings at all levels in page content SHALL use the serif font family.

#### Scenario: Page heading renders serif
- **WHEN** any page renders an `<h1>` or `<h2>` heading
- **THEN** the heading SHALL display in the serif font stack

#### Scenario: Body text renders sans-serif
- **WHEN** any page renders body text, labels, or form content
- **THEN** the text SHALL display in the system sans-serif font stack

### Requirement: Contemplative spacing scale
The UI SHALL use a generous spacing scale to create visual calm. Page content SHALL use `p-8` (was `p-6`), cards SHALL use `p-6` (was `p-4`), and inter-card gaps SHALL use `gap-6` (was `gap-4`). Border radii SHALL use `rounded-xl` for cards (was `rounded-lg`).

#### Scenario: Page layout spacing
- **WHEN** any authenticated page renders inside the main content area
- **THEN** the content container SHALL have `p-8` padding and cards within SHALL use `p-6` with `gap-6` between them

### Requirement: Icon vocabulary
The UI SHALL replace all text-based status indicators with semantic icons from `@heroicons/react`. Each icon SHALL have a consistent semantic mapping documented in the component spec.

#### Scenario: Lesson published badge uses icon
- **WHEN** a lesson list card renders a Published badge
- **THEN** it SHALL display the `CheckBadgeIcon` (heroicons) alongside the "Published" text

#### Scenario: Gated node shows lock icon
- **WHEN** a lesson tree renders a gated node (requires_prior_sibling_answered)
- **THEN** it SHALL display the `LockClosedIcon` (heroicons)

### Requirement: Dark mode support
The UI SHALL support dark mode using Tailwind v4's built-in `dark:` variant strategy tied to the `prefers-color-scheme` media query. All pages and components SHALL have complete dark mode coverage using design tokens, not hardcoded Tailwind defaults.

#### Scenario: System dark mode renders correctly
- **WHEN** the user's OS is set to dark mode
- **THEN** all pages SHALL render with dark backgrounds (`dark:bg-slate-950`), light text, and appropriate dark variants for all components

### Requirement: Accessibility baseline
The UI SHALL meet WCAG 2.1 AA contrast ratios for all text content. Form inputs SHALL have visible focus rings (`focus:ring-2 focus:ring-cerulean-500`). Interactive elements SHALL be keyboard navigable.

#### Scenario: Focus ring on form inputs
- **WHEN** a user tabs into any form input field
- **THEN** the input SHALL display a visible 2px cerulean focus ring

#### Scenario: Button contrast meets AA
- **WHEN** a primary button renders with text on background
- **THEN** the contrast ratio between text and background SHALL be at least 4.5:1
