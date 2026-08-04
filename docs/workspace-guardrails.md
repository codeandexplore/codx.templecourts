# Workspace Guardrails

The Temple Courts — principles governing project independence, versioning, and code review. Read this before planning any change (`/opsx:explore`) and during code review. Enforceable via CI where indicated.

---

## I. Project Independence

**A project is one deployable artifact.** No project may import source code from another deployable project's `src/` directory.

- `codx.temple-api` — .NET 10 Clean Architecture, ships as a single Docker image
- `codx.temple-ui` — React/Vite SPA, ships as static assets
- `codx.temple-e2e` — Playwright test suite, not deployed
- Future: `codx.temple-website`, `codx.temple-mobile`, `codx.temple-integrations`

**Rule:** Zero `import` / `using` / `reference` statements that cross `projects/*/src/` boundaries.

| Allowed | Forbidden |
|---------|-----------|
| UI imports from `node_modules/@heroicons/react` | UI imports from `projects/codx.temple-api/src/Codx.Temple.Application` |
| E2E imports from `@playwright/test` | E2E imports from `projects/codx.temple-ui/src` |
| API internal: `Application` references `Domain` | API imports from `projects/codx.temple-ui/src` |

**CI enforcement:** `.github/workflows/guard.yml` cross-import check.
**Reviewer check:** If you see a cross-project import in a diff, reject the PR.

---

## II. Contract-First Communication

**The API owns the contract.** All communication between projects occurs via the API's OpenAPI specification — never by sharing code.

- `codx.temple-api` publishes `openapi.json` at `/openapi/v1.json`
- `codx.temple-ui` generates TypeScript types from this spec (auto-generated, never hand-edited)
- `codx.temple-e2e` tests against the same contract at the HTTP level
- Future frontends (website, mobile) generate their own types from the same spec

**Rule:** The `openapi.json` in the API project is the single source of truth for integration contracts.

**CI enforcement:** Stale-types check — if `openapi.json` changed and generated types in consumers were not regenerated, CI fails.

**Reviewer check:** No frontend PR should contain hand-written types matching an API endpoint shape. Generate from the spec.

---

## III. Independent Semver

**Each project has its own `MAJOR.MINOR.PATCH` version.** A PR bumps only the project(s) it touches. No forced alignment across projects.

| Project | Version location | Current |
|---------|-----------------|---------|
| `codx.temple-api` | `Directory.Build.props` `<VersionPrefix>` (shared across all 4 .NET projects) | `0.1.0` |
| `codx.temple-ui` | `package.json` `"version"` | `0.1.0` |
| `codx.temple-e2e` | `package.json` `"version"` | `0.1.0` |

**Bump rules:**
- **MAJOR** (1.0.0): breaking API change, removed endpoint, incompatible schema
- **MINOR** (0.2.0): new endpoint, new field, new feature, contract addition
- **PATCH** (0.1.1): bug fix, typo, refactor, no contract change

**Git tags:** `{project}-v{M}.{m}.{p}` — e.g. `codx.temple-api-v0.1.0`, `codx.temple-ui-v0.2.1`

**CI enforcement (semi):** CI checks that a version was bumped. Correct magnitude (minor vs major) is a human review decision.
**Reviewer check:** Does the version bump magnitude match the change's impact?

---

## IV. One Project Per PR

**A single merged PR touches code in exactly one `projects/*/` directory.**

Cross-project changes (e.g. API contract change + UI consumption) are staged as sequential PRs:

1. Commit and push all changes locally (working branch may contain both)
2. Stage only the first project's files: `git add projects/codx.temple-api/`
3. Commit: `feat(api): add GET /lessons/:id`
4. Push, open PR, review, merge, deploy
5. Stage the second project's files: `git add projects/codx.temple-ui/`
6. Commit: `feat(ui): consume new lesson detail endpoint`
7. Push, open PR, review, merge, deploy

This ensures:
- Each PR has a focused review scope
- Each merge triggers an independent deployment
- API v0.2.0 is live and stable before UI v0.3.0 ships against it

**CI enforcement:** PR check rejects if diff touches more than one `projects/*/` directory.
**Exception:** Future `packages/` changes count as their own "project" — consuming projects update their dependency in follow-up PRs.

### Sequencing cross-project changes

Cross-project changes are sequenced as additive PRs — ship one project first, then the next. **You may develop both API and UI changes on the same branch.** Staging and merging are sequential — not writing code.

1. **API first:** additive endpoint/field change (never break existing contract) → commit, push, PR, review, merge, tag, deploy.
2. **UI/mobile second:** consume the new contract → commit, push, PR, review, merge, tag, deploy.
3. **Cleanup (rare):** remove old API surface once nothing depends on it.

See the worked example below for the full git command sequence.

**Exception:** only use one atomic PR when there's a genuine breaking change with no backward-compatible intermediate state. Note the plan explicitly in the OpenSpec change doc's PR Plan so it's decided at planning time, not discovered by a confused reviewer.

---

## V. E2E as Contract Boundary

**E2E tests import zero application source code.** They validate behavior at the HTTP/DOM level only.

- E2E imports: `@playwright/test`, `playwright` — nothing else
- E2E talks to the app via URLs, selectors, API requests — never via internal state
- E2E code must be portable: run it against any deployed instance (local, staging, production)

**CI enforcement:** CI checks that E2E project contains no imports from other `projects/*/src/` directories.

---

## VI. Version Tags

**Every merge to `main` receiving a version bump must be tagged.**

Format: `{project}-v{MAJOR}.{MINOR}.{PATCH}`

Examples:
```
codx.temple-api-v0.1.0
codx.temple-api-v0.1.1
codx.temple-ui-v0.2.0
codx.temple-e2e-v0.1.0
```

**CI enforcement (future):** CI auto-tags on merge. Manual tagging during Phase 0.

---

## VII. Shared Packages (Future)

When shared code is needed across multiple frontends (website, mobile), it lives in `packages/`, not in any project's `src/`.

**Planned packages:**
```
packages/
  codx.temple-api-types/      ← Generated from OpenAPI spec, consumed by all frontends
  codx.temple-design-tokens/  ← Colors, typography, spacing — pure TypeScript, zero React
  codx.temple-auth-core/      ← OIDC token handling, shared across platforms
```

**Rules (when active):**
- Each package has its own `package.json` with independent semver
- Packages are pure TypeScript — no React, no platform-specific code
- Consumers pin package versions in their own `package.json`
- Changes to a package ship as their own PR (same as §IV)
- Consuming projects update their dependency lock in a follow-up PR

**Not shared (by design):**
- React components — mobile and web have different rendering engines
- Platform-specific code — each project owns its own UI layer

---

## Enforcement Summary

| Rule | CI Enforces | Human Checks During Review |
|------|-------------|---------------------------|
| I. No cross-project `src/` imports | Yes (grep guard) | — |
| II. OpenAPI types up to date | Yes (stale-types check) | Contract makes semantic sense |
| III. Version bumped | Semi (detects no-bump) | Bump magnitude is correct |
| IV. One project per PR | Yes (path check) | — |
| V. E2E zero app imports | Yes (grep guard) | Tests cover golden path |
| VI. Git tag present | — (future) | Tag exists and format is correct |

---

## Worked Example: API + UI Cross-Project Change

**Goal:** Add `GET /lessons/:id` endpoint, consume it in UI lesson-detail page.

```
# 1) Create feature branch from main
git checkout main
git pull
git checkout -b feat/lesson-detail

# 2) Develop both API and UI changes freely on the same branch
#    (write API endpoint, DTOs, tests; write UI page, components, tests)

# ═══════════════════════════════════════════════════════════
# 3) Stage 1 — Ship API first
# ═══════════════════════════════════════════════════════════

# Stage only API files
git add projects/codx.temple-api/

# Commit API changes (include version bump if adding new surface)
git commit -m "feat(api): add GET /lessons/:id endpoint"
#   → bumps codx.temple-api to 0.2.0 (minor — new endpoint)

# Push and open PR
git push -u origin feat/lesson-detail
gh pr create --title "feat(api): add GET /lessons/:id" --body "..."

# Review → merge → tag → deploy
#   After merge to main:
git checkout main && git pull
git tag codx.temple-api-v0.2.0
git push --tags
#   Deploy API (CI/CD or manual)

# ═══════════════════════════════════════════════════════════
# 4) Stage 2 — Ship UI second (API is now live)
# ═══════════════════════════════════════════════════════════

# Back on the feature branch
git checkout feat/lesson-detail

# Regenerate API types from the live API's openapi.json
#   pnpm generate-types   (or equivalent script)

# Stage only UI files
git add projects/codx.temple-ui/

# Commit UI changes (own version bump)
git commit -m "feat(ui): add lesson detail page consuming GET /lessons/:id"
#   → bumps codx.temple-ui to 0.2.0 (minor — new feature)

# Push same branch, open second PR
git push
gh pr create --title "feat(ui): add lesson detail page" --body "..."

# Review → merge → tag → deploy
#   After merge to main:
git checkout main && git pull
git tag codx.temple-ui-v0.2.0
git push --tags
#   Deploy UI
```

### Key points

- **Write both changes on one branch.** No need to wait for API approval before starting UI work.
- **Stage per-project.** `git add projects/codx.temple-api/` then `git add projects/codx.temple-ui/` — CI rejects a PR whose diff touches two `projects/*/` directories.
- **Ship API before UI.** UI's PR depends on the API contract being deployed and live.
- **Regenerate types.** After API merges, regenerate UI types from the live `openapi.json` before committing the UI PR.
- **Two commits, two PRs, two version bumps, two tags.** Each project gets its own independent version lifecycle.

---

_Last updated: 2026-08-03_
