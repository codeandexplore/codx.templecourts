import { apiSlice } from "../store/apiSlice";
import type { LessonVersionDto } from "./lessonsApi";

const versionsApi = apiSlice.injectEndpoints({
  endpoints: (builder) => ({
    getVersions: builder.query<LessonVersionDto[], string>({
      query: (lessonKey) => `/api/lessons/${lessonKey}/versions`,
      providesTags: ["Versions"],
    }),
    createDraft: builder.mutation<LessonVersionDto, { lessonKey: string; changeNotes?: string }>({
      query: ({ lessonKey, changeNotes }) => ({
        url: `/api/lessons/${lessonKey}/versions`,
        method: "POST",
        body: { changeNotes },
      }),
      invalidatesTags: ["Versions"],
    }),
    publishVersion: builder.mutation<void, { lessonKey: string; versionId: string }>({
      query: ({ lessonKey, versionId }) => ({
        url: `/api/lessons/${lessonKey}/versions/${versionId}/publish`,
        method: "POST",
      }),
      invalidatesTags: ["Versions", "Lessons"],
    }),
  }),
});

export const { useGetVersionsQuery, useCreateDraftMutation, usePublishVersionMutation } = versionsApi;
