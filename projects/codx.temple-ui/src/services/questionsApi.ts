import { apiSlice } from "../store/apiSlice";
import type { QuestionDto } from "./lessonsApi";

const questionsApi = apiSlice.injectEndpoints({
  endpoints: (builder) => ({
    createQuestion: builder.mutation<
      QuestionDto,
      {
        nodeKey: string;
        questionType: string;
        promptText: string;
        metadata?: Record<string, unknown>;
        referenceContext?: Record<string, unknown>;
        order?: number;
      }
    >({
      query: ({ nodeKey, ...body }) => ({
        url: `/api/lesson-nodes/${nodeKey}/questions`,
        method: "POST",
        body,
      }),
      invalidatesTags: ["Questions", "Versions"],
    }),
    updateQuestion: builder.mutation<
      QuestionDto,
      {
        nodeKey: string;
        questionKey: string;
        promptText: string;
        metadata?: Record<string, unknown>;
        referenceContext?: Record<string, unknown>;
      }
    >({
      query: ({ nodeKey, questionKey, ...body }) => ({
        url: `/api/lesson-nodes/${nodeKey}/questions/${questionKey}`,
        method: "PUT",
        body,
      }),
      invalidatesTags: ["Questions", "Versions"],
    }),
    deleteQuestion: builder.mutation<void, { nodeKey: string; questionKey: string }>({
      query: ({ nodeKey, questionKey }) => ({
        url: `/api/lesson-nodes/${nodeKey}/questions/${questionKey}`,
        method: "DELETE",
      }),
      invalidatesTags: ["Questions", "Versions"],
    }),
    reorderQuestions: builder.mutation<
      void,
      { nodeKey: string; orderedKeys: string[] }
    >({
      query: ({ nodeKey, ...body }) => ({
        url: `/api/lesson-nodes/${nodeKey}/questions/reorder`,
        method: "PUT",
        body,
      }),
      invalidatesTags: ["Questions", "Versions"],
    }),
  }),
});

export const {
  useCreateQuestionMutation,
  useUpdateQuestionMutation,
  useDeleteQuestionMutation,
  useReorderQuestionsMutation,
} = questionsApi;
