import { useState, useRef, useEffect } from "react";
import { useListLessonsQuery, useCreateLessonMutation } from "../services/lessonsApi";
import { useListRoleAssignmentsQuery, useAssignRoleMutation, useGetAssignmentsQuery, useListUsersQuery, useRevokeRoleMutation } from "../services/adminApi";
import { BookOpenIcon, PlusIcon, ShieldCheckIcon, ClipboardDocumentListIcon, UserIcon } from "@heroicons/react/24/outline";
import { Button } from "../components/ui/button";
import { Input } from "../components/ui/input";
import { Card, CardContent } from "../components/ui/card";
import { Badge } from "../components/ui/badge";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "../components/ui/tabs";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from "../components/ui/dialog";
import AssignmentsTab from "../components/admin/AssignmentsTab";
import UsersTab from "../components/admin/UsersTab";

export default function AdminPage() {
  return (
    <div>
      <div className="flex items-center gap-4 mb-8">
        <div className="size-10 rounded-xl bg-cerulean-100 dark:bg-cerulean-900/30 flex items-center justify-center shrink-0">
          <ShieldCheckIcon className="size-5 text-cerulean-600 dark:text-cerulean-400" />
        </div>
        <div>
          <h2 className="font-serif text-2xl font-semibold text-parchment-900 dark:text-white">Administration</h2>
          <p className="text-sm text-parchment-500 dark:text-slate-400">Manage lessons, roles, and assignments</p>
        </div>
      </div>

      <QuickStats />

      <Tabs defaultValue="lessons" className="w-full">
        <Card className="w-full">
          <div className="p-5 pb-3 border-b border-parchment-100 dark:border-slate-800">
            <TabsList className="mb-0">
              <TabsTrigger value="lessons">
                <BookOpenIcon className="size-4 mr-1.5" />
                Lessons
              </TabsTrigger>
              <TabsTrigger value="roles">
                <ShieldCheckIcon className="size-4 mr-1.5" />
                Roles
              </TabsTrigger>
              <TabsTrigger value="assignments">
                <ClipboardDocumentListIcon className="size-4 mr-1.5" />
                Assignments
              </TabsTrigger>
              <TabsTrigger value="users">
                <UserIcon className="size-4 mr-1.5" />
                Users
              </TabsTrigger>
            </TabsList>
          </div>
          <TabsContent value="lessons">
            <LessonsContent />
          </TabsContent>
          <TabsContent value="roles">
            <RolesContent />
          </TabsContent>
          <TabsContent value="assignments">
            <AssignmentsTab />
          </TabsContent>
          <TabsContent value="users">
            <UsersTab />
          </TabsContent>
        </Card>
      </Tabs>
    </div>
  );
}

function QuickStats() {
  const { data: lessons } = useListLessonsQuery();
  const { data: roles } = useListRoleAssignmentsQuery();
  const { data: assignments } = useGetAssignmentsQuery(undefined);

  const stats = [
    { label: "Lessons", value: lessons?.length ?? 0, icon: BookOpenIcon },
    { label: "Roles", value: roles?.length ?? 0, icon: ShieldCheckIcon },
    { label: "Assignments", value: assignments?.length ?? 0, icon: ClipboardDocumentListIcon },
  ];

  return (
    <div className="grid grid-cols-3 gap-4 mb-8">
      {stats.map((s) => (
        <Card key={s.label} className="p-4">
          <CardContent className="flex items-center gap-3">
            <div className="size-10 rounded-lg bg-parchment-100 dark:bg-slate-800 flex items-center justify-center shrink-0">
              <s.icon className="size-5 text-parchment-500 dark:text-slate-400" />
            </div>
            <div>
              <p className="text-2xl font-semibold text-parchment-900 dark:text-white tabular-nums">{s.value}</p>
              <p className="text-xs text-parchment-500 dark:text-slate-400">{s.label}</p>
            </div>
          </CardContent>
        </Card>
      ))}
    </div>
  );
}

function LessonsContent() {
  const { data: lessons, isLoading } = useListLessonsQuery();
  const [open, setOpen] = useState(false);
  const [success, setSuccess] = useState("");
  const listRef = useRef<HTMLDivElement>(null);
  const prevCount = useRef(lessons?.length ?? 0);

  useEffect(() => {
    if (lessons && lessons.length > prevCount.current) {
      setTimeout(() => {
        listRef.current?.lastElementChild?.scrollIntoView({ behavior: "smooth", block: "nearest" });
      }, 100);
    }
    prevCount.current = lessons?.length ?? 0;
  }, [lessons]);

  const handleCreated = (title: string) => {
    setOpen(false);
    setSuccess(`Lesson "${title}" created successfully.`);
    setTimeout(() => setSuccess(""), 3500);
  };

  if (isLoading) {
    return (
      <div className="py-8 text-center">
        <div className="size-8 mx-auto mb-3 rounded-full border-2 border-cerulean-500 border-t-transparent animate-spin" />
        <p className="text-parchment-500 dark:text-slate-400 text-sm">Loading lessons...</p>
      </div>
    );
  }

  if (!lessons || lessons.length === 0) {
    return (
      <div className="p-5 pt-4">
        <EmptyState
          icon={<BookOpenIcon className="size-8" />}
          title="No lessons yet"
          description="Create your first lesson to start building content."
          action={
            <Dialog open={open} onOpenChange={setOpen}>
              <DialogTrigger asChild>
                <Button className="bg-cerulean-600 hover:bg-cerulean-700 text-white mt-3">
                  <PlusIcon className="size-4 mr-1.5" />
                  New Lesson
                </Button>
              </DialogTrigger>
              <DialogContent>
                <DialogHeader>
                  <DialogTitle className="font-serif">Create Lesson</DialogTitle>
                </DialogHeader>
                <CreateLessonForm onDone={handleCreated} onCancel={() => setOpen(false)} />
              </DialogContent>
            </Dialog>
          }
        />
      </div>
    );
  }

  const publishedCount = lessons.filter((l) => l.currentPublishedVersionId).length;

  return (
    <div className="p-5 pt-4">
      <div className="flex items-center justify-between mb-4">
        <div>
          <h3 className="font-serif text-lg font-medium text-parchment-900 dark:text-white">
            All Lessons
            <Badge variant="secondary" className="ml-3 align-middle">{lessons.length}</Badge>
          </h3>
          <p className="text-xs text-parchment-400 dark:text-slate-500 mt-0.5">
            {publishedCount} published · {lessons.length - publishedCount} draft
          </p>
        </div>
        <Dialog open={open} onOpenChange={setOpen}>
          <DialogTrigger asChild>
            <Button className="bg-cerulean-600 hover:bg-cerulean-700 text-white">
              <PlusIcon className="size-4 mr-1.5" />
              New Lesson
            </Button>
          </DialogTrigger>
          <DialogContent>
            <DialogHeader>
              <DialogTitle className="font-serif">Create Lesson</DialogTitle>
            </DialogHeader>
            <CreateLessonForm onDone={handleCreated} onCancel={() => setOpen(false)} />
          </DialogContent>
        </Dialog>
      </div>
      {success && (
        <p className="text-sm text-emerald-600 dark:text-emerald-400 bg-emerald-50 dark:bg-emerald-900/20 rounded-lg px-4 py-3 mb-4 flex items-center gap-2">
          <span className="text-base">&#x2713;</span> {success}
        </p>
      )}
      <div className="space-y-2" ref={listRef}>
        {lessons.map((l) => (
          <Card key={l.key} className="p-4 hover:border-cerulean-200 dark:hover:border-cerulean-700 hover:shadow-sm transition-all cursor-pointer">
            <CardContent className="flex items-center justify-between">
              <div className="flex items-center gap-4 min-w-0">
                <span className="text-xs font-mono text-parchment-400 dark:text-slate-500 w-8 shrink-0 tabular-nums">#{l.number}</span>
                <span className="text-sm font-medium text-parchment-900 dark:text-white truncate">{l.title}</span>
                <Badge variant={l.status === "Active" ? "success" : "warning"} className="shrink-0 text-[10px]">{l.status}</Badge>
              </div>
              <Badge variant={l.currentPublishedVersionId ? "success" : "secondary"} className="shrink-0 ml-3 text-[10px]">
                {l.currentPublishedVersionId ? "Published" : "Draft"}
              </Badge>
            </CardContent>
          </Card>
        ))}
      </div>
    </div>
  );
}

function CreateLessonForm({ onDone, onCancel }: { onDone: (title: string) => void; onCancel: () => void }) {
  const [number, setNumber] = useState("");
  const [title, setTitle] = useState("");
  const [createLesson, { isLoading: loading, error: rawError }] = useCreateLessonMutation();
  const error = rawError && "data" in rawError ? (rawError.data as { error?: string })?.error : "";

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await createLesson({ number: parseInt(number), title }).unwrap();
      onDone(title);
      setNumber("");
      setTitle("");
    } catch { /* RTK Query handles error state */ }
  };

  return (
    <form onSubmit={handleSubmit} className="flex flex-col gap-4">
      {error && <p className="text-sm text-red-600 bg-red-50 dark:bg-red-900/20 rounded-lg p-2">{error}</p>}
      <div className="flex flex-col gap-1.5">
        <label htmlFor="lesson-number" className="text-sm font-medium text-parchment-700 dark:text-slate-300">Lesson Number</label>
        <Input id="lesson-number" value={number} onChange={e => setNumber(e.target.value)} type="number" placeholder="e.g. 17" required />
      </div>
      <div className="flex flex-col gap-1.5">
        <label htmlFor="lesson-title" className="text-sm font-medium text-parchment-700 dark:text-slate-300">Title</label>
        <Input id="lesson-title" value={title} onChange={e => setTitle(e.target.value)} placeholder="Lesson title" required />
      </div>
      <div className="flex gap-2 justify-end pt-2">
        <Button type="button" variant="ghost" onClick={onCancel}>Cancel</Button>
        <Button type="submit" disabled={loading} className="bg-cerulean-600 hover:bg-cerulean-700 text-white">
          {loading ? "Creating..." : "Create"}
        </Button>
      </div>
    </form>
  );
}

function RolesContent() {
  const { data: assignments, isLoading } = useListRoleAssignmentsQuery();
  const { data: users } = useListUsersQuery();
  const [assignRole] = useAssignRoleMutation();
  const [revokeRole] = useRevokeRoleMutation();
  const [userId, setUserId] = useState("");
  const [userSearch, setUserSearch] = useState("");
  const [userDropdownOpen, setUserDropdownOpen] = useState(false);
  const [role, setRole] = useState("Teacher");
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  const selectedUser = users?.find((u) => u.id === userId);

  const filteredUsers = (users || []).filter((u) =>
    !userSearch ||
    u.email.toLowerCase().includes(userSearch.toLowerCase()) ||
    u.displayName.toLowerCase().includes(userSearch.toLowerCase())
  );

  const handleAssign = async () => {
    setError("");
    setSuccess("");
    try {
      await assignRole({ userId, role }).unwrap();
      setUserId("");
      setUserSearch("");
      setSuccess("Role assigned successfully.");
      setTimeout(() => setSuccess(""), 3500);
    } catch (e: unknown) {
      const err = e as { data?: { error?: string } };
      setError(err?.data?.error || "Failed");
    }
  };

  if (isLoading) {
    return (
      <div className="py-8 text-center">
        <div className="size-8 mx-auto mb-3 rounded-full border-2 border-cerulean-500 border-t-transparent animate-spin" />
        <p className="text-parchment-500 dark:text-slate-400 text-sm">Loading role assignments...</p>
      </div>
    );
  }

  return (
    <div className="p-5 pt-4">
      <h3 className="font-serif text-lg font-medium text-parchment-900 dark:text-white mb-4">
        Role Assignments
        {assignments && assignments.length > 0 && (
          <Badge variant="secondary" className="ml-3 align-middle">{assignments.length}</Badge>
        )}
      </h3>
      <div className="rounded-xl border border-parchment-100 dark:border-slate-800 bg-parchment-50/50 dark:bg-slate-900/30 p-4 mb-4">
        <p className="text-xs text-parchment-400 dark:text-slate-500 mb-3">Select a user and assign a role.</p>
        <div className="flex gap-3">
          <div className="flex-1 flex flex-col gap-1.5">
            <label htmlFor="role-user-id" className="text-xs font-medium text-parchment-700 dark:text-slate-300">User</label>
            <div className="relative">
              <input
                type="text"
                value={selectedUser ? `${selectedUser.displayName} (${selectedUser.email})` : userSearch}
                onChange={(e) => {
                  setUserSearch(e.target.value);
                  setUserId("");
                  setUserDropdownOpen(true);
                }}
                onFocus={() => setUserDropdownOpen(true)}
                onBlur={() => setTimeout(() => setUserDropdownOpen(false), 150)}
                placeholder="Search by name or email..."
                className="w-full rounded-lg border border-parchment-200 dark:border-slate-700 bg-white dark:bg-slate-900 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-cerulean-500 dark:text-white"
              />
              {userDropdownOpen && filteredUsers.length > 0 && (
                <div className="absolute z-50 mt-1 w-full rounded-lg border border-parchment-200 dark:border-slate-700 bg-white dark:bg-slate-900 shadow-lg max-h-48 overflow-y-auto">
                  {filteredUsers.map((u) => (
                    <button
                      key={u.id}
                      type="button"
                      onMouseDown={(e) => e.preventDefault()}
                      onClick={() => {
                        setUserId(u.id);
                        setUserSearch("");
                        setUserDropdownOpen(false);
                      }}
                      className={`w-full text-left px-3 py-2 text-sm hover:bg-parchment-50 dark:hover:bg-slate-800 ${
                        userId === u.id ? "bg-cerulean-50 dark:bg-cerulean-900/20" : ""
                      }`}
                    >
                      <span className="font-medium text-parchment-900 dark:text-white">{u.displayName}</span>
                      <span className="ml-2 text-xs text-parchment-400 dark:text-slate-500">{u.email}</span>
                    </button>
                  ))}
                </div>
              )}
            </div>
          </div>
          <div className="w-32 flex flex-col gap-1.5">
            <label htmlFor="role-select" className="text-xs font-medium text-parchment-700 dark:text-slate-300">Role</label>
            <select id="role-select" value={role} onChange={e => setRole(e.target.value)}
              className="rounded-lg border border-parchment-200 dark:border-slate-700 bg-white dark:bg-slate-900 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-cerulean-500 dark:text-white">
              <option>Admin</option>
              <option>Teacher</option>
              <option>Student</option>
            </select>
          </div>
          <div className="flex items-end pb-px">
            <Button onClick={handleAssign} disabled={!userId} className="bg-cerulean-600 hover:bg-cerulean-700 text-white">Assign</Button>
          </div>
        </div>
        {error && <p className="text-sm text-red-600 bg-red-50 dark:bg-red-900/20 rounded-lg p-2 mt-3">{error}</p>}
        {success && (
          <p className="text-sm text-emerald-600 dark:text-emerald-400 bg-emerald-50 dark:bg-emerald-900/20 rounded-lg p-2 mt-3 flex items-center gap-1.5">
            <span className="text-base">&#x2713;</span> {success}
          </p>
        )}
      </div>
      {!assignments || assignments.length === 0 ? (
        <EmptyState icon={<ShieldCheckIcon className="size-8" />} title="No role assignments" description="Assign roles to users to get started." />
      ) : (
        <div className="space-y-2">
          {assignments.map((a) => (
            <Card key={a.id} className="p-4 hover:shadow-sm transition-all">
              <CardContent className="flex items-center justify-between">
                <div className="min-w-0">
                  <span className="text-sm font-medium text-parchment-900 dark:text-white">{a.userDisplayName || a.userEmail}</span>
                  <span className="ml-3 text-xs text-parchment-500 dark:text-slate-400 hidden sm:inline">{a.userEmail}</span>
                </div>
                <div className="flex items-center gap-2 shrink-0">
                  <Badge variant={a.role === "Admin" ? "default" : a.role === "Teacher" ? "warning" : "secondary"} className="text-[10px]">
                    {a.role}
                  </Badge>
                  <button
                    onClick={() => revokeRole(a.id)}
                    className="p-1 rounded text-parchment-400 dark:text-slate-500 hover:text-red-600 dark:hover:text-red-400 hover:bg-red-50 dark:hover:bg-red-900/20 transition-colors"
                    title="Revoke role"
                  >
                    <svg className="size-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
                      <path strokeLinecap="round" strokeLinejoin="round" d="M6 18L18 6M6 6l12 12" />
                    </svg>
                  </button>
                </div>
              </CardContent>
            </Card>
          ))}
        </div>
      )}
    </div>
  );
}

function EmptyState({ icon, title, description, action }: { icon: React.ReactNode; title: string; description: string; action?: React.ReactNode }) {
  return (
    <div className="py-12 text-center">
      <div className="size-14 mx-auto mb-4 rounded-xl bg-parchment-100 dark:bg-slate-800 flex items-center justify-center text-parchment-300 dark:text-slate-600">
        {icon}
      </div>
      <h3 className="font-serif text-lg font-medium text-parchment-700 dark:text-slate-300 mb-1">{title}</h3>
      <p className="text-sm text-parchment-400 dark:text-slate-500">{description}</p>
      {action}
    </div>
  );
}
