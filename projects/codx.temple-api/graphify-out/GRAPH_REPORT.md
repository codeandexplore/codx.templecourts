# Graph Report - projects\codx.temple-api  (2026-08-02)

## Corpus Check
- cluster-only mode — file stats not available

## Summary
- 139 nodes · 151 edges · 12 communities
- Extraction: 100% EXTRACTED · 0% INFERRED · 0% AMBIGUOUS
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `c08b7896`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- Codx.Temple.Application
- User
- http
- Codx.Temple.API
- ICurrentUserAccessor
- Codx.Temple.API.IntegrationTests
- Exception
- Codx.Temple.API.Tests
- Codx.Temple.Domain.Tests
- ICallerContextAccessor.cs
- ApplicationExtensions.cs

## God Nodes (most connected - your core abstractions)
1. `Codx.Temple.API` - 18 edges
2. `Codx.Temple.API.IntegrationTests` - 12 edges
3. `Codx.Temple.Application` - 10 edges
4. `Codx.Temple.API.Tests` - 10 edges
5. `User` - 10 edges
6. `Codx.Temple.Application.Tests` - 9 edges
7. `Codx.Temple.Domain.Tests` - 9 edges
8. `Codx.Temple.Infrastructure` - 8 edges
9. `http` - 6 edges
10. `https` - 6 edges

## Surprising Connections (you probably didn't know these)
- `User` --references--> `UserStatus`  [EXTRACTED]
  src/Codx.Temple.Domain/Entities/User.cs → src/Codx.Temple.Domain/Enums/UserStatus.cs
- `RoleAssignment` --references--> `UserRole`  [EXTRACTED]
  src/Codx.Temple.Domain/Entities/RoleAssignment.cs → src/Codx.Temple.Domain/Enums/UserRole.cs
- `User` --references--> `RoleAssignment`  [EXTRACTED]
  src/Codx.Temple.Domain/Entities/User.cs → src/Codx.Temple.Domain/Entities/RoleAssignment.cs

## Import Cycles
- None detected.

## Communities (12 total, 0 thin omitted)

### Community 0 - "Codx.Temple.Application"
Cohesion: 0.09
Nodes (23): FluentValidation (12.1.1), FluentValidation.DependencyInjectionExtensions (12.1.1), Microsoft.EntityFrameworkCore (10.0.4), Microsoft.EntityFrameworkCore.Design (10.0.4), Npgsql.EntityFrameworkCore.PostgreSQL (10.0.3), Scrutor (7.0.0), Codx.Temple.Application, net10.0 (+15 more)

### Community 1 - "User"
Cohesion: 0.12
Nodes (14): Codx.Temple.Domain.Entities, Codx.Temple.Domain.Enums, Codx.Temple.Domain.Exceptions, List, RoleAssignment, DateTimeOffset, Guid, User (+6 more)

### Community 2 - "http"
Cohesion: 0.13
Nodes (15): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, launchBrowser, applicationUrl, commandName (+7 more)

### Community 3 - "Codx.Temple.API"
Cohesion: 0.14
Nodes (14): Microsoft.AspNetCore.Authentication.JwtBearer (10.0.4), Microsoft.AspNetCore.OpenApi (10.0.5), Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore (10.0.4), Microsoft.OpenApi (2.4.1), OpenTelemetry.Extensions.Hosting (1.12.0), OpenTelemetry.Instrumentation.AspNetCore (1.12.0), OpenTelemetry.Instrumentation.EntityFrameworkCore (1.13.0-beta.1), OpenTelemetry.Instrumentation.Http (1.12.0) (+6 more)

### Community 4 - "ICurrentUserAccessor"
Cohesion: 0.18
Nodes (7): CancellationToken, Codx.Temple.Application.Abstractions, ICurrentUserAccessor, Guid, IReadOnlyCollection, IUnitOfWork, Task

### Community 5 - "Codx.Temple.API.IntegrationTests"
Cohesion: 0.18
Nodes (11): Microsoft.AspNetCore.Mvc.Testing (10.0.4), Testcontainers.PostgreSql (4.10.0), Codx.Temple.API.IntegrationTests, net10.0, coverlet.collector (6.0.4), FluentAssertions (8.8.0), Microsoft.NET.Test.Sdk (17.14.1), Moq (4.20.72) (+3 more)

### Community 6 - "Exception"
Cohesion: 0.24
Nodes (6): Codx.Temple.Application.Exceptions, Exception, ConflictException, ForbiddenException, NotFoundException, DomainRuleViolationException

### Community 7 - "Codx.Temple.API.Tests"
Cohesion: 0.22
Nodes (9): Codx.Temple.API.Tests, net10.0, coverlet.collector (6.0.4), FluentAssertions (8.8.0), Microsoft.NET.Test.Sdk (17.14.1), Moq (4.20.72), xunit (2.9.3), xunit.runner.visualstudio (3.1.4) (+1 more)

### Community 8 - "Codx.Temple.Domain.Tests"
Cohesion: 0.25
Nodes (8): Codx.Temple.Domain.Tests, net10.0, coverlet.collector (6.0.4), Microsoft.NET.Test.Sdk (17.14.1), Moq (4.20.72), xunit (2.9.3), xunit.runner.visualstudio (3.1.4), Microsoft.NET.Sdk

### Community 9 - "ICallerContextAccessor.cs"
Cohesion: 0.33
Nodes (4): Codx.Temple.Application.DTOs.Auth, Codx.Temple.Application.Interfaces, CallerContextDto, ICallerContextAccessor

### Community 10 - "ApplicationExtensions.cs"
Cohesion: 0.33
Nodes (4): Codx.Temple.Application, IServiceCollection, ApplicationExtensions, ApplicationMarker

## Knowledge Gaps
- **70 isolated node(s):** `net10.0`, `Microsoft.AspNetCore.Authentication.JwtBearer (10.0.4)`, `Microsoft.AspNetCore.OpenApi (10.0.5)`, `Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore (10.0.4)`, `Microsoft.OpenApi (2.4.1)` (+65 more)
  These have ≤1 connection - possible missing edges or undocumented components.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `Codx.Temple.API` connect `Codx.Temple.API` to `Codx.Temple.Application`, `Codx.Temple.API.IntegrationTests`, `Codx.Temple.API.Tests`?**
  _High betweenness centrality (0.098) - this node is a cross-community bridge._
- **Why does `Codx.Temple.API.IntegrationTests` connect `Codx.Temple.API.IntegrationTests` to `Codx.Temple.Application`, `Codx.Temple.API`?**
  _High betweenness centrality (0.063) - this node is a cross-community bridge._
- **Why does `Codx.Temple.API.Tests` connect `Codx.Temple.API.Tests` to `Codx.Temple.Application`, `Codx.Temple.API`?**
  _High betweenness centrality (0.051) - this node is a cross-community bridge._
- **What connects `net10.0`, `Microsoft.AspNetCore.Authentication.JwtBearer (10.0.4)`, `Microsoft.AspNetCore.OpenApi (10.0.5)` to the rest of the system?**
  _70 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Codx.Temple.Application` be split into smaller, more focused modules?**
  _Cohesion score 0.09420289855072464 - nodes in this community are weakly interconnected._
- **Should `User` be split into smaller, more focused modules?**
  _Cohesion score 0.12121212121212122 - nodes in this community are weakly interconnected._
- **Should `http` be split into smaller, more focused modules?**
  _Cohesion score 0.13333333333333333 - nodes in this community are weakly interconnected._