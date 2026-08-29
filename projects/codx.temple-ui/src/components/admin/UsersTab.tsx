import { useState, useMemo } from "react";
import { useListUsersQuery, useResetUserPasswordMutation, useUpdateUserStatusMutation } from "../../services/adminApi";
import { Card, CardContent } from "../ui/card";
import { Badge } from "../ui/badge";
import { Button } from "../ui/button";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "../ui/dialog";
import { UserIcon, KeyIcon, XCircleIcon, CheckCircleIcon } from "@heroicons/react/24/outline";

export default function UsersTab() {
  const { data: users, isLoading, isError } = useListUsersQuery();
  const [search, setSearch] = useState("");

  const filtered = useMemo(() => {
    if (!users) return [];
    if (!search.trim()) return users;
    const q = search.toLowerCase();
    return users.filter(
      (u) => u.email.toLowerCase().includes(q) || u.displayName.toLowerCase().includes(q)
    );
  }, [users, search]);

  if (isLoading) {
    return (
      <div className="py-8 text-center">
        <div className="size-8 mx-auto mb-3 rounded-full border-2 border-cerulean-500 border-t-transparent animate-spin" />
        <p className="text-parchment-500 dark:text-slate-400 text-sm">Loading users...</p>
      </div>
    );
  }

  if (isError) {
    return <div className="py-8 text-center"><p className="text-red-600 text-sm">Failed to load users.</p></div>;
  }

  return (
    <div className="p-5 pt-4">
      <div className="flex items-center justify-between mb-4">
        <h3 className="font-serif text-lg font-medium text-parchment-900 dark:text-white">
          Users
          {users && users.length > 0 && <Badge variant="secondary" className="ml-3 align-middle">{users.length}</Badge>}
        </h3>
      </div>

      <div className="mb-4">
        <input
          type="text"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          placeholder="Search by name or email..."
          className="w-full rounded-lg border border-parchment-200 dark:border-slate-700 bg-white dark:bg-slate-900 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-cerulean-500 dark:text-white"
        />
      </div>

      {filtered.length === 0 ? (
        <div className="py-12 text-center">
          <div className="size-14 mx-auto mb-4 rounded-xl bg-parchment-100 dark:bg-slate-800 flex items-center justify-center text-parchment-300 dark:text-slate-600">
            <UserIcon className="size-8" />
          </div>
          <h3 className="font-serif text-lg font-medium text-parchment-700 dark:text-slate-300 mb-1">No users found</h3>
          <p className="text-sm text-parchment-400 dark:text-slate-500">Try a different search.</p>
        </div>
      ) : (
        <div className="space-y-2">
          {filtered.map((u) => (
            <Card key={u.id} className="p-4 hover:shadow-sm transition-all">
              <CardContent>
                <div className="flex items-center justify-between gap-3">
                  <div className="min-w-0">
                    <div className="flex items-center gap-2 mb-1">
                      <span className="text-sm font-medium text-parchment-900 dark:text-white truncate">
                        {u.displayName || u.email}
                      </span>
                      <Badge variant={u.status === "Active" ? "success" : "secondary"} className="text-[10px]">
                        {u.status}
                      </Badge>
                    </div>
                    <span className="text-xs text-parchment-500 dark:text-slate-400">{u.email}</span>
                    <div className="flex gap-1 mt-1.5 flex-wrap">
                      {u.roles.length === 0 ? (
                        <span className="text-xs text-parchment-400 dark:text-slate-500">No roles</span>
                      ) : (
                        u.roles.map((r) => (
                          <Badge key={r} variant={r === "Admin" ? "default" : r === "Teacher" ? "warning" : "secondary"} className="text-[9px]">
                            {r}
                          </Badge>
                        ))
                      )}
                    </div>
                  </div>
                  <div className="flex items-center gap-2 shrink-0">
                    <ResetPasswordDialog userId={u.id} />
                    <StatusToggle userId={u.id} currentStatus={u.status} />
                  </div>
                </div>
              </CardContent>
            </Card>
          ))}
        </div>
      )}
    </div>
  );
}

function ResetPasswordDialog({ userId }: { userId: string }) {
  const [resetPassword, { isLoading: resetting }] = useResetUserPasswordMutation();
  const [open, setOpen] = useState(false);
  const [newPassword, setNewPassword] = useState("");
  const [error, setError] = useState("");
  const [done, setDone] = useState(false);

  const handleReset = async () => {
    if (newPassword.length < 8) {
      setError("Password must be at least 8 characters.");
      return;
    }
    setError("");
    try {
      await resetPassword({ userId, newPassword }).unwrap();
      setDone(true);
    } catch (e: unknown) {
      const err = e as { data?: { error?: string } };
      setError(err?.data?.error || "Failed to reset password");
    }
  };

  const close = () => {
    setOpen(false);
    setNewPassword("");
    setError("");
    setDone(false);
  };

  return (
    <Dialog open={open} onOpenChange={(o) => (o ? setOpen(true) : close())}>
      <Button variant="ghost" size="sm" onClick={() => setOpen(true)}>
        <KeyIcon className="size-3.5 mr-1" />
        Reset Password
      </Button>
      <DialogContent>
        <DialogHeader>
          <DialogTitle className="font-serif">Reset Password</DialogTitle>
        </DialogHeader>
        {done ? (
          <p className="text-sm text-emerald-600 dark:text-emerald-400 bg-emerald-50 dark:bg-emerald-900/20 rounded-lg p-3">
            Password reset successfully.
          </p>
        ) : (
          <div className="flex flex-col gap-3">
            <p className="text-sm text-parchment-600 dark:text-slate-400">Set a new password for this user.</p>
            {error && <p className="text-sm text-red-600 bg-red-50 dark:bg-red-900/20 rounded-lg p-2">{error}</p>}
            <div className="flex flex-col gap-1.5">
              <label htmlFor="reset-password" className="text-sm font-medium text-parchment-700 dark:text-slate-300">New Password</label>
              <input
                id="reset-password"
                type="password"
                value={newPassword}
                onChange={(e) => setNewPassword(e.target.value)}
                placeholder="Minimum 8 characters"
                className="w-full rounded-lg border border-parchment-200 dark:border-slate-700 bg-white dark:bg-slate-900 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-cerulean-500 dark:text-white"
              />
            </div>
            <div className="flex gap-2 justify-end pt-2">
              <Button variant="ghost" onClick={close}>Cancel</Button>
              <Button onClick={handleReset} disabled={resetting} className="bg-cerulean-600 hover:bg-cerulean-700 text-white">
                {resetting ? "Resetting..." : "Reset"}
              </Button>
            </div>
          </div>
        )}
      </DialogContent>
    </Dialog>
  );
}

function StatusToggle({ userId, currentStatus }: { userId: string; currentStatus: string }) {
  const [updateStatus, { isLoading: updating }] = useUpdateUserStatusMutation();
  const isActive = currentStatus === "Active";

  const handleToggle = async () => {
    await updateStatus({ userId, status: isActive ? "Inactive" : "Active" }).unwrap();
  };

  return (
    <Button
      variant="ghost"
      size="sm"
      onClick={handleToggle}
      disabled={updating}
      className={isActive ? "text-red-600 dark:text-red-400" : "text-emerald-600 dark:text-emerald-400"}
    >
      {isActive ? <XCircleIcon className="size-3.5 mr-1" /> : <CheckCircleIcon className="size-3.5 mr-1" />}
      {isActive ? "Suspend" : "Activate"}
    </Button>
  );
}
