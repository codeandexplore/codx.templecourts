import { useState } from "react";
import { Link } from "react-router-dom";
import { useListAppointmentsQuery, useCreateAppointmentMutation, useConfirmAppointmentMutation, useCancelAppointmentMutation } from "../services/appointmentsApi";
import { useGetTeacherStudentsQuery } from "../services/teacherApi";
import { ArrowLeftIcon, PlusIcon, CheckCircleIcon, XCircleIcon } from "@heroicons/react/24/outline";
import { Button } from "../components/ui/button";
import { Card, CardContent } from "../components/ui/card";
import { Badge } from "../components/ui/badge";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "../components/ui/dialog";

export default function AppointmentsPage() {
  const { data: appointments, isLoading } = useListAppointmentsQuery();
  const { data: students } = useGetTeacherStudentsQuery();
  const [createAppointment] = useCreateAppointmentMutation();
  const [confirmAppointment] = useConfirmAppointmentMutation();
  const [cancelAppointment] = useCancelAppointmentMutation();
  const [showCreate, setShowCreate] = useState(false);

  if (isLoading) return <div className="text-parchment-500 dark:text-slate-400">Loading appointments...</div>;

  return (
    <div>
      <Link to="/teacher" className="inline-flex items-center gap-1 text-sm text-cerulean-600 hover:underline mb-6">
        <ArrowLeftIcon className="size-4" />
        Back to dashboard
      </Link>
      <div className="flex items-center justify-between mb-6">
        <h2 className="font-serif text-2xl font-semibold text-parchment-900 dark:text-white">Appointments</h2>
        <Button className="bg-cerulean-600 hover:bg-cerulean-700 text-white" onClick={() => setShowCreate(true)}>
          <PlusIcon className="size-4 mr-1.5" />
          New Appointment
        </Button>
      </div>

      <div className="space-y-2">
        {!appointments || appointments.length === 0 ? (
          <p className="text-parchment-400 dark:text-slate-500 text-sm">No appointments yet.</p>
        ) : (
          appointments.map((a) => {
            const student = students?.find((s) => s.studentId === a.studentId);
            return (
              <Card key={a.id} className="p-4">
                <CardContent>
                  <div className="flex items-center justify-between gap-3">
                    <div className="min-w-0 flex-1">
                      <div className="flex items-center gap-2">
                        <span className="text-sm font-medium text-parchment-900 dark:text-white">
                          {student?.studentDisplayName ?? a.studentId.slice(0, 8)}
                        </span>
                        <Badge variant={a.status === "Confirmed" ? "success" : a.status === "Cancelled" ? "destructive" : "warning"} className="text-[10px]">
                          {a.status}
                        </Badge>
                      </div>
                      <div className="text-xs text-parchment-500 dark:text-slate-400 mt-1">
                        {new Date(a.scheduledAt).toLocaleString()} · {a.durationMinutes} min
                      </div>
                      {a.meetingLink && (
                        <a href={a.meetingLink} target="_blank" rel="noreferrer" className="text-xs text-cerulean-600 hover:underline">
                          {a.meetingLink}
                        </a>
                      )}
                    </div>
                    {a.status === "Proposed" && (
                      <div className="flex items-center gap-1 shrink-0">
                        <Button size="sm" variant="ghost" className="text-emerald-600 dark:text-emerald-400" onClick={() => confirmAppointment(a.id)}>
                          <CheckCircleIcon className="size-4" />
                          Confirm
                        </Button>
                        <Button size="sm" variant="ghost" className="text-red-600 dark:text-red-400" onClick={() => cancelAppointment(a.id)}>
                          <XCircleIcon className="size-4" />
                          Cancel
                        </Button>
                      </div>
                    )}
                  </div>
                </CardContent>
              </Card>
            );
          })
        )}
      </div>

      {showCreate && (
        <CreateAppointmentDialog
          students={students ?? []}
          onClose={() => setShowCreate(false)}
          onCreate={async (body) => {
            await createAppointment(body).unwrap();
            setShowCreate(false);
          }}
        />
      )}
    </div>
  );
}

function CreateAppointmentDialog({
  students,
  onClose,
  onCreate,
}: {
  students: { studentId: string; studentDisplayName: string; studentEmail: string }[];
  onClose: () => void;
  onCreate: (body: { studentId: string; scheduledAt: string; durationMinutes: number; meetingLink?: string }) => Promise<void>;
}) {
  const [studentId, setStudentId] = useState("");
  const [scheduledAt, setScheduledAt] = useState("");
  const [duration, setDuration] = useState("30");
  const [meetingLink, setMeetingLink] = useState("");
  const [saving, setSaving] = useState(false);

  const handleCreate = async () => {
    if (!studentId || !scheduledAt) return;
    setSaving(true);
    try {
      await onCreate({
        studentId,
        scheduledAt: new Date(scheduledAt).toISOString(),
        durationMinutes: parseInt(duration) || 30,
        meetingLink: meetingLink.trim() || undefined,
      });
    } catch { /* handled by RTK */ }
    setSaving(false);
  };

  return (
    <Dialog open onOpenChange={(o) => !o && onClose()}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle className="font-serif">New Appointment</DialogTitle>
        </DialogHeader>
        <div className="flex flex-col gap-3">
          <div className="flex flex-col gap-1.5">
            <label className="text-sm font-medium text-parchment-700 dark:text-slate-300">Student</label>
            <select
              value={studentId}
              onChange={(e) => setStudentId(e.target.value)}
              className="w-full rounded-lg border border-parchment-200 dark:border-slate-700 bg-white dark:bg-slate-900 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-cerulean-500 dark:text-white"
            >
              <option value="">Select a student...</option>
              {students.map((s) => (
                <option key={s.studentId} value={s.studentId}>{s.studentDisplayName || s.studentEmail}</option>
              ))}
            </select>
          </div>
          <div className="flex flex-col gap-1.5">
            <label className="text-sm font-medium text-parchment-700 dark:text-slate-300">Scheduled Time</label>
            <input
              type="datetime-local"
              value={scheduledAt}
              onChange={(e) => setScheduledAt(e.target.value)}
              className="w-full rounded-lg border border-parchment-200 dark:border-slate-700 bg-white dark:bg-slate-900 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-cerulean-500 dark:text-white"
            />
          </div>
          <div className="flex flex-col gap-1.5">
            <label className="text-sm font-medium text-parchment-700 dark:text-slate-300">Duration (minutes)</label>
            <input
              type="number"
              value={duration}
              onChange={(e) => setDuration(e.target.value)}
              className="w-full rounded-lg border border-parchment-200 dark:border-slate-700 bg-white dark:bg-slate-900 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-cerulean-500 dark:text-white"
            />
          </div>
          <div className="flex flex-col gap-1.5">
            <label className="text-sm font-medium text-parchment-700 dark:text-slate-300">Meeting Link (external)</label>
            <input
              type="text"
              value={meetingLink}
              onChange={(e) => setMeetingLink(e.target.value)}
              placeholder="https://..."
              className="w-full rounded-lg border border-parchment-200 dark:border-slate-700 bg-white dark:bg-slate-900 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-cerulean-500 dark:text-white"
            />
          </div>
          <div className="flex gap-2 justify-end pt-2">
            <Button variant="ghost" onClick={onClose}>Cancel</Button>
            <Button onClick={handleCreate} disabled={saving || !studentId || !scheduledAt} className="bg-cerulean-600 hover:bg-cerulean-700 text-white">
              {saving ? "Saving..." : "Create"}
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
}