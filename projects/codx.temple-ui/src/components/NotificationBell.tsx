import { useState } from "react";
import { useListNotificationsQuery, useMarkNotificationReadMutation, useMarkAllNotificationsReadMutation } from "../services/notificationsApi";
import { BellIcon } from "@heroicons/react/24/outline";

export default function NotificationBell() {
  const { data: notifications } = useListNotificationsQuery(undefined, { pollingInterval: 15000 });
  const [markRead] = useMarkNotificationReadMutation();
  const [markAllRead] = useMarkAllNotificationsReadMutation();
  const [open, setOpen] = useState(false);

  const unreadCount = notifications?.filter((n) => !n.isRead).length ?? 0;

  const handleClick = (id: string) => {
    markRead(id);
  };

  return (
    <div className="relative">
      <button
        type="button"
        onClick={() => {
          if (open && unreadCount > 0) markAllRead();
          setOpen(!open);
        }}
        className="relative p-2 rounded-lg text-parchment-500 dark:text-slate-400 hover:text-parchment-700 dark:hover:text-slate-200 hover:bg-parchment-100 dark:hover:bg-slate-800 transition-colors"
        title="Notifications"
      >
        <BellIcon className="size-5" />
        {unreadCount > 0 && (
          <span className="absolute -top-0.5 -right-0.5 size-4 rounded-full bg-red-500 text-white text-[9px] font-medium flex items-center justify-center">
            {unreadCount > 9 ? "9+" : unreadCount}
          </span>
        )}
      </button>

      {open && (
        <div className="absolute left-0 bottom-full mb-2 w-80 rounded-xl border border-parchment-200 dark:border-slate-700 bg-white dark:bg-slate-900 shadow-lg z-50 overflow-hidden">
          <div className="p-3 border-b border-parchment-100 dark:border-slate-800 flex items-center justify-between">
            <span className="text-sm font-medium text-parchment-900 dark:text-white">Notifications</span>
            {unreadCount > 0 && (
              <button type="button" onClick={() => markAllRead()} className="text-xs text-cerulean-600 hover:underline">
                Mark all read
              </button>
            )}
          </div>
          <div className="max-h-80 overflow-y-auto">
            {!notifications || notifications.length === 0 ? (
              <p className="p-4 text-sm text-parchment-400 dark:text-slate-500">No notifications.</p>
            ) : (
              notifications.map((n) => (
                <button
                  key={n.id}
                  type="button"
                  onClick={() => handleClick(n.id)}
                  className={`w-full text-left px-4 py-3 text-sm border-b border-parchment-50 dark:border-slate-800 hover:bg-parchment-50 dark:hover:bg-slate-800 ${
                    n.isRead ? "text-parchment-500 dark:text-slate-400" : "text-parchment-900 dark:text-white font-medium"
                  }`}
                >
                  <span className="block">{n.type}</span>
                  <span className="text-xs text-parchment-400 dark:text-slate-500">
                    {new Date(n.createdAt).toLocaleString()}
                  </span>
                </button>
              ))
            )}
          </div>
        </div>
      )}
    </div>
  );
}