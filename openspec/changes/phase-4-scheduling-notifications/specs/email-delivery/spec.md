# Email Delivery

## ADDED Requirements

### Requirement: Transactional emails sent via Mailgun

The system SHALL send transactional emails via Mailgun for appointment creation and new thread messages.

#### Scenario: Email on appointment created
- **WHEN** a Teacher creates an appointment
- **THEN** an email notification is sent to the student

#### Scenario: Email on new thread message
- **WHEN** a participant posts in a thread
- **THEN** an email notification is sent to the other participant
