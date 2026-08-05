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

const adminApi = apiSlice.injectEndpoints({
  endpoints: (builder) => ({
    listRoleAssignments: builder.query<RoleAssignmentDto[], void>({
      query: () => "/admin/role-assignments",
    }),
    assignRole: builder.mutation<RoleAssignmentDto, { userId: string; role: string }>({
      query: (body) => ({ url: "/admin/role-assignments", method: "POST", body }),
    }),
  }),
});

export const { useListRoleAssignmentsQuery, useAssignRoleMutation } = adminApi;
