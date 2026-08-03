# Graph Report - projects\codx.temple-e2e  (2026-08-02)

## Corpus Check
- cluster-only mode — file stats not available

## Summary
- 14 nodes · 12 edges · 4 communities
- Extraction: 100% EXTRACTED · 0% INFERRED · 0% AMBIGUOUS
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `c08b7896`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- package.json
- scripts
- devDependencies

## God Nodes (most connected - your core abstractions)
1. `scripts` - 5 edges
2. `@playwright/test` - 2 edges
3. `private` - 1 edges
4. `test` - 1 edges
5. `test:ui` - 1 edges
6. `test:debug` - 1 edges
7. `report` - 1 edges
8. `@playwright/test` - 1 edges

## Surprising Connections (you probably didn't know these)
- None detected - all connections are within the same source files.

## Import Cycles
- None detected.

## Communities (4 total, 0 thin omitted)

### Community 0 - "package.json"
Cohesion: 0.40
Nodes (4): name, private, type, version

### Community 1 - "scripts"
Cohesion: 0.40
Nodes (5): scripts, report, test, test:debug, test:ui

### Community 2 - "devDependencies"
Cohesion: 0.67
Nodes (3): devDependencies, @playwright/test, @playwright/test

## Knowledge Gaps
- **9 isolated node(s):** `name`, `private`, `version`, `type`, `test` (+4 more)
  These have ≤1 connection - possible missing edges or undocumented components.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `scripts` connect `scripts` to `package.json`?**
  _High betweenness centrality (0.487) - this node is a cross-community bridge._
- **Why does `devDependencies` connect `devDependencies` to `package.json`?**
  _High betweenness centrality (0.256) - this node is a cross-community bridge._
- **What connects `name`, `private`, `version` to the rest of the system?**
  _9 weakly-connected nodes found - possible documentation gaps or missing edges._