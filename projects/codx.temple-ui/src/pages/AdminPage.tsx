import { useState } from "react";
import { useListLessonsQuery, useCreateLessonMutation } from "../services/lessonsApi";
import { useListRoleAssignmentsQuery, useAssignRoleMutation } from "../services/adminApi";

export default function AdminPage() {
  const [tab, setTab] = useState<"lessons" | "roles">("lessons");

  return (
    <div>
      <h2 className="text-2xl font-semibold text-gray-900 dark:text-white mb-6">Admin Dashboard</h2>
      <div className="flex gap-2 mb-6">
        <TabButton active={tab === "lessons"} onClick={() => setTab("lessons")}>Lessons</TabButton>
        <TabButton active={tab === "roles"} onClick={() => setTab("roles")}>Roles</TabButton>
      </div>
      {tab === "lessons" ? <LessonsTab /> : <RolesTab />}
    </div>
  );
}

function TabButton({ active, onClick, children }: { active: boolean; onClick: () => void; children: string }) {
  return (
    <button onClick={onClick} className={`px-4 py-2 rounded-lg text-sm font-medium ${active ? "bg-blue-600 text-white" : "bg-gray-100 dark:bg-gray-800 text-gray-700 dark:text-gray-300 hover:bg-gray-200 dark:hover:bg-gray-700"}`}>
      {children}
    </button>
  );
}

function LessonsTab() {
  const { data: lessons, isLoading } = useListLessonsQuery();
  const [creating, setCreating] = useState(false);

  if (isLoading) return <p className="text-gray-600">Loading...</p>;

  return (
    <div>
      <div className="flex items-center justify-between mb-4">
        <h3 className="text-lg font-medium text-gray-900 dark:text-white">All Lessons</h3>
        <button onClick={() => setCreating(!creating)} className="rounded-lg bg-green-600 px-4 py-2 text-sm text-white font-medium hover:bg-green-700">
          {creating ? "Cancel" : "New Lesson"}
        </button>
      </div>
      {creating && <CreateLessonForm onDone={() => setCreating(false)} />}
      <div className="space-y-2">
        {lessons?.map((l) => (
          <div key={l.key} className="flex items-center justify-between rounded-lg border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-900 p-3">
            <div>
              <span className="text-xs text-gray-500">#{l.number}</span>
              <span className="ml-2 text-sm font-medium text-gray-900 dark:text-white">{l.title}</span>
              <span className={`ml-2 text-xs px-2 py-0.5 rounded ${l.status === "Active" ? "bg-green-100 text-green-700" : "bg-yellow-100 text-yellow-700"}`}>{l.status}</span>
            </div>
            <span className="text-xs text-gray-500">{l.currentPublishedVersionId ? "Published" : "Draft"}</span>
          </div>
        ))}
      </div>
    </div>
  );
}

function CreateLessonForm({ onDone }: { onDone: () => void }) {
  const [number, setNumber] = useState("");
  const [title, setTitle] = useState("");
  const [createLesson, { isLoading: loading, error: rawError }] = useCreateLessonMutation();
  const error = rawError && "data" in rawError ? (rawError.data as { error?: string })?.error : "";

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await createLesson({ number: parseInt(number), title }).unwrap();
      setNumber("");
      setTitle("");
      onDone();
    } catch { /* RTK Query handles error state */ }
  };

  return (
    <form onSubmit={handleSubmit} className="mb-4 p-4 rounded-lg border border-gray-200 dark:border-gray-700 bg-gray-50 dark:bg-gray-800 space-y-3">
      {error && <p className="text-sm text-red-600">{error}</p>}
      <div className="flex gap-3">
        <input value={number} onChange={e => setNumber(e.target.value)} type="number" placeholder="Lesson #" className="w-24 rounded-lg border px-3 py-2 text-sm dark:bg-gray-900 dark:text-white" required />
        <input value={title} onChange={e => setTitle(e.target.value)} placeholder="Lesson title" className="flex-1 rounded-lg border px-3 py-2 text-sm dark:bg-gray-900 dark:text-white" required />
        <button type="submit" disabled={loading} className="rounded-lg bg-blue-600 px-4 py-2 text-sm text-white font-medium hover:bg-blue-700 disabled:opacity-50">
          {loading ? "Creating..." : "Create"}
        </button>
      </div>
    </form>
  );
}

function RolesTab() {
  const { data: assignments, isLoading } = useListRoleAssignmentsQuery();
  const [assignRole] = useAssignRoleMutation();
  const [userId, setUserId] = useState("");
  const [role, setRole] = useState("Teacher");
  const [error, setError] = useState("");

  const handleAssign = async () => {
    setError("");
    try {
      await assignRole({ userId, role }).unwrap();
      setUserId("");
    } catch (e: unknown) {
      const err = e as { data?: { error?: string } };
      setError(err?.data?.error || "Failed");
    }
  };

  return (
    <div>
      <h3 className="text-lg font-medium text-gray-900 dark:text-white mb-4">Role Assignments</h3>
      <div className="mb-4 p-4 rounded-lg border border-gray-200 dark:border-gray-700 bg-gray-50 dark:bg-gray-800 space-y-2">
        <div className="flex gap-3">
          <input value={userId} onChange={e => setUserId(e.target.value)} placeholder="User ID (GUID)" className="flex-1 rounded-lg border px-3 py-2 text-sm dark:bg-gray-900 dark:text-white" />
          <select value={role} onChange={e => setRole(e.target.value)} className="rounded-lg border px-3 py-2 text-sm dark:bg-gray-900 dark:text-white">
            <option>Admin</option>
            <option>Teacher</option>
            <option>Student</option>
          </select>
          <button onClick={handleAssign} className="rounded-lg bg-blue-600 px-4 py-2 text-sm text-white font-medium hover:bg-blue-700">Assign</button>
        </div>
        {error && <p className="text-sm text-red-600">{error}</p>}
      </div>
      <div className="space-y-2">
        {isLoading ? <p className="text-gray-600">Loading...</p> : assignments?.map((a) => (
          <div key={a.id} className="flex items-center justify-between rounded-lg border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-900 p-3">
            <div>
              <span className="text-sm font-medium text-gray-900 dark:text-white">{a.userDisplayName || a.userEmail}</span>
              <span className="ml-2 text-xs text-gray-500">{a.userEmail}</span>
            </div>
            <span className="text-xs font-medium px-2 py-0.5 rounded bg-blue-100 text-blue-700">{a.role}</span>
          </div>
        ))}
      </div>
    </div>
  );
}
