## 1. Mailgun Service

- [x] 1.1 Create `MailgunEmailService` (Mailgun HTTP API + logging fallback)
- [x] 1.2 Register `IHttpClientFactory` "Mailgun" client + `IEmailService → MailgunEmailService` in `InfrastructureExtensions`
- [x] 1.3 Remove `LoggingEmailService` registration and file
- [x] 1.4 Add `Email` section to `appsettings.json`

## 2. Wire Email Triggers

- [x] 2.1 `ConfirmAppointmentUseCase` — email student on confirm
- [x] 2.2 `PostThreadMessageUseCase` — email the recipient (both directions)

## 3. Verification

- [x] 3.1 Run `dotnet build` and `dotnet test` in API project
- [x] 3.2 Manually verify: unconfigured → logs email; (optionally with real Mailgun keys) → sends
