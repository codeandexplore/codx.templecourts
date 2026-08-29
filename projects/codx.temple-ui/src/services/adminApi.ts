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

export interface UserDto {
  id: string;
  email: string;
  displayName: string;
  status: string;
  roles: string[];
}

const adminApi = apiSlice.injectEndpoints({
  endpoints: (builder) => ({
    listRoleAssignments: builder.query<RoleAssignmentDto[], void>({
      query: () => "/admin/role-assignments",
      providesTags: ["Roles"],
    }),
    assignRole: builder.mutation<RoleAssignmentDto, { userId: string; role: string }>({
      query: (body) => ({ url: "/admin/role-assignments", method: "POST", body }),
      invalidatesTags: ["Roles", "Users"],
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
    listUsers: builder.query<UserDto[], void>({
      query: () => "/admin/users",
      providesTags: ["Users"],
    }),
    revokeRole: builder.mutation<void, string>({
      query: (assignmentId) => ({ url: `/admin/role-assignments/${assignmentId}`, method: "DELETE" }),
      invalidatesTags: ["Roles", "Users"],
    }),
    resetUserPassword: builder.mutation<void, { userId: string; newPassword: string }>({
      query: ({ userId, newPassword }) => ({
        url: `/admin/users/${userId}/reset-password`,
        method: "POST",
        body: { newPassword },
      }),
    }),
    updateUserStatus: builder.mutation<void, { userId: string; status: string }>({
      query: ({ userId, status }) => ({
        url: `/admin/users/${userId}/status`,
        method: "PUT",
        body: { status },
      }),
      invalidatesTags: ["Users"],
    }),
  }),
});

export const {
  useListRoleAssignmentsQuery,
  useAssignRoleMutation,
  useGetAssignmentsQuery,
  useReassignStudentMutation,
  useListUsersQuery,
  useRevokeRoleMutation,
  useResetUserPasswordMutation,
  useUpdateUserStatusMutation,
} = adminApi;
