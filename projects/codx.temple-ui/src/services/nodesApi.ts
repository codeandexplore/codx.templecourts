import { apiSlice } from "../store/apiSlice";
import type { LessonNodeDto } from "./lessonsApi";

const nodesApi = apiSlice.injectEndpoints({
  endpoints: (builder) => ({
    createNode: builder.mutation<
      LessonNodeDto,
      {
        versionId: string;
        parentNodeKey?: string;
        title: string;
        description: string;
        requiresPriorSiblingAnswered?: boolean;
        order?: number;
      }
    >({
      query: ({ versionId, ...body }) => ({
        url: `/api/lesson-versions/${versionId}/nodes`,
        method: "POST",
        body,
      }),
      invalidatesTags: ["Nodes", "Versions"],
    }),
    updateNode: builder.mutation<
      LessonNodeDto,
      {
        versionId: string;
        nodeKey: string;
        title: string;
        description: string;
        requiresPriorSiblingAnswered: boolean;
      }
    >({
      query: ({ versionId, nodeKey, ...body }) => ({
        url: `/api/lesson-versions/${versionId}/nodes/${nodeKey}`,
        method: "PUT",
        body,
      }),
      invalidatesTags: ["Nodes", "Versions"],
    }),
    deleteNode: builder.mutation<void, { versionId: string; nodeKey: string }>({
      query: ({ versionId, nodeKey }) => ({
        url: `/api/lesson-versions/${versionId}/nodes/${nodeKey}`,
        method: "DELETE",
      }),
      invalidatesTags: ["Nodes", "Versions"],
    }),
    reorderNodes: builder.mutation<
      void,
      { versionId: string; parentNodeKey?: string; orderedKeys: string[] }
    >({
      query: ({ versionId, ...body }) => ({
        url: `/api/lesson-versions/${versionId}/nodes/reorder`,
        method: "PUT",
        body,
      }),
      invalidatesTags: ["Nodes", "Versions"],
    }),
  }),
});

export const {
  useCreateNodeMutation,
  useUpdateNodeMutation,
  useDeleteNodeMutation,
  useReorderNodesMutation,
} = nodesApi;
