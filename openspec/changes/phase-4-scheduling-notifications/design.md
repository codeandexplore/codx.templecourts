## Context

Phase 3b added AnswerThread communication. Phase 4 adds scheduling and email delivery.

## Goals / Non-Goals

**Goals:**
- Teacher creates appointments with external meeting_link
- Student can view their appointments
- Teacher confirms/cancels appointments
- Status lifecycle: Proposed → Confirmed → Completed/Cancelled
- Email delivery via Mailgun for appointments and thread messages

**Non-Goals:**
- Google Calendar / iCal integration
- Embedded video (meeting_link is a plain URL)
- Recurring appointments
- Reminder scheduling (cron/cronjob) — reminder notification is manual/immediate for MVP

## Decisions

### Meeting link: plain text field

**Decision:** `StudySchedule.MeetingLink` is a plain string. No validation beyond max length.

**Rationale:** Per architecture: "plain external URL, no embedding." Any URL scheme is valid.

### Email: transactional via Mailgun

**Decision:** Mailgun SDK sends transactional emails. Fire-and-forget — no outbox/retry in MVP.

**Rationale:** Per workspace setup doc §1: "Mailgun for transactional email delivery."
