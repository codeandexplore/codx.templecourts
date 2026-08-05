import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import type { RootState } from "./store";
import { setTokens, logout } from "./authSlice";

const baseQuery = fetchBaseQuery({
  baseUrl: import.meta.env.VITE_API_URL || "http://localhost:5000",
  prepareHeaders: (headers, { getState }) => {
    const token = (getState() as RootState).auth.accessToken;
    if (token) {
      headers.set("authorization", `Bearer ${token}`);
    }
    return headers;
  },
});

const baseQueryWithReauth: typeof baseQuery = async (args, api, extraOptions) => {
  let result = await baseQuery(args, api, extraOptions);

  if (result.error && result.error.status === 401) {
    const refreshToken = (api.getState() as RootState).auth.refreshToken;
    if (refreshToken) {
      const refreshResult = await baseQuery(
        { url: "/auth/refresh", method: "POST", body: { refreshToken } },
        api,
        extraOptions,
      );
      if (refreshResult.data) {
        const data = refreshResult.data as { accessToken: string; refreshToken: string; expiresAt: string };
        api.dispatch(setTokens(data));
        result = await baseQuery(args, api, extraOptions);
      } else {
        api.dispatch(logout());
      }
    } else {
      api.dispatch(logout());
    }
  }
  return result;
};

export const apiSlice = createApi({
  baseQuery: baseQueryWithReauth,
  tagTypes: ["Lessons", "Attempts", "Notes"],
  endpoints: () => ({}),
});
