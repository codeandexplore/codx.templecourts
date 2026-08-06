## ADDED Requirements

### Requirement: Button component with three variants
The UI SHALL provide a reusable Button component via shadcn/ui with three semantic variants: **primary** (cerulean blue, for main actions like "Start Lesson"), **secondary** (olive outline, for secondary actions like "Save"), and **ghost** (transparent with subtle text, for utility actions like "Sign Out"). All variants SHALL support `disabled` state with reduced opacity.

#### Scenario: Primary button renders correctly
- **WHEN** a page renders a primary button with text "Start Lesson"
- **THEN** the button SHALL have cerulean background (`bg-cerulean-600`), white text, and `hover:bg-cerulean-700`

#### Scenario: Ghost button renders correctly
- **WHEN** a page renders a ghost button with text "Sign Out"
- **THEN** the button SHALL have transparent background, subtle text color, and `hover:bg-slate-100 dark:hover:bg-slate-800`

#### Scenario: Disabled button shows reduced opacity
- **WHEN** a button has the `disabled` attribute (e.g., empty form)
- **THEN** the button SHALL render with `opacity-50` and `cursor-not-allowed`

### Requirement: Card component for lesson, student, and question displays
The UI SHALL provide a Card component with a consistent structure: parchment/white background, soft border, generous internal padding (`p-6`), and an optional header area. Variants SHALL include: default (for lists), interactive (hover state for clickable cards), and answered (emerald tint for completed questions).

#### Scenario: Lesson card renders in grid
- **WHEN** the Lessons page loads published lessons
- **THEN** each lesson SHALL render as a Card with lesson number, title, question count, and a status badge

#### Scenario: Interactive card shows hover feedback
- **WHEN** a user hovers over a clickable Card (e.g., a lesson card)
- **THEN** the card SHALL show a subtle shadow lift (`hover:shadow-md`) and border color change (`hover:border-cerulean-200`)

#### Scenario: Answered question card shows success state
- **WHEN** a question in the attempt runner has been answered
- **THEN** the Card SHALL display an emerald left border and a "Answered" badge with `CheckBadgeIcon`

### Requirement: Form Input component consistent across all pages
The UI SHALL provide a reusable Input and Textarea component with consistent styling: parchment/white background, soft border, focused cerulean ring, and accessible labels. Error states SHALL show red border and error message text below.

#### Scenario: Input shows focus ring on tab
- **WHEN** a user focuses an Input field
- **THEN** the Input SHALL display a `ring-2 ring-cerulean-500` focus ring with transparent border

#### Scenario: Input shows error state
- **WHEN** an Input has a validation error (e.g., invalid email)
- **THEN** the Input SHALL display a red border (`border-red-500`) and the associated error message SHALL render below in red text

### Requirement: Badge component for status indicators
The UI SHALL provide a Badge component via shadcn/ui with variants for: **default** (info/gray), **success** (published/answered, emerald), **warning** (draft/pending, gold), and **destructive** (unresolved flags). Badges SHALL be used for lesson status, question status, and role labels.

#### Scenario: Published badge renders emerald
- **WHEN** a lesson has `status: "Published"`
- **THEN** the Badge SHALL render with emerald background and text, with `CheckBadgeIcon` icon

#### Scenario: Draft badge renders gold
- **WHEN** a lesson has `status: "Draft"`
- **THEN** the Badge SHALL render with gold background and text, with `PencilIcon` icon

### Requirement: Tab component for admin and teacher dashboards
The UI SHALL provide a Tabs component via shadcn/ui for dashboards with multiple sections (Admin: Lessons + Roles, Teacher: future tabs). Active tabs SHALL use cerulean styling; inactive tabs SHALL use ghost styling.

#### Scenario: Admin page tabs render correctly
- **WHEN** the Admin page loads
- **THEN** two tabs SHALL render: "Lessons" and "Roles". The active tab SHALL have cerulean underline/background and the inactive tab SHALL have a subtle ghost appearance.

#### Scenario: Tab switch preserves state
- **WHEN** a user clicks an inactive tab
- **THEN** the active tab indicator SHALL move to the clicked tab, and the content panel SHALL switch to match

### Requirement: Dialog/modal component for confirmations and forms
The UI SHALL provide a Dialog component via shadcn/ui for modal confirmations (e.g., role assignment, lesson creation). Dialogs SHALL include overlay, title, description, content area, and action buttons.

#### Scenario: Create lesson dialog opens and closes
- **WHEN** the admin clicks "Create Lesson"
- **THEN** a Dialog SHALL open with form fields (number, title). Submitting or clicking cancel SHALL close the dialog.

#### Scenario: Dialog overlay blocks interaction
- **WHEN** a Dialog is open
- **THEN** a semi-transparent overlay SHALL cover the page, and clicking outside the dialog SHALL NOT close it (persistent for form safety)
