import { apiSlice } from "../store/apiSlice";

export interface AppointmentDto {
  id: string;
  studentId: string;
  teacherId: string;
  scheduledAt: string;
  durationMinutes: number;
  meetingLink: string | null;
  status: string;
  createdAt: string;
}

const appointmentsApi = apiSlice.injectEndpoints({
  endpoints: (builder) => ({
    listAppointments: builder.query<AppointmentDto[], void>({
      query: () => "/api/appointments",
      providesTags: ["Appointments"],
    }),
    getAppointment: builder.query<AppointmentDto, string>({
      query: (id) => `/api/appointments/${id}`,
      providesTags: ["Appointments"],
    }),
    createAppointment: builder.mutation<AppointmentDto, { studentId: string; scheduledAt: string; durationMinutes: number; meetingLink?: string }>({
      query: (body) => ({ url: "/api/appointments", method: "POST", body }),
      invalidatesTags: ["Appointments"],
    }),
    confirmAppointment: builder.mutation<AppointmentDto, string>({
      query: (id) => ({ url: `/api/appointments/${id}/confirm`, method: "POST" }),
      invalidatesTags: ["Appointments"],
    }),
    cancelAppointment: builder.mutation<AppointmentDto, string>({
      query: (id) => ({ url: `/api/appointments/${id}/cancel`, method: "POST" }),
      invalidatesTags: ["Appointments"],
    }),
  }),
});

export const {
  useListAppointmentsQuery,
  useGetAppointmentQuery,
  useCreateAppointmentMutation,
  useConfirmAppointmentMutation,
  useCancelAppointmentMutation,
} = appointmentsApi;