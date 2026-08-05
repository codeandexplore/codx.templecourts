import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";

export default function RequireRole({ role }: { role: string }) {
  const { isAuthenticated, roles } = useAuth();
  if (!isAuthenticated) return <Navigate to="/login" replace />;
  if (!roles.includes(role)) return <div className="p-8 text-center text-gray-600">You do not have permission to view this page.</div>;
  return <Outlet />;
}
