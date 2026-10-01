## ADDED Requirements

### Requirement: Email delivered via Mailgun with logging fallback
The system SHALL send transactional email via Mailgun's HTTP API when `Email:ApiKey` and `Email:Domain` are configured, and SHALL log-and-skip (no send) when they are unset.

#### Scenario: Configured Mailgun sends email
- **WHEN** an email is triggered and Mailgun credentials are configured
- **THEN** the email SHALL be sent via Mailgun

#### Scenario: Unconfigured Mailgun falls back to logging
- **WHEN** an email is triggered and Mailgun credentials are unset
- **THEN** the email SHALL be logged and not sent

### Requirement: Email triggered for appointment events
The system SHALL send email to the student when an appointment is created and when it is confirmed.

#### Scenario: Appointment created emails student
- **WHEN** a Teacher creates an appointment
- **THEN** the student SHALL receive an email

#### Scenario: Appointment confirmed emails student
- **WHEN** a Teacher confirms an appointment
- **THEN** the student SHALL receive an email

### Requirement: Email triggered for thread messages
The system SHALL send email to the other participant when a message is posted in a thread.

#### Scenario: Teacher message emails student
- **WHEN** a Teacher posts in a student's thread
- **THEN** the student SHALL receive an email

#### Scenario: Student message emails teacher
- **WHEN** a Student posts in their thread
- **THEN** the thread's teacher SHALL receive an email