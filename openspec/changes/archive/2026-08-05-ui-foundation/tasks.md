## 1. Dependencies

- [ ] 1.1 Install react-router-dom, @reduxjs/toolkit, react-redux, axios
- [ ] 1.2 Install react-hook-form, zod, @hookform/resolvers
- [ ] 1.3 Install @heroicons/react, @headlessui/react

## 2. Redux Store

- [ ] 2.1 Create auth slice (user, tokens, login/logout/refresh actions)
- [ ] 2.2 Create RTK Query API client with Axios baseQuery + token injection + 401 refresh
- [ ] 2.3 Create Redux Provider wrapper in main.tsx

## 3. Auth Pages

- [ ] 3.1 Create Login page with react-hook-form + zod validation
- [ ] 3.2 Create Register page with form validation
- [ ] 3.3 Create useAuth hook (login, register, logout, refresh)
- [ ] 3.4 Wire up Google login button

## 4. Router

- [ ] 4.1 Create ProtectedRoute component (redirect to /login)
- [ ] 4.2 Create RequireRole component (show unauthorized for wrong role)
- [ ] 4.3 Set up lazy-loaded routes: /, /login, /register, /lessons, /lessons/:key

## 5. Layout

- [ ] 5.1 Improve AppLayout with sidebar nav (responsive)
- [ ] 5.2 Add user menu (display name, logout)

## 6. Polish

- [ ] 6.1 pnpm lint, type-check, test — all pass
