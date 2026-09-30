import { Link } from "react-router-dom";
import { useListAppointmentsQuery } from "../services/appointmentsApi";
import { ArrowLeftIcon } from "@heroicons/react/24/outline";
import { Card, CardContent } from "../components/ui/card";
import { Badge } from "../components/ui/badge";

export default function StudentAppointmentsPage() {
  const { data: appointments, isLoading } = useListAppointmentsQuery();

  if (isLoading) return <div className="text-parchment-500 dark:text-slate-400">Loading appointments...</div>;

  return (
    <div>
      <Link to="/" className="inline-flex items-center gap-1 text-sm text-cerulean-600 hover:underline mb-6">
        <ArrowLeftIcon className="size-4" />
        Back to Home
      </Link>
      <h2 className="font-serif text-2xl font-semibold text-parchment-900 dark:text-white mb-6">My Appointments</h2>

      <div className="space-y-2">
        {!appointments || appointments.length === 0 ? (
          <p className="text-parchment-400 dark:text-slate-500 text-sm">No appointments scheduled.</p>
        ) : (
          appointments.map((a) => (
            <Card key={a.id} className="p-4">
              <CardContent className="flex items-center justify-between gap-3">
                <div className="min-w-0 flex-1">
                  <div className="flex items-center gap-2">
                    <span className="text-sm font-medium text-parchment-900 dark:text-white">
                      {new Date(a.scheduledAt).toLocaleString()}
                    </span>
                    <Badge variant={a.status === "Confirmed" ? "success" : a.status === "Cancelled" ? "destructive" : "warning"} className="text-[10px]">
                      {a.status}
                    </Badge>
                  </div>
                  <div className="text-xs text-parchment-500 dark:text-slate-400 mt-1">{a.durationMinutes} min</div>
                  {a.meetingLink && a.status === "Confirmed" && (
                    <a href={a.meetingLink} target="_blank" rel="noreferrer" className="text-xs text-cerulean-600 hover:underline">
                      {a.meetingLink}
                    </a>
                  )}
                </div>
              </CardContent>
            </Card>
          ))
        )}
      </div>
    </div>
  );
}