import { apiSlice } from "../store/apiSlice";

export interface TeacherCheckQuestionDto {
  id: string;
  questionKey: string;
  noteText: string;
  isOrphaned: boolean;
  createdAt: string;
}

export interface PublishedQuestionDto {
  questionKey: string;
  promptText: string;
  lessonNumber: number;
  lessonTitle: string;
}

const checkQuestionApi = apiSlice.injectEndpoints({
  endpoints: (builder) => ({
    listCheckQuestions: builder.query<TeacherCheckQuestionDto[], void>({
      query: () => "/api/check-questions",
      providesTags: ["CheckQuestions"],
    }),
    getCheckQuestion: builder.query<TeacherCheckQuestionDto, string>({
      query: (id) => `/api/check-questions/${id}`,
      providesTags: ["CheckQuestions"],
    }),
    createCheckQuestion: builder.mutation<TeacherCheckQuestionDto, { questionKey: string; noteText: string }>({
      query: (body) => ({ url: "/api/check-questions", method: "POST", body }),
      invalidatesTags: ["CheckQuestions"],
    }),
    updateCheckQuestion: builder.mutation<TeacherCheckQuestionDto, { id: string; noteText: string }>({
      query: ({ id, noteText }) => ({
        url: `/api/check-questions/${id}`,
        method: "PUT",
        body: { noteText },
      }),
      invalidatesTags: ["CheckQuestions"],
    }),
    deleteCheckQuestion: builder.mutation<void, string>({
      query: (id) => ({ url: `/api/check-questions/${id}`, method: "DELETE" }),
      invalidatesTags: ["CheckQuestions"],
    }),
    listPublishedQuestions: builder.query<PublishedQuestionDto[], void>({
      query: () => "/api/check-questions/questions",
    }),
  }),
});

export const {
  useListCheckQuestionsQuery,
  useGetCheckQuestionQuery,
  useCreateCheckQuestionMutation,
  useUpdateCheckQuestionMutation,
  useDeleteCheckQuestionMutation,
  useListPublishedQuestionsQuery,
} = checkQuestionApi;