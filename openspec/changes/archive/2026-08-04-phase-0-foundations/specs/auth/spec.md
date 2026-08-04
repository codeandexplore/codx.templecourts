## ADDED Requirements

### Requirement: User can register with email and password

The system SHALL allow a new user to register with an email address and password. The password SHALL be hashed using BCrypt before storage. The email SHALL be unique across all users. Upon successful registration, the system SHALL return a JWT access token and a refresh token.

#### Scenario: Successful registration
- **WHEN** a client sends a POST request to `/auth/register` with a valid email, display name, and password (min 8 characters)
- **THEN** the system creates a User with a BCrypt-hashed password, issues a JWT access token (15-minute expiry) and a refresh token (7-day expiry), and returns both tokens plus the user profile

#### Scenario: Duplicate email
- **WHEN** a client sends a POST request to `/auth/register` with an email already in use
- **THEN** the system returns 409 Conflict with a message indicating the email is already registered

#### Scenario: Weak password
- **WHEN** a client sends a POST request to `/auth/register` with a password shorter than 8 characters
- **THEN** the system returns 422 Unprocessable Entity with a validation error

### Requirement: User can log in with email and password

The system SHALL authenticate a user by email and password. The system SHALL verify the password against the stored BCrypt hash. Upon successful authentication, the system SHALL return a new JWT access token and refresh token, rotating the refresh token.

#### Scenario: Successful login
- **WHEN** a client sends a POST request to `/auth/login` with a registered email and correct password
- **THEN** the system returns a JWT access token and a new refresh token, invalidating any previous refresh token for that user

#### Scenario: Invalid credentials
- **WHEN** a client sends a POST request to `/auth/login` with an incorrect password or unregistered email
- **THEN** the system returns 401 Unauthorized with a generic "invalid credentials" message (no distinction between wrong email vs wrong password)

### Requirement: User can log in with Google OAuth

The system SHALL accept a Google `id_token` and authenticate or register the user. If a User with the matching `google_id` exists, the system SHALL authenticate that user. If a User with the matching email exists but no `google_id`, the system SHALL link the Google account by setting `google_id`. If no matching user exists, the system SHALL create a new User with `google_id` set.

#### Scenario: New Google user
- **WHEN** a client sends a POST request to `/auth/google` with a valid Google `id_token` for an email not in the system
- **THEN** the system creates a new User with `google_id` set, `password_hash` null, display name from Google, and returns JWT access + refresh tokens

#### Scenario: Returning Google user
- **WHEN** a client sends a POST request to `/auth/google` with a valid Google `id_token` for an email already linked to a `google_id`
- **THEN** the system authenticates the existing user and returns JWT access + refresh tokens

#### Scenario: Email match but no google_id (account linking)
- **WHEN** a client sends a POST request to `/auth/google` with a valid Google `id_token` for an email that has an existing password-based account (no `google_id`)
- **THEN** the system links the Google account by setting the user's `google_id`, authenticates the user, and returns JWT access + refresh tokens

#### Scenario: Invalid Google token
- **WHEN** a client sends a POST request to `/auth/google` with an invalid or expired `id_token`
- **THEN** the system returns 401 Unauthorized

### Requirement: User can refresh an access token

The system SHALL accept a refresh token and issue a new access + refresh token pair. The old refresh token SHALL be invalidated (rotation). Refresh tokens that are expired or do not match the stored hash SHALL be rejected.

#### Scenario: Successful token refresh
- **WHEN** a client sends a POST request to `/auth/refresh` with a valid, non-expired refresh token
- **THEN** the system returns a new JWT access token and a new refresh token, invalidating the previous refresh token

#### Scenario: Expired refresh token
- **WHEN** a client sends a POST request to `/auth/refresh` with an expired refresh token
- **THEN** the system returns 401 Unauthorized

#### Scenario: Invalid refresh token
- **WHEN** a client sends a POST request to `/auth/refresh` with a refresh token that does not match any stored hash
- **THEN** the system returns 401 Unauthorized

### Requirement: JWT Bearer authentication protects endpoints

The system SHALL validate JWT Bearer tokens on every request to protected endpoints. Requests without a valid token SHALL be rejected with 401 Unauthorized. The token SHALL include `sub`, `email`, and `application_role` claims.

#### Scenario: Authenticated request
- **WHEN** a client sends a request with a valid JWT Bearer token in the Authorization header
- **THEN** the system sets `HttpContext.User` with the claims from the token and allows the request to proceed

#### Scenario: Missing token
- **WHEN** a client sends a request to a protected endpoint without an Authorization header
- **THEN** the system returns 401 Unauthorized

#### Scenario: Expired token
- **WHEN** a client sends a request with an expired JWT Bearer token
- **THEN** the system returns 401 Unauthorized
