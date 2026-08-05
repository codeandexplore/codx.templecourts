import { useCallback } from "react";
import { useAppDispatch, useAppSelector } from "../store/store";
import { setCredentials, logout as logoutAction } from "../store/authSlice";
import { useLoginMutation, useRegisterMutation } from "../services/authApi";

export function useAuth() {
  const dispatch = useAppDispatch();
  const { user, accessToken, refreshToken } = useAppSelector((s) => s.auth);
  const [loginMutation] = useLoginMutation();
  const [registerMutation] = useRegisterMutation();

  const login = useCallback(async (email: string, password: string) => {
    const result = await loginMutation({ email, password }).unwrap();
    dispatch(setCredentials({ user: result.user, accessToken: result.accessToken, refreshToken: result.refreshToken, expiresAt: result.expiresAt }));
    return result;
  }, [dispatch, loginMutation]);

  const register = useCallback(async (email: string, password: string, displayName: string) => {
    const result = await registerMutation({ email, password, displayName }).unwrap();
    dispatch(setCredentials({ user: result.user, accessToken: result.accessToken, refreshToken: result.refreshToken, expiresAt: result.expiresAt }));
    return result;
  }, [dispatch, registerMutation]);

  const logout = useCallback(() => {
    dispatch(logoutAction());
  }, [dispatch]);

  const isAuthenticated = !!accessToken;
  const roles = user?.roles || [];

  return { user, isAuthenticated, roles, login, register, logout, accessToken, refreshToken };
}
