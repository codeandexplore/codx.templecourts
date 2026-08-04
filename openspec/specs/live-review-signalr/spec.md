# live-review-signalr Specification

## Purpose
TBD - created by archiving change phase-3a-live-review. Update Purpose after archive.
## Requirements
### Requirement: SignalR hub groups connections by session ID

The system SHALL create a SignalR hub that groups connections by session ID. Teacher and Student connect with a session ID and join the corresponding group.

#### Scenario: Teacher joins session group
- **WHEN** a Teacher connects to the hub with a session ID
- **THEN** the Teacher is added to the session's SignalR group

#### Scenario: Student joins session group
- **WHEN** a Student connects to the hub with a session ID they are the participant of
- **THEN** the Student is added to the session's SignalR group and receives Student-safe payloads (no reference_context)

#### Scenario: Unauthorized user cannot join
- **WHEN** a user who is neither the session's Teacher nor Student attempts to join a session group
- **THEN** the connection is rejected

### Requirement: Teacher actions broadcast state changes

When the Teacher performs a session action (start, advance, end, mark answer reviewed), the system SHALL broadcast the resulting state to all group members via the hub.

#### Scenario: State broadcast on advance
- **WHEN** a Teacher advances the session
- **THEN** all group members receive the new current_question_id and session state

