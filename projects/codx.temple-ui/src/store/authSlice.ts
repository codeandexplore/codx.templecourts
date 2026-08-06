import { createSlice, type PayloadAction } from "@reduxjs/toolkit";

export interface AuthUser {
  id: string;
  email: string;
  displayName: string;
  roles: string[];
}

interface AuthState {
  user: AuthUser | null;
  accessToken: string | null;
  refreshToken: string | null;
  expiresAt: string | null;
}

const AUTH_KEY = "templecourts_auth";

function loadAuth(): AuthState {
  try {
    const raw = localStorage.getItem(AUTH_KEY);
    if (raw) {
      const parsed = JSON.parse(raw);
      if (parsed.accessToken && parsed.user) {
        return parsed;
      }
    }
  } catch { /* ignore corrupt storage */ }
  return { user: null, accessToken: null, refreshToken: null, expiresAt: null };
}

function saveAuth(state: AuthState) {
  try {
    localStorage.setItem(AUTH_KEY, JSON.stringify(state));
  } catch { /* ignore quota errors */ }
}

function clearAuth() {
  try {
    localStorage.removeItem(AUTH_KEY);
  } catch { /* ignore */ }
}

const initialState: AuthState = loadAuth();

const authSlice = createSlice({
  name: "auth",
  initialState,
  reducers: {
    setCredentials(state, action: PayloadAction<{ user: AuthUser; accessToken: string; refreshToken: string; expiresAt: string }>) {
      state.user = action.payload.user;
      state.accessToken = action.payload.accessToken;
      state.refreshToken = action.payload.refreshToken;
      state.expiresAt = action.payload.expiresAt;
      saveAuth(state);
    },
    setTokens(state, action: PayloadAction<{ accessToken: string; refreshToken: string; expiresAt: string }>) {
      state.accessToken = action.payload.accessToken;
      state.refreshToken = action.payload.refreshToken;
      state.expiresAt = action.payload.expiresAt;
      saveAuth(state);
    },
    logout(state) {
      state.user = null;
      state.accessToken = null;
      state.refreshToken = null;
      state.expiresAt = null;
      clearAuth();
    },
  },
});

export const { setCredentials, setTokens, logout } = authSlice.actions;
export default authSlice.reducer;
