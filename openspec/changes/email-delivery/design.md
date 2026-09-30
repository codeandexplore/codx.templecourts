## Context

`IEmailService.SendAsync(to, subject, body)` exists; `LoggingEmailService` is a stub. The only email call site is `CreateAppointmentUseCase` (fire-and-forget, student only). The architecture doc specifies Mailgun as the provider and "fire-and-forget, no outbox/retry in MVP".

## Goals / Non-Goals

**Goals:**
- Real Mailgun delivery via its HTTP API
- Graceful logging fallback when no API key/domain is configured (dev-friendly)
- Wire email to appointment creation/confirmation and thread messages (both directions)

**Non-Goals:**
- Outbox / retry / idempotency (fire-and-forget per MVP)
- HTML email templates (plain-text body only)
- Email scheduling/cron reminders
- SMTP or other providers

## Decisions

### D1: Mailgun HTTP API over SDK

**Choice**: Post to `https://api.mailgun.net/v3/{domain}/messages` with `HttpClient` (basic auth `api:{key}` + form data), no third-party SDK.

**Rationale**: Mailgun's REST API is simple; avoiding a dependency keeps the stack lean. Uses `IHttpClientFactory` ("Mailgun" client) for connection reuse/timeouts.

### D2: Logging fallback when unconfigured

**Choice**: If `Email:ApiKey` or `Email:Domain` is empty, log the email and return (no send).

**Rationale**: Dev/staging runs without credentials; production supplies real keys. Matches the existing dev-friendly behavior.

### D3: Fire-and-forget send (no await blocking)

**Choice**: Email is triggered without awaiting (like the existing appointment email), so a slow/failing mail provider doesn't block the request.

**Rationale**: Matches the documented MVP decision; failures are logged, not surfaced to the caller.

## Risks / Trade-offs

- **[Risk] Swallowed email failures** → Logged via `ILogger`; acceptable for MVP fire-and-forget.
- **[Trade-off] Plain-text only** → No HTML templates; sufficient for transactional notices.
- **[Trade-off] No retry/outbox** → A transient Mailgun failure loses the email; acceptable for MVP.