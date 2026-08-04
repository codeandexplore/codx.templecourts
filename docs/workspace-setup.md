# Workspace Setup — The Temple Courts

Single-repo (monorepo) workspace. Three projects: `codx.temple-api`, `codx.temple-ui`, `codx.temple-e2e`.

---

## 1. Technology Decisions

| Decision | Choice | Rationale |
|----------|--------|-----------|
| **Database** | PostgreSQL | JSONB support for `answer_value`/`metadata`/`reference_context` columns; open-source; strong .NET support via Npgsql/EF Core |
| **ORM / Migrations** | EF Core (Npgsql) | Migrations in `Codx.Temple.Infrastructure`; run at startup in dev, via `dotnet ef database update` in CI |
| **Package manager (ui)** | pnpm | Fast, strict, good monorepo support |
| **API style** | Controllers (`[ApiController]`) | Cleaner for 15+ resource types; better Swagger integration; matches Ezra pattern |
| **Real-time** | SignalR | Live study sessions need real-time sync between Teacher and Student |
| **Email** | Mailgun | Transactional email delivery for notifications |
| **E2E** | Playwright | Browser-based validation; matches workspace tooling |

---

## 2. Tools

| Tool | Role | Scope |
|------|------|-------|
| **OpenCode** (sst) | Terminal AI coding agent, via OpenRouter | Config resolves per working directory |
| **OpenSpec** | Spec-driven development: proposal → design → tasks | Single instance at workspace root |
| **Graphify** | Repo knowledge-graph generator for fast agent context retrieval | One instance per project |
| **Superpowers** (selective) | Execution-stage skills: TDD enforcement, systematic debugging, subagent-driven development, code review | Installed globally; brainstorming/planning skills disabled — OpenSpec owns that stage |
| **UI/UX Pro Max** | Design-pattern/UX-guidance skill for frontend generation | Installed inside `projects/codx.temple-ui/` only |
| **GitHub MCP** | PR creation, review, issue tracking | Workspace-wide |
| **Playwright MCP** | Browser-based validation/E2E | Primarily `projects/codx.temple-e2e/` |
| **awesome-design-md** | Curated DESIGN.md collection (reference) | Browse at https://github.com/VoltAgent/awesome-design-md; copy a DESIGN.md into `projects/codx.temple-ui/design.md` to give AI agents a design system to match |

**Tools deliberately scoped:**

- **Superpowers**: Only execution-stage skills installed globally (`systematic-debugging`, `test-driven-development`, `requesting-code-review`, `using-superpowers`). Brainstorming/plan-writing skills are skipped — OpenSpec owns planning.
- **UI/UX Pro Max**: Installed locally in `projects/codx.temple-ui/.agents/skills/` — visible only to UI sessions.
- **DESIGN.md**: Not committed yet; pick a design system from awesome-design-md or write your own. When ready, place at `projects/codx.temple-ui/design.md`.

---

## 3. Folder Structure

