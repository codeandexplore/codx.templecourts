import { apiSlice } from "../store/apiSlice";

export interface RoleAssignmentDto {
  id: string;
  userId: string;
  userEmail: string;
  userDisplayName: string;
  role: string;
  assignedBy: string;
  assignedAt: string;
}

export interface TeacherAssignmentDto {
  id: string;
  studentId: string;
  studentEmail: string;
  studentDisplayName: string;
  primaryTeacherId: string;
  primaryTeacherEmail: string;
  primaryTeacherDisplayName: string;
  status: string;
  assignedAt: string;
  endedAt: string | null;
}

const adminApi = apiSlice.injectEndpoints({
  endpoints: (builder) => ({
    listRoleAssignments: builder.query<RoleAssignmentDto[], void>({
      query: () => "/admin/role-assignments",
    }),
    assignRole: builder.mutation<RoleAssignmentDto, { userId: string; role: string }>({
      query: (body) => ({ url: "/admin/role-assignments", method: "POST", body }),
    }),
    getAssignments: builder.query<TeacherAssignmentDto[], string | undefined>({
      query: (status) => ({
        url: "/admin/assignments",
        params: status ? { status } : undefined,
      }),
      providesTags: ["Assignments"],
    }),
    reassignStudent: builder.mutation<TeacherAssignmentDto, { studentId: string; newTeacherId: string }>({
      query: (body) => ({ url: "/admin/assignments/reassign", method: "POST", body }),
      invalidatesTags: ["Assignments"],
    }),
  }),
});

export const {
  useListRoleAssignmentsQuery,
  useAssignRoleMutation,
  useGetAssignmentsQuery,
  useReassignStudentMutation,
} = adminApi;
