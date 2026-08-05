## Context

The UI needs its foundational layer before any feature pages. Follows workspace conventions: RTK Query + Axios, Redux Toolkit, react-hook-form + zod, Tailwind 4, co-located styles.

## Decisions

- **Router**: React Router v7 with createBrowserRouter, lazy-loaded routes
- **Auth**: JWT in memory only (Redux state), refresh token rotation, Google OAuth via popup
- **API**: RTK Query with Axios baseQuery, auto-inject Authorization header, 401 → refresh → retry
- **Forms**: react-hook-form + zod validation
- **Icons**: @heroicons/react (outline)
- **No CSS modules**: All styles via Tailwind utility classes in JSX
