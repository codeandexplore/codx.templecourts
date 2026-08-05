import { apiSlice } from "../store/apiSlice";

export interface LoginRequest { email: string; password: string; }
export interface RegisterRequest { email: string; password: string; displayName: string; }
export interface AuthResponse { accessToken: string; refreshToken: string; expiresAt: string; user: { id: string; email: string; displayName: string; roles: string[]; }; }

const authApi = apiSlice.injectEndpoints({
  endpoints: (builder) => ({
    login: builder.mutation<AuthResponse, LoginRequest>({
      query: (body) => ({ url: "/auth/login", method: "POST", body }),
    }),
    register: builder.mutation<AuthResponse, RegisterRequest>({
      query: (body) => ({ url: "/auth/register", method: "POST", body }),
    }),
  }),
});

export const { useLoginMutation, useRegisterMutation } = authApi;
