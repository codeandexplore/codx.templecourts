# ui-api-client Specification

## Purpose
TBD - created by archiving change ui-foundation. Update Purpose after archive.
## Requirements
### Requirement: API client handles JWT token injection
The API client SHALL automatically attach the Authorization Bearer header from the Redux auth state.

#### Scenario: Token injection
- **WHEN** an authenticated request is made
- **THEN** the Authorization Bearer header is present

### Requirement: API client handles 401 with token refresh
On 401 response, the client SHALL attempt a token refresh. If refresh succeeds, retry the original request. If refresh fails, clear auth state and redirect to login.

#### Scenario: Token refresh success
- **WHEN** a request returns 401 and a valid refresh token exists
- **THEN** the token is refreshed and the original request is retried

#### Scenario: Token refresh failure
- **WHEN** a request returns 401 and the refresh fails
- **THEN** the auth state is cleared and the user is redirected to login

