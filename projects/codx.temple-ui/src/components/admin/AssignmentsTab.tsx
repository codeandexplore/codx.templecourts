import { useState } from "react";
import { useGetAssignmentsQuery, useReassignStudentMutation } from "../../services/adminApi";
import { Card, CardContent } from "../ui/card";
import { Badge } from "../ui/badge";
import { Button } from "../ui/button";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from "../ui/dialog";
import { ClipboardDocumentListIcon, ArrowPathIcon } from "@heroicons/react/24/outline";

export default function AssignmentsTab() {
  const { data: assignments, isLoading, isError } = useGetAssignmentsQuery(undefined);

  if (isLoading) {
    return (
      <div className="py-8 text-center">
        <div className="size-8 mx-auto mb-3 rounded-full border-2 border-cerulean-500 border-t-transparent animate-spin" />
        <p className="text-parchment-500 dark:text-slate-400 text-sm">Loading assignments...</p>
      </div>
    );
  }

  if (isError) {
    return (
      <div className="py-8 text-center">
        <p className="text-red-600 text-sm">Failed to load assignments.</p>
      </div>
    );
  }

  const activeCount = assignments?.filter((a) => a.status === "Active").length ?? 0;

  return (
    <div className="p-5 pt-4">
      <h3 className="font-serif text-lg font-medium text-parchment-900 dark:text-white mb-4">
        Teacher-Student Assignments
        {assignments && assignments.length > 0 && (
          <Badge variant="secondary" className="ml-3 align-middle">{assignments.length}</Badge>
        )}
      </h3>
      {assignments && assignments.length > 0 && (
        <p className="text-xs text-parchment-400 dark:text-slate-500 mb-4">{activeCount} active</p>
      )}
      {!assignments || assignments.length === 0 ? (
        <div className="py-12 text-center">
          <div className="size-14 mx-auto mb-4 rounded-xl bg-parchment-100 dark:bg-slate-800 flex items-center justify-center text-parchment-300 dark:text-slate-600">
            <ClipboardDocumentListIcon className="size-8" />
          </div>
          <h3 className="font-serif text-lg font-medium text-parchment-700 dark:text-slate-300 mb-1">No assignments yet</h3>
          <p className="text-sm text-parchment-400 dark:text-slate-500">Assignments appear when teachers claim students.</p>
        </div>
      ) : (
        <div className="space-y-2">
          {assignments.map((a) => (
            <Card key={a.id} className="p-4 hover:shadow-sm transition-all">
              <CardContent>
                <div className="flex items-center justify-between">
                  <div className="min-w-0">
                    <div className="flex items-center gap-2 mb-1">
                      <span className="text-sm font-medium text-parchment-900 dark:text-white">{a.studentDisplayName || a.studentEmail}</span>
                      <Badge variant={a.status === "Active" ? "success" : "secondary"} className="text-[10px]">{a.status}</Badge>
                    </div>
                    <div className="text-xs text-parchment-500 dark:text-slate-400 space-x-2">
                      <span>Teacher: {a.primaryTeacherDisplayName || a.primaryTeacherEmail}</span>
                      <span>&middot;</span>
                      <span>Since {new Date(a.assignedAt).toLocaleDateString()}</span>
                      {a.endedAt && (<><span>&middot;</span><span>Ended {new Date(a.endedAt).toLocaleDateString()}</span></>)}
                    </div>
                  </div>
                  {a.status === "Active" && <ReassignDialog studentId={a.studentId} />}
                </div>
              </CardContent>
            </Card>
          ))}
        </div>
      )}
    </div>
  );
}

function ReassignDialog({ studentId }: { studentId: string }) {
  const [reassignStudent, { isLoading: reassigning }] = useReassignStudentMutation();
  const [open, setOpen] = useState(false);
  const [newTeacherId, setNewTeacherId] = useState("");
  const [error, setError] = useState("");

  const handleReassign = async () => {
    if (!newTeacherId.trim()) { setError("Teacher ID is required"); return; }
    setError("");
    try {
      await reassignStudent({ studentId, newTeacherId: newTeacherId.trim() }).unwrap();
      setOpen(false);
      setNewTeacherId("");
    } catch { setError("Failed to reassign student"); }
  };

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        <Button variant="ghost" size="sm"><ArrowPathIcon className="size-3.5 mr-1" />Reassign</Button>
      </DialogTrigger>
      <DialogContent>
        <DialogHeader>
          <DialogTitle className="font-serif">Reassign Student</DialogTitle>
        </DialogHeader>
        <div className="flex flex-col gap-3">
          <p className="text-sm text-parchment-600 dark:text-slate-400">Assign this student to a different teacher. The current assignment will be ended.</p>
          {error && <p className="text-sm text-red-600 bg-red-50 dark:bg-red-900/20 rounded-lg p-2">{error}</p>}
          <div className="flex flex-col gap-1.5">
            <label htmlFor="reassign-teacher-id" className="text-sm font-medium text-parchment-700 dark:text-slate-300">New Teacher ID</label>
            <input id="reassign-teacher-id" type="text" value={newTeacherId} onChange={(e) => setNewTeacherId(e.target.value)}
              placeholder="Enter teacher user ID"
              className="w-full rounded-lg border border-parchment-200 dark:border-slate-700 bg-white dark:bg-slate-900 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-cerulean-500 dark:text-white" />
          </div>
          <div className="flex gap-2 justify-end pt-2">
            <Button variant="ghost" onClick={() => setOpen(false)}>Cancel</Button>
            <Button onClick={handleReassign} disabled={reassigning || !newTeacherId.trim()} className="bg-cerulean-600 hover:bg-cerulean-700 text-white">
              {reassigning ? "Reassigning..." : "Reassign"}
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
}
