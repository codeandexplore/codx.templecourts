import { apiSlice } from "../store/apiSlice";

export interface NotificationDto {
  id: string;
  type: string;
  referenceType: string;
  referenceId: string;
  deliveryChannel: string;
  isRead: boolean;
  createdAt: string;
}

const notificationsApi = apiSlice.injectEndpoints({
  endpoints: (builder) => ({
    listNotifications: builder.query<NotificationDto[], void>({
      query: () => "/api/notifications",
      providesTags: ["Notifications"],
    }),
    markNotificationRead: builder.mutation<void, string>({
      query: (id) => ({ url: `/api/notifications/${id}/read`, method: "POST" }),
      invalidatesTags: ["Notifications"],
    }),
    markAllNotificationsRead: builder.mutation<void, void>({
      query: () => ({ url: "/api/notifications/read-all", method: "POST" }),
      invalidatesTags: ["Notifications"],
    }),
  }),
});

export const {
  useListNotificationsQuery,
  useMarkNotificationReadMutation,
  useMarkAllNotificationsReadMutation,
} = notificationsApi;