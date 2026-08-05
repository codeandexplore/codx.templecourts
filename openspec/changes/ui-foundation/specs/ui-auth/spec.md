# UI Auth

## ADDED Requirements

### Requirement: User can register with email and password
The UI SHALL provide a registration form with email, display name, and password fields. Validation errors SHALL be displayed inline.

#### Scenario: Successful registration
- **WHEN** a user submits valid registration details
- **THEN** the user is authenticated and redirected to the home page

### Requirement: User can log in with email and password
The UI SHALL provide a login form with email and password. On success, JWT tokens are stored in Redux state (memory only).

#### Scenario: Successful login
- **WHEN** a user submits valid credentials
- **THEN** the user is authenticated and redirected to the home page

### Requirement: User can log in with Google
The UI SHALL provide a Google sign-in button that opens the OAuth popup and sends the id_token to the API.

### Requirement: Authenticated routes redirect to login
Routes requiring authentication SHALL redirect to /login when the user is not authenticated.

### Requirement: Role-based route guarding
Routes requiring specific roles (Admin, Teacher) SHALL show an unauthorized page when the user lacks the role.
