import { Outlet, Link, useNavigate } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";
import { BookOpenIcon, AcademicCapIcon, UserGroupIcon, Cog6ToothIcon, HomeIcon, PencilSquareIcon, ClipboardDocumentListIcon, ChatBubbleLeftRightIcon } from "@heroicons/react/24/outline";

export default function AppLayout() {
  const { user, isAuthenticated, roles, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  return (
    <div className="flex min-h-screen bg-parchment-50 dark:bg-slate-950">
      <aside className="w-64 border-r border-parchment-200 dark:border-slate-800 bg-white dark:bg-slate-900 p-6 flex flex-col">
        <div>
          <Link to="/" className="font-serif text-2xl font-semibold text-parchment-900 dark:text-white tracking-tight">
            The Temple Courts
          </Link>
        </div>
        <div className="my-5 border-t border-parchment-200 dark:border-slate-800" />
        <nav className="flex-1 space-y-0.5">
          <NavLink to="/" icon={<HomeIcon className="size-5" />}>Home</NavLink>
          {isAuthenticated && <NavLink to="/lessons" icon={<BookOpenIcon className="size-5" />}>Lessons</NavLink>}
          {roles.includes("Teacher") && <NavLink to="/teacher" icon={<UserGroupIcon className="size-5" />}>Students</NavLink>}
          {roles.includes("Teacher") && <NavLink to="/teacher/check-questions" icon={<ChatBubbleLeftRightIcon className="size-5" />}>Check Questions</NavLink>}
          {roles.includes("Admin") && <NavLink to="/admin" icon={<Cog6ToothIcon className="size-5" />}>Admin</NavLink>}
          {roles.includes("Admin") && <NavLink to="/admin/editor" icon={<PencilSquareIcon className="size-5" />}>Lesson Editor</NavLink>}
          {roles.includes("Admin") && <NavLink to="/admin" icon={<ClipboardDocumentListIcon className="size-5" />}>Assignments</NavLink>}
        </nav>
        {isAuthenticated && (
          <div className="border-t border-parchment-200 dark:border-slate-800 pt-4">
            <div className="flex items-center gap-3 mb-3">
              <div className="size-8 rounded-full bg-cerulean-100 dark:bg-cerulean-900 flex items-center justify-center">
                <AcademicCapIcon className="size-4 text-cerulean-600 dark:text-cerulean-400" />
              </div>
              <p className="text-sm font-medium text-parchment-800 dark:text-slate-200 truncate">{user?.displayName}</p>
            </div>
            <button onClick={handleLogout} className="text-sm text-slate-500 dark:text-slate-400 hover:text-red-600 dark:hover:text-red-400 transition-colors">
              Sign out
            </button>
          </div>
        )}
      </aside>
      <main className="flex-1 p-8">
        <Outlet />
      </main>
    </div>
  );
}

function NavLink({ to, children, icon }: { to: string; children: string; icon: React.ReactNode }) {
  return (
    <Link to={to} className="flex items-center gap-3 rounded-xl px-3 py-2 text-sm text-parchment-700 dark:text-slate-300 hover:bg-parchment-100 dark:hover:bg-slate-800 hover:text-parchment-900 dark:hover:text-white transition-colors">
      {icon}
      {children}
    </Link>
  );
}
