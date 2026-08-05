## Why

The UI is a bare Vite scaffold with 2 placeholder components. It needs the foundational layer: auth, routing, API client, Redux store — before any feature pages can be built.

## What Changes

- Add dependencies: react-router-dom, @reduxjs/toolkit, react-redux, axios, react-hook-form, zod, @hookform/resolvers, @heroicons/react, @headlessui/react
- Set up Redux store with auth slice + RTK Query API client
- Build auth pages: Login, Register with form validation
- Build protected route wrapper and role-based route guards
- Add API service layer with Axios baseQuery + JWT token injection + 401 refresh
- Add OIDC auth flow for Google login button
- Improve AppLayout with responsive sidebar navigation

## Capabilities

### New Capabilities
- `ui-auth`: Login, Register pages, auth state management, protected routes
- `ui-api-client`: RTK Query API client with token injection and 401 handling

## Impact

- **UI project**: New dependencies, Redux store, API client, auth pages, router restructure