```
codx.templecourts/                              ← git root
  AGENTS.md                                     ← workspace-wide conventions
  README.md
  .editorconfig                                 ← shared formatting (C#, JSX/TSX, JSON, MD)
  .gitignore                                    ← dotnet + node + general patterns
  docker-compose.yml                            ← PostgreSQL (local dev)
  scripts/
    graphify-all.ps1                            ← [planned] regenerate per-project graphify-out/
  packages/                                     ← [planned] shared code (when a second consumer exists)
    dotnet/                                     ← [planned]
      Codx.Shared.Domain/
      Codx.Shared.Auth/
    ts/                                         ← [planned]
      codx-api-contracts/
      codx-ui-kit/
  openspec/
    specs/
      api/...
      ui/...
      e2e/...
    changes/...
  .github/
    workflows/
      ci.yml                                    ← path-scoped build/test/lint
      guard.yml                                 ← cross-import + single-project PR check
      deploy-api.yml                             ← [planned] triggered by codx.temple-api-v* tags
      deploy-ui.yml                               ← [planned] triggered by codx.temple-ui-v* tags
      graphify-check.yml                        ← [planned] ensures graphify-out/ is current
    CODEOWNERS                                   ← [planned] per-path review routing
  projects/
    codx.temple-api/
      AGENTS.md                                 ← .NET-specific conventions
      graphify-out/                             ← project-scoped index (committed)
      Codx.Temple.slnx
      Directory.Build.props
      src/
        Codx.Temple.Domain/                     ← entities, value objects, enums, domain exceptions (ZERO dependencies)
        Codx.Temple.Application/                ← use cases, DTOs, validators, abstractions (depends on Domain only)
        Codx.Temple.Infrastructure/             ← EF Core, migrations, auth, email (Mailgun), repositories (depends on Application)
        Codx.Temple.API/                        ← ASP.NET Web API, entrypoint, SignalR hubs, controllers, middleware, observability (depends on Infrastructure)
      tests/
        Codx.Temple.Domain.Tests/
        Codx.Temple.Application.Tests/
        Codx.Temple.API.Tests/
        Codx.Temple.API.IntegrationTests/       ← real DB via Testcontainers.PostgreSql
      docs/
        architecture-and-design.md              ← copy of root docs for project-local reference
    codx.temple-ui/
      AGENTS.md                                 ← React-specific conventions
      graphify-out/
      design.md                                 ← DESIGN.md for AI agent UI generation (from awesome-design-md or custom)
      .opencode/                                ← UI/UX Pro Max skill config, scoped here only
      .agents/skills/ui-ux-pro-max/             ← UI/UX Pro Max installed locally
      package.json
      pnpm-lock.yaml
      vite.config.ts
      tsconfig.json
      tsconfig.app.json
      tsconfig.node.json
      tsconfig.test.json
      index.html
      eslint.config.js                          ← ESLint 10 flat config
      postcss.config.js
      vitest.config.ts
      pnpm-workspace.yaml                       ← build-script approval only (msw, esbuild)
      public/
      src/
        main.tsx                                 ← entry point
        App.tsx                                  ← root component (Provider + RouterProvider)
        App.css
        index.css                                ← Tailwind directives
        app/
          store.ts                               ← Redux store + typed hooks
        routes/
          router.tsx                             ← createBrowserRouter with lazy-loaded routes
        layouts/
          AppLayout.tsx                          ← application shell (sidebar + header + <Outlet/>)
        pages/
          HomePage.tsx
        components/                              ← shared cross-feature components
        features/                                ← domain-driven feature modules (auth, lessons, study-sessions)
        services/                                ← API client, OIDC service, observability
      tests/
        setup.ts
      mocks/                                     ← MSW handlers
    codx.temple-e2e/
      AGENTS.md                                 ← E2E-specific conventions
      graphify-out/
      package.json
      playwright.config.ts
      tests/
      fixtures/
```

### Placement rules

- **`openspec/`** — root level, single instance. Namespaced by project under `specs/`, so cross-project changes are one proposal, one PR.
- **`graphify-out/`** — per project, not workspace-wide. Keeps agent context scoped; committed for team cold-start.
- **`AGENTS.md`** — layered. Root file for workspace-wide conventions; each project has its own for stack-specific conventions. OpenCode's working-directory resolution means sessions inside `projects/codx.temple-api/` pick up only its own `AGENTS.md`.
- **Project-specific tooling** (UI/UX Pro Max, `design.md`) — installed/created inside the owning project's folder only, invisible to sessions working elsewhere.
- **`packages/`** — cross-project shared code only, extracted reactively when a second real consumer exists.

### Setup gotchas

1. `dotnet new` and Vite scaffolding sometimes auto-initialize their own `.git`. After scaffolding any project, confirm no nested `.git` exists — a stray one silently submodule-blacks that folder.
2. After `dotnet new`, delete any auto-generated `ScaffoldingReadMe.txt` files (already in `.gitignore`).
3. `pnpm install` from the workspace root is NOT used — each JS project manages its own `node_modules/`. The `pnpm-workspace.yaml` in `projects/codx.temple-ui/` exists only to approve build scripts for `msw` and `esbuild` (pnpm 11 requirement), not to define a monorepo workspace.
4. The API's `appsettings.Development.json` contains a placeholder connection string — real credentials go in .NET User Secrets (`dotnet user-secrets set`). This file IS committed with placeholder values only.

---

## 4. API Project Pattern (from codx.ezra-api)

### Clean Architecture layer dependencies

```
API → Infrastructure → Application → Domain (zero dependencies)
API also directly references Application for DI registration
```

### Key conventions (verified from ezra-api)

- **No MediatR** — Use cases are concrete `sealed class *UseCase` classes with a single `ExecuteAsync(...)` method. Auto-registered via Scrutor: `scan.AssemblyOf<ApplicationMarker>().Where(t => t.Name.EndsWith("UseCase")).AsSelf().WithScopedLifetime()`.
- **Controllers inject use cases directly** (not via interfaces, not via MediatR).
- **Validation**: FluentValidation, manual invocation in controllers (`_validator.ValidateAsync()`), auto-registered via `AddValidatorsFromAssemblyContaining<ApplicationMarker>()`.
- **EF Core**: global query filters (tenant scoping), `IEntityTypeConfiguration<T>` per entity, `MigrationsAssembly` specified in `AddDbContext`.
- **Repositories**: interfaces in `Application.Abstractions`, implementations in `Infrastructure.Persistence.Repositories`.
- **Observability**: Serilog (dev), OpenTelemetry (tracing: ASP.NET + HttpClient + EF Core), health checks on `/healthz` and `/readyz`.
- **Auth**: JWT Bearer with `MapInboundClaims = false`, `NameClaimType = "sub"`, `RoleClaimType = "application_role"`.
- **Global error handling**: `IExceptionHandler` mapping NotFound/Conflict/Forbidden/DomainRuleViolation/DbUpdateException → HTTP status codes.
- **Program.cs exposes `public partial class Program { }`** for `WebApplicationFactory<T>` in integration tests.
- **Connection strings**: priority is User Secrets > `appsettings.Development.json` > `appsettings.json`.
- **Testing**: xUnit + Moq across all layers. Integration tests use `Testcontainers.PostgreSql` + `WebApplicationFactory<T>`.

### Project-specific adaptions for Temple Courts

- Database: PostgreSQL (not SQL Server). Use `Npgsql.EntityFrameworkCore.PostgreSQL`.
- Testcontainers: `Testcontainers.PostgreSql` (not MsSql).
- Email: Mailgun SDK (not stub).
- SignalR hubs for live study sessions (add `Microsoft.AspNetCore.SignalR` to API project).
- The `reference_context` strip: enforced at the DTO mapping/serialization layer. Integration tests must assert no `reference_context` in any response for Student-authenticated calls.

---

## 5. UI Project Pattern (from codx.ezra-ui)

### Stack

| Layer | Choice |
|-------|--------|
| React | 19.x (19.2.8) |
| Vite | 8.x (8.2.0) |
| TypeScript | strict mode (6.0) |
| Package manager | pnpm 11 |
| Styling | Tailwind CSS v4 (PostCSS plugin) |
| State (server) | RTK Query (Axios-based baseQuery with auto auth token injection + 401 retry) |
| State (client) | Redux Toolkit slices (per-feature UI state) |
| Routing | React Router v7, `createBrowserRouter`, lazy-loaded routes |
| Forms | react-hook-form + zod via @hookform/resolvers |
| Auth | oidc-client-ts (Authorization Code + PKCE, in-memory token store) |
| Icons | @heroicons/react (outline + solid) |
| Headless primitives | @headlessui/react |
| Tables | @tanstack/react-table (headless, server-side pagination) |
| Testing | Vitest + jsdom + React Testing Library + MSW |
| Linting | ESLint 10 flat config |
| API mock | MSW v2 |
| Design system | `design.md` (from awesome-design-md or custom); UI/UX Pro Max skill scoped to project |

### Key conventions (verified from ezra-ui)

- **Feature-first architecture**: each feature is a self-contained module with `api/`, `components/`, `hooks/`, `slices/`, `types/` folders + barrel `index.ts`.
- **Components ≤ 200 lines** (per Ezra constitution; enforce as guideline).
- **No `any` types**. Zero ESLint warnings/errors in committed code.
- **Tokens in memory only** — never localStorage/sessionStorage.
- **Dark mode**: full support via Tailwind `dark:` prefix; built-in Tailwind v4 color scheme.
- **All styles co-located** as Tailwind utility classes in JSX — no CSS modules, no separate CSS files per component.
- **RTK Query tag-based cache invalidation** for automatic refetch after mutations.
- **Client-side pagination/sort/filter state** in Redux UI slices (NOT URL params).
- **Observability**: OpenTelemetry tracing (route-level + per-API-call spans), Sentry-like error boundary.

