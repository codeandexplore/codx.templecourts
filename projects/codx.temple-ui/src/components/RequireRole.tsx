import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";
import { Card, CardContent } from "../components/ui/card";
import { ShieldExclamationIcon } from "@heroicons/react/24/outline";

export default function RequireRole({ role }: { role: string }) {
  const { isAuthenticated, roles } = useAuth();
  if (!isAuthenticated) return <Navigate to="/login" replace />;
  if (!roles.includes(role)) {
    return (
      <div className="flex items-center justify-center py-16">
        <Card className="p-8 max-w-sm text-center">
          <CardContent className="flex flex-col items-center gap-3">
            <ShieldExclamationIcon className="size-10 text-parchment-300 dark:text-slate-600" />
            <p className="text-parchment-600 dark:text-slate-400">You do not have permission to view this page.</p>
          </CardContent>
        </Card>
      </div>
    );
  }
  return <Outlet />;
}
