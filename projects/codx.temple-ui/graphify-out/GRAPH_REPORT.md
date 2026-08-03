# Graph Report - projects\codx.temple-ui  (2026-08-02)

## Corpus Check
- cluster-only mode — file stats not available

## Summary
- 140 nodes · 132 edges · 35 communities (13 shown, 22 thin omitted)
- Extraction: 100% EXTRACTED · 0% INFERRED · 0% AMBIGUOUS
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `c08b7896`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- compilerOptions
- compilerOptions
- package.json
- plugins
- scripts
- App.tsx
- devDependencies
- tsconfig.json
- autoprefixer
- eslint
- @types/react
- eslint-plugin-react-refresh
- globals
- jsdom
- msw
- oxlint
- postcss
- tailwindcss
- @tailwindcss/postcss
- @testing-library/react
- @testing-library/user-event
- @types/node
- @types/react-dom
- typescript
- typescript-eslint
- vite
- @vitejs/plugin-react
- vitest
- @vitest/coverage-v8

## God Nodes (most connected - your core abstractions)
1. `compilerOptions` - 18 edges
2. `compilerOptions` - 15 edges
3. `scripts` - 9 edges
4. `plugins` - 4 edges
5. `rules` - 3 edges
6. `lib` - 3 edges
7. `react` - 2 edges
8. `react/only-export-components` - 2 edges
9. `react` - 2 edges
10. `react-dom` - 2 edges

## Surprising Connections (you probably didn't know these)
- `plugins` --extends--> `react`  [EXTRACTED]
  .oxlintrc.json → .oxlintrc.json  _Bridges community 3 → community 5_

## Import Cycles
- None detected.

## Communities (35 total, 22 thin omitted)

### Community 0 - "compilerOptions"
Cohesion: 0.08
Nodes (23): DOM, src, vite/client, compilerOptions, allowArbitraryExtensions, allowImportingTsExtensions, erasableSyntaxOnly, jsx (+15 more)

### Community 1 - "compilerOptions"
Cohesion: 0.10
Nodes (19): node, vite.config.ts, compilerOptions, allowImportingTsExtensions, erasableSyntaxOnly, lib, module, moduleDetection (+11 more)

### Community 2 - "package.json"
Cohesion: 0.20
Nodes (9): dependencies, react, react-dom, name, private, type, version, react (+1 more)

### Community 3 - "plugins"
Cohesion: 0.22
Nodes (8): plugins, rules, react/only-export-components, react/rules-of-hooks, $schema, oxc, typescript, warn

### Community 4 - "scripts"
Cohesion: 0.22
Nodes (9): scripts, build, dev, lint, preview, test, test:coverage, test:watch (+1 more)

### Community 5 - "App.tsx"
Cohesion: 0.31
Nodes (5): react, App(), router, AppLayout(), HomePage()

### Community 6 - "devDependencies"
Cohesion: 0.29
Nodes (7): @eslint/js, eslint-plugin-react-hooks, devDependencies, @eslint/js, eslint-plugin-react-hooks, @testing-library/jest-dom, @testing-library/jest-dom

## Knowledge Gaps
- **80 isolated node(s):** `$schema`, `typescript`, `oxc`, `react/rules-of-hooks`, `warn` (+75 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **22 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `devDependencies` connect `devDependencies` to `package.json`, `autoprefixer`, `eslint`, `@types/react`, `eslint-plugin-react-refresh`, `globals`, `jsdom`, `msw`, `oxlint`, `postcss`, `tailwindcss`, `@tailwindcss/postcss`, `@testing-library/react`, `@testing-library/user-event`, `@types/node`, `@types/react-dom`, `typescript`, `typescript-eslint`, `vite`, `@vitejs/plugin-react`, `vitest`, `@vitest/coverage-v8`?**
  _High betweenness centrality (0.210) - this node is a cross-community bridge._
- **Why does `scripts` connect `scripts` to `package.json`?**
  _High betweenness centrality (0.052) - this node is a cross-community bridge._
- **What connects `$schema`, `typescript`, `oxc` to the rest of the system?**
  _80 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `compilerOptions` be split into smaller, more focused modules?**
  _Cohesion score 0.08333333333333333 - nodes in this community are weakly interconnected._
- **Should `compilerOptions` be split into smaller, more focused modules?**
  _Cohesion score 0.1 - nodes in this community are weakly interconnected._