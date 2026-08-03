# AGENTS.md

The Temple Courts — a Bible study & community app. Three projects in a monorepo: `codx.temple-api` (.NET 10), `codx.temple-ui` (React/Vite), `codx.temple-e2e` (Playwright).

## Read these before writing anything

- `docs/workspace-guardrails.md` — project independence, versioning, and PR rules (enforced by CI).
- `docs/architecture-and-design.md` — domain model, MVP scope, and non-negotiable product constraints.
- `docs/workspace-setup.md` — monorepo layout, tooling, and workflow conventions.

## Non-negotiable product constraints (from architecture doc)

These govern every design/code decision; preserve them in anything you write:

- **Guided Discovery Principle**: the app teaches by questions and Scripture, never by declaring conclusions. No content, lesson, or feature may state/dictate answers.
- **`reference_context` (expected-answer/guidance) is NEVER served to the Student role** — strip it at the response-serialization layer.
- **No auto-grading, ever.** No correct/incorrect concept; "reviewed" is a teacher action, **live `StudySession` only** (never asynchronous).
- **Lesson versioning**: `LessonAttempt.lesson_version_id` is pinned at creation; stable identity is `_key` (survives version bumps), `id` is version-specific. Orphaned bank notes are flagged at read-time, never deleted.
- Lesson structure is a **dynamic recursive tree** (max 3 levels, leaf-only questions, min 1 top-level node before publish), traversed depth-first on read.

## Project structure (existing)

```
codx.templecourts/
  projects/
    codx.temple-api/          ← .NET 10, Clean Architecture (Domain/Application/Infrastructure/API)
    codx.temple-ui/           ← React 19, Vite 8, TypeScript, Tailwind 4, Redux Toolkit
    codx.temple-e2e/          ← Playwright
```

When scaffolding new projects under `projects/`, confirm `dotnet new`/Vite did **not** create a nested `.git` — a stray one silently submodule-blacks out that folder.

## Developer commands

```sh
# Start local dev infrastructure
docker compose up -d                                   # PostgreSQL 16

# API
cd projects/codx.temple-api
dotnet build                                           # build all
dotnet test                                            # run all tests
dotnet run --project src/Codx.Temple.API               # start API

# UI
cd projects/codx.temple-ui
pnpm dev                                               # start Vite dev server
pnpm lint                                              # ESLint
pnpm type-check                                        # tsc --noEmit
pnpm test                                              # Vitest

# E2E
cd projects/codx.temple-e2e
pnpm test                                              # Playwright
```

## Workflow conventions

- Planning goes through OpenSpec (`/opsx:explore` → `/opsx:propose` → implement from `openspec/changes/.../tasks.md`).
- Bug-fix tiers: trivial → skip OpenSpec; ordinary → `/opsx:ff`; pure refactor → `--skip-specs`.
- Use Graphify for per-project context; run opencode from the project subdirectory so it picks up the layered project-level `AGENTS.md`.
