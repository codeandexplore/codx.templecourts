import { useListAuditLogsQuery } from "../../services/adminApi";
import { Card, CardContent } from "../ui/card";
import { Badge } from "../ui/badge";
import { ClockIcon } from "@heroicons/react/24/outline";

export default function AuditLogTab() {
  const { data: logs, isLoading, isError } = useListAuditLogsQuery();

  if (isLoading) {
    return (
      <div className="py-8 text-center">
        <div className="size-8 mx-auto mb-3 rounded-full border-2 border-cerulean-500 border-t-transparent animate-spin" />
        <p className="text-parchment-500 dark:text-slate-400 text-sm">Loading audit log...</p>
      </div>
    );
  }

  if (isError) {
    return <div className="py-8 text-center"><p className="text-red-600 text-sm">Failed to load audit log.</p></div>;
  }

  return (
    <div className="p-5 pt-4">
      <h3 className="font-serif text-lg font-medium text-parchment-900 dark:text-white mb-4">
        Audit Log
        {logs && logs.length > 0 && <Badge variant="secondary" className="ml-3 align-middle">{logs.length}</Badge>}
      </h3>

      <div className="space-y-2">
        {!logs || logs.length === 0 ? (
          <p className="text-parchment-400 dark:text-slate-500 text-sm">No audit events recorded.</p>
        ) : (
          logs.map((log) => (
            <Card key={log.id} className="p-4">
              <CardContent>
                <div className="flex items-center justify-between gap-3">
                  <div className="min-w-0">
                    <div className="flex items-center gap-2">
                      <Badge variant="secondary" className="text-[10px]">{log.action}</Badge>
                      <span className="text-sm text-parchment-800 dark:text-slate-200">
                        by {log.performedByDisplayName || log.performedById.slice(0, 8)}
                      </span>
                    </div>
                    {log.targetUserDisplayName && (
                      <p className="text-xs text-parchment-500 dark:text-slate-400 mt-1">Target: {log.targetUserDisplayName}</p>
                    )}
                    {log.metadata && (
                      <p className="text-xs text-parchment-500 dark:text-slate-400 mt-1 italic">{log.metadata}</p>
                    )}
                  </div>
                  <div className="flex items-center gap-1 text-xs text-parchment-400 dark:text-slate-500 shrink-0">
                    <ClockIcon className="size-3.5" />
                    {new Date(log.createdAt).toLocaleString()}
                  </div>
                </div>
              </CardContent>
            </Card>
          ))
        )}
      </div>
    </div>
  );
}