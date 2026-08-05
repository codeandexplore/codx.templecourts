# UI API Client

## ADDED Requirements

### Requirement: API client handles JWT token injection
The API client SHALL automatically attach the Authorization Bearer header from the Redux auth state.

### Requirement: API client handles 401 with token refresh
On 401 response, the client SHALL attempt a token refresh. If refresh succeeds, retry the original request. If refresh fails, clear auth state and redirect to login.