### Project-specific adaptions for Temple Courts

- **SignalR client** connection for live study sessions.
- **Lesson tree renderer**: recursive component for depth-first lesson traversal (max depth 3).
- **5 question types**: YES_NO, TRUE_FALSE, FILL_BLANK, SELECT_EMBEDDED, ESSAY — each gets its own input component in `features/lessons/components/`.
- **Sibling gating UI**: enforce `requires_prior_sibling_answered` in the lesson runner.
- **`reference_context`** must never render in Student-role UI — strictly a server-side strip, UI should not even have the field in its TypeScript types for student-facing responses.

### DESIGN.md (design system for AI agents)

The `DESIGN.md` file at `projects/codx.temple-ui/design.md` tells AI agents how the UI should look and feel (colors, typography, spacing, components). It's the visual counterpart to `AGENTS.md` (which covers build conventions).

**Source:** Browse the curated collection at https://github.com/VoltAgent/awesome-design-md and pick a design system that matches the desired tone (e.g., Notion for warm minimalism, Linear for precise engineering aesthetic, Stripe for gradient elegance). Copy the chosen `DESIGN.md` into the UI project.

**If no DESIGN.md exists yet:** The UI/UX Pro Max skill will generate UI based on its own internal design rules. For consistent output across sessions, create a `design.md`.

---

## 6. E2E Project Pattern

### Stack

| Layer | Choice |
|-------|--------|
| Framework | Playwright |
| Language | TypeScript |
| Fixtures | Custom fixtures for authenticated sessions (Admin, Teacher, Student) |
| API helpers | Direct API calls for test setup/teardown (bypass UI for seed data) |

### Key conventions

- Tests organized by domain: `tests/auth/`, `tests/lessons/`, `tests/study-sessions/`.
- Custom `test.extend()` fixtures for each role (authenticated browser context).
- Use Playwright MCP for interactive debugging during development.
- Smoke tests: Admin creates/publishes lesson, Student answers, Teacher reviews — full golden path.

---

## 7. Docker Compose (local dev)

```yaml
# docker-compose.yml at workspace root
services:
  postgres:
    image: postgres:16-alpine
    environment:
      POSTGRES_USER: templecourts
      POSTGRES_PASSWORD: templecourts_dev
      POSTGRES_DB: templecourts
    ports:
      - "5432:5432"
    volumes:
      - pgdata:/var/lib/postgresql/data

  # API is NOT in compose — run via `dotnet run` for hot reload
  # UI is NOT in compose — run via `pnpm dev` for HMR

volumes:
  pgdata:
```

---

## 8. Google OAuth Setup

Google OAuth login is part of Phase 0 (auth). The flow: the SPA shows the Google sign-in popup → obtains an `id_token` → sends it to `POST /auth/google`. The API validates the `id_token` locally against Google's JWKS keys — no network call to Google per login.

### 8.1 Google Cloud Console Setup

1. Go to [Google Cloud Console](https://console.cloud.google.com) and create or select a project (e.g. "Temple Courts").

2. **Configure the OAuth consent screen** (APIs & Services → OAuth consent screen):
   - User type: **External** (production users; testing mode limits to test accounts)
   - App name: "The Temple Courts"
   - User support email: your email
   - Developer contact email: your email
   - Scopes: use defaults only (`openid`, `email`, `profile`) — no extra scopes needed
   - Test users: add your own Google account (and any other early testers)

3. **Create an OAuth 2.0 Client ID** (APIs & Services → Credentials → Create Credentials → OAuth client ID):
   - Application type: **Web application**
   - Name: e.g. "Temple Courts Dev"
   - **Authorized JavaScript origins:**
     - `http://localhost:5173` (Vite dev server — the SPA origin that shows the Google sign-in popup)
   - **Authorized redirect URIs:** leave empty (not used in this flow; the SPA handles the popup, not a server redirect)

4. **Copy the Client ID** — it looks like `1234567890-abc123.apps.googleusercontent.com`. This is the only value the API needs. You do **not** need the Client Secret for this flow (the API validates `id_token` signatures, it does not exchange a code).

### 8.2 API Configuration

Store the Client ID via .NET User Secrets (never commit real credentials):

```sh
cd projects/codx.temple-api/src/Codx.Temple.API
dotnet user-secrets set "GoogleAuth:ClientId" "1234567890-abc123.apps.googleusercontent.com"
```

The API reads this value at startup and uses it to validate the `aud` claim in incoming Google `id_token`s. The `appsettings.json` contains a placeholder section for documentation:

```json
"GoogleAuth": {
  "ClientId": ""
}
```

### 8.3 How Validation Works (Implementation Detail)

The `GoogleAuthService` (in Infrastructure) does:

1. Fetch Google's JWKS from `https://www.googleapis.com/oauth2/v3/certs` (cached with a 24-hour TTL)
2. Validate the `id_token` JWT signature using the RS256 key from JWKS
3. Validate claims: `aud` matches configured Client ID, `iss` is `accounts.google.com` or `https://accounts.google.com`, `exp` is in the future
4. Extract `email`, `name`, `sub` (Google account ID) from the validated token

Google's JWKS endpoint is called once at startup and every 24 hours — not per login. This avoids the latency and external-dependency risk of calling the deprecated `tokeninfo` endpoint on every login.

### 8.4 Testing in Dev (API-Only, No SPA)

During Phase 0 the UI doesn't exist yet. To test `/auth/google` directly:

- **Option A — OAuth 2.0 Playground:** Go to https://developers.google.com/oauthplayground, select your Client ID under settings gear → "Use your own OAuth credentials", authorize with scope `openid email profile`, exchange auth code for tokens, and use the resulting `id_token` in a curl request:
  ```sh
  curl -X POST http://localhost:5000/auth/google \
    -H "Content-Type: application/json" \
    -d '{"id_token": "<paste-id-token-here>"}'
  ```

- **Option B — Quick script:** Once the SPA exists (Phase 2+), sign in via the UI and copy the `id_token` from browser dev tools. Redirect here for now: use the Playground described above.

### 8.5 Production Notes

- Add production origins (e.g. `https://templecourts.com`) to Authorized JavaScript origins
- Publish the OAuth consent screen (remove "Testing" status) before going live
- Verify the app meets Google's verification requirements for external apps (logo, privacy policy URL, terms of service URL)

---

## 9. Workflow

| Stage | Tool | Notes |
|-------|------|-------|
| **1. Design** | OpenSpec `/opsx:explore` | Exploration is first-class |
| **2. Plan / Spec** | OpenSpec `/opsx:propose` | Produces artifacts in `openspec/`; namespace by project |
| **3. Implement** | OpenCode + Superpowers (TDD, subagents) + Graphify context | Graphify supplies focused per-project context |
| **4. Review** | Superpowers code-review skill + GitHub MCP | Review against `docs/workspace-guardrails.md` |
| **5. Validate** | Playwright MCP (E2E) + `/opsx:ff` for fast-follow fixes | Trivial fixes skip OpenSpec entirely |

### Bug-fix tiers

- **Trivial fix** → skip OpenSpec.
- **Ordinary bug** → `/opsx:ff` (fast-forward).
- **Pure refactor** → `--skip-specs`.

---

## 10. Phase 0 Implementation Order

| Step | Action | Artifact | Status |
|------|--------|----------|--------|
| 0.1 | Update `.gitignore` (merge Node patterns + general) | `.gitignore` | ✅ done |
| 0.2 | Add `.editorconfig` (C#, JSX/TSX, JSON, MD) | `.editorconfig` | ✅ done |
| 0.3 | Add `docker-compose.yml` (PostgreSQL) | `docker-compose.yml` | ✅ done |
| 0.4 | Scaffold `projects/codx.temple-api/` via `dotnet new` | solution + 4 classlib + webapi | ✅ done |
| 0.5 | Verify no nested `.git` in api | manual check | ✅ done |
| 0.6 | Scaffold `projects/codx.temple-ui/` via `pnpm create vite` | Vite React TS template | ✅ done |
| 0.7 | Verify no nested `.git` in ui | manual check | ✅ done |
| 0.8 | Scaffold `projects/codx.temple-e2e/` | Playwright config | ✅ done |
| 0.9 | Verify no nested `.git` in e2e | manual check | ✅ done |
| 0.10 | Add path-scoped CI + guard workflows | `ci.yml`, `guard.yml` | ✅ done |
| 0.11 | Install tooling (Superpowers, UI/UX Pro Max, Graphify) | skills + graphify-out/ | ✅ done |
| 0.12 | Create `openspec/` directory structure | `openspec/specs/{api,ui,e2e}` | ✅ done |
| 0.13 | Versioning: `Directory.Build.props`, semver, git tag convention | docs, config files | ✅ done |
| 0.14 | Implement auth (email/password + Google OAuth) in API | Phase 0 deliverable | ⏳ pending |
| 0.15 | Implement core schema via EF migration | Phase 0 deliverable | ⏳ pending |
| 0.16 | Admin-guard middleware + role checks | Phase 0 deliverable | ⏳ pending |
| 0.17 | Add tag-triggered deploy workflows (`deploy-api.yml`, `deploy-ui.yml`) | see §12 | 📅 later |
| 0.18 | Add `graphify-check.yml` + `.github/CODEOWNERS` | CI + review routing | 📅 later |
| 0.19 | Create `packages/` skeleton (when a second real consumer exists) | see §13 | 📅 later |

### pnpm 11 note

pnpm 11 requires build-script approval for packages. `projects/codx.temple-ui/pnpm-workspace.yaml` approves `msw` and `esbuild` — if adding new packages with build scripts, update this file.

---

## 11. Versioning Convention

| Project | Version lives in | Current |
|---------|-----------------|---------|
| `codx.temple-api` | `Directory.Build.props` `<VersionPrefix>` (shared across all 4 .NET projects — they ship as one artifact) | `0.1.0` |
| `codx.temple-ui` | `package.json` `"version"` | `0.1.0` |
| `codx.temple-e2e` | `package.json` `"version"` | `0.1.0` |

**Git tags:** `{project}-v{M}.{m}.{p}` — e.g. `codx.temple-api-v0.1.0`. Tags are created on merge to `main`.

**Bump rules and enforcement:** see `docs/workspace-guardrails.md` §III and §VI.

---

## 12. CI/CD Architecture

**CI** — path-scoped `ci.yml` using `dorny/paths-filter`. Detects which projects changed, conditionally runs each project's build/test job. The `e2e` job runs whenever `api` or `ui` files change (it needs both to do anything meaningful), not only when `codx.temple-e2e/` itself changed.

**CD** — separate tag-triggered workflows (`deploy-api.yml` on `codx.temple-api-v*` tags, `deploy-ui.yml` on `codx.temple-ui-v*`). A UI tag cannot trigger an API deploy. Tags are pushed as a follow-up after the version-bump merge to `main`. Deploy workflows should re-verify the test suite for the exact commit SHA before publishing.

**Guard** — `guard.yml` runs on every PR: cross-import check (no `projects/*/src/` imports across project boundaries) + single-project check (one PR = one `projects/*/` directory).

See `docs/workspace-guardrails.md` for the rules these enforce.

---

## 13. Project Boundaries

Projects communicate via API contracts and `packages/`, never by importing each other's source. This is enforced by CI (`guard.yml` cross-import check).

Planned `packages/` (created reactively when a second consumer exists):

```
packages/
  dotnet/
    Codx.Shared.Domain/          ← platform-agnostic primitives, zero project deps
    Codx.Shared.Auth/            ← JWT claims mapping, role constants
  ts/
    codx-api-contracts/          ← generated from API's OpenAPI spec (consumed by all frontends)
    codx-ui-kit/                  ← design tokens + pure logic (validators, formatters) — no React components
```

Changes to `packages/` are always a cross-project OpenSpec change (never `--skip-specs`). Each package has its own semver; consumers pin specific versions.

Full rules: see `docs/workspace-guardrails.md` §I, II, VII.

---

_Last updated: 2026-08-03 (added §8 Google OAuth setup)_
