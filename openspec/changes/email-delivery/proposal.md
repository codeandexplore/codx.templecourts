## Why

Email delivery is a logging stub — `LoggingEmailService` writes to the logger and sends nothing. The `email-delivery` spec requires transactional emails via Mailgun for appointment creation and new thread messages. With a config-ready Mailgun implementation that falls back to logging when no API key is configured, email works in production and degrades gracefully in development.

## What Changes

- New `MailgunEmailService` — sends via Mailgun's HTTP API; falls back to logging when `Email:ApiKey`/`Email:Domain` are unset
- Replace the `LoggingEmailService` DI registration
- Add `Email` configuration section (`ApiKey`, `Domain`, `From`)
- Wire email to appointment creation/confirmation (→ student) and thread messages (→ recipient, both directions)

## Capabilities

### Modified Capabilities
- `email-delivery`: Implements real Mailgun delivery with a logging fallback, and wires it to the appointment and thread-message triggers.

## Impact

- **New service**: `MailgunEmailService`
- **Removed service**: `LoggingEmailService`
- **Modified**: `InfrastructureExtensions` (register `IHttpClientFactory`-backed Mailgun client + service), `CreateAppointmentUseCase` (already emails — now uses the real service), `ConfirmAppointmentUseCase` (email student), `PostThreadMessageUseCase` (email recipient)
- **Config**: `appsettings.json` gains an `Email` section
- **No new entities or migrations**