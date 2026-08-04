# Decisions Log

Record of architectural decisions made during workspace design. Not a how-to — see `workspace-setup.md` for patterns and structure, `workspace-guardrails.md` for rules.

---

## Decision records

| Decision | Rationale |
|----------|-----------|
| Monorepo over polyrepo | Single product, tightly coupled projects, cross-project atomic changes required |
| Single root `openspec/`, namespaced by project | Avoids bridging problems inside one repo |
| Per-project `graphify-out/`, committed | Keeps agent context scoped; supports team cold-start |
| Superpowers execution-stage only | Planning/brainstorming skills would compete with OpenSpec |
| PostgreSQL over SQL Server | JSONB for domain JSON columns; open-source; matches containerized dev |
| Clean Architecture (Ezra pattern) | Domain complexity justifies separation; proven in sibling project |
| Use Cases over MediatR | Simpler DI, explicit dependencies, direct controller injection; matches Ezra |
| Controllers over Minimal API | 15+ resource types benefit from controller organization; better Swagger |
| SignalR for live sessions | Real-time sync between Teacher and Student is core to the product |
| pnpm over npm | Faster, stricter, better monorepo support if JS projects grow |
| In-memory token store | Security: tokens never touch localStorage/sessionStorage; matches Ezra |
| Path-scoped CI, tag-triggered CD | Merges to `main` never deploy; only a project-scoped tag (e.g. `codx.temple-api-v0.1.0`) triggers that project's deploy workflow — contains the blast radius of an accidental multi-project PR |
| `packages/` extracted reactively, not speculatively | Premature shared packages guess wrong about the boundary; extract only when a second real consumer exists |
| Additive-first API changes, sequenced PRs preferred over one atomic multi-project PR | Keeps individual PRs reviewable; only genuine breaking changes with no compatible intermediate state need one atomic PR |
| Stay monorepo now; extract per-project reactively against specific criteria, not preemptively | The coupling/PR-size pain monorepo creates is a process-tooling problem (CI scoping, CODEOWNERS, sequencing), not by itself a reason to fragment into multi-repo |

---

## Monorepo vs. Multi-Repo — Tradeoffs

**The honest asymmetry:** multi-repo makes cross-project coupling structurally impossible to do by accident (no `git commit` spans two repos); monorepo only makes it discouraged, via CI/review process that can be skipped or forgotten. This is a real point in multi-repo's favor.

**The counter-argument:** multi-repo relocates the coordination cost rather than removing it — a shared-package change becomes publish, then bump version in every consumer, then a PR in each, then merge/deploy in order, which can turn a same-day cross-cutting change into a multi-week one. It also trades a visible failure mode (a big PR a reviewer can see and CI can catch) for an invisible one (a consumer repo silently pinned to a stale dependency version, surfacing as a production bug weeks later with little paper trail).

**Decision:** stay monorepo. Address coupling/PR-size concerns with tooling (path-scoped CI + tag-triggered CD, boundary enforcement + CODEOWNERS, sequenced-PR convention) rather than repo topology. Revisit only when a specific extraction criterion below actually fires, with a concrete frequency/cost rather than a general worry.

### Extraction criteria — split a project into its own repo when:

| Signal | Why it matters |
|--------|---------------|
| Independent release cadence that collides with the others (e.g. mobile App Store review cycles vs. web's continuous deploy) | Monorepo tagging gets awkward when one project needs a release branch to sit untouched for a review period while `main` keeps moving |
| Different access control needs (e.g. external contractors should see only one project, never the rest) | Repo-level permissions are a much cleaner tool than path-level permissions in one repo |
| A `packages/*` module graduates into a reusable library consumed by a genuinely separate product outside this workspace | At that point it's infrastructure, not "part of the workspace" |
| CI time becomes untenable even with path-scoped/affected-based tooling | Rare — usually a caching gap (Nx/Turborepo), worth ruling out first |

Team size alone is not a trigger — it's these specific operational needs.

### Extraction mechanics, if/when it happens

`git filter-repo` (not the deprecated `filter-branch`) cleanly extracts one project with full history into its own repo. Mechanical and cheap if done close to when the triggering signal fires (e.g. `codx.temple-mobile` extracted early in its life). What's actually hard is history/reference migration: cross-referenced PR/issue numbers, `openspec/changes/` entries that spanned multiple projects, in-flight branches, and CI/deployment audit trail. Delay makes this harder — extract close to the boundary actually forming, not retroactively after years of entangled history.

---

_Last updated: 2026-08-03_
