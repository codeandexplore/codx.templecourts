## Context

The API is feature-complete through Phase 4. Phase 5 is about quality: real content, security validation, audit trail, and regression coverage.

## Goals / Non-Goals

**Goals:**
- Replace lesson 2-16 placeholders with content-rich lessons at depths 1, 2, and 3
- Add 5 question types across seed data (not just Essay)
- Verify role enforcement on every protected endpoint (security tests)
- Verify reference_context is absent from all Student-role responses
- Audit log for role elevation and reassignment
- Regression tests for critical cross-system flows

**Non-Goals:**
- Performance optimization
- Load testing
- Production deployment config (CI/CD, secrets, monitoring)
- UI or E2E tests
