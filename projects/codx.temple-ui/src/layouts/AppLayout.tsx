import { Outlet, Link, useNavigate } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";

export default function AppLayout() {
  const { user, isAuthenticated, roles, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  return (
    <div className="flex min-h-screen bg-gray-50 dark:bg-gray-950">
      <aside className="w-64 border-r border-gray-200 dark:border-gray-800 bg-white dark:bg-gray-900 p-4 flex flex-col">
        <Link to="/" className="text-lg font-semibold text-gray-900 dark:text-white mb-6">
          The Temple Courts
        </Link>
        <nav className="flex-1 space-y-1">
          <NavLink to="/">Home</NavLink>
          {isAuthenticated && <NavLink to="/lessons">Lessons</NavLink>}
          {roles.includes("Teacher") && <NavLink to="/teacher">Teacher</NavLink>}
          {roles.includes("Admin") && <NavLink to="/admin">Admin</NavLink>}
        </nav>
        {isAuthenticated && (
          <div className="border-t border-gray-200 dark:border-gray-800 pt-4">
            <p className="text-sm text-gray-600 dark:text-gray-400">{user?.displayName}</p>
            <button onClick={handleLogout} className="text-sm text-red-600 hover:underline mt-1">Sign out</button>
          </div>
        )}
      </aside>
      <main className="flex-1 p-6">
        <Outlet />
      </main>
    </div>
  );
}

function NavLink({ to, children }: { to: string; children: React.ReactNode }) {
  return (
    <Link to={to} className="block rounded-lg px-3 py-2 text-sm text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-800">
      {children}
    </Link>
  );
}
