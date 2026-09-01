import { apiSlice } from "../store/apiSlice";

export interface LessonAttemptDto {
  id: string;
  lessonKey: string;
  lessonVersionId: string;
  status: string;
  startedAt: string;
  completedAt: string | null;
  answeredQuestionKeys: string[];
  activeSessionId?: string;
}

export interface StudentAnswerDto {
  id: string;
  lessonAttemptId: string;
  questionKey: string;
  answerValue: unknown;
  promptSnapshot: string;
  questionTypeSnapshot: string;
  submittedAt: string;
  threadId?: string;
}

export interface StudentQuestionNoteDto {
  id: string;
  questionKey: string;
  noteText: string;
  createdAt: string;
}

export interface StudentAttemptSummaryDto {
  id: string;
  lessonKey: string;
  lessonNumber: number;
  lessonTitle: string;
  status: string;
  startedAt: string;
  completedAt: string | null;
  answeredCount: number;
}

const studentApi = apiSlice.injectEndpoints({
  endpoints: (builder) => ({
    startAttempt: builder.mutation<LessonAttemptDto, string>({
      query: (lessonKey) => ({ url: `/api/lessons/${lessonKey}/attempts`, method: "POST" }),
      invalidatesTags: ["Attempts"],
    }),
    getAttempt: builder.query<LessonAttemptDto, string>({
      query: (id) => `/api/attempts/${id}`,
      providesTags: ["Attempts"],
    }),
    getAttemptByLesson: builder.query<LessonAttemptDto | null, string>({
      query: (lessonKey) => `/api/lessons/${lessonKey}/attempt`,
      providesTags: ["Attempts"],
    }),
    submitAnswer: builder.mutation<StudentAnswerDto, { attemptId: string; questionKey: string; answerValue: unknown }>({
      query: ({ attemptId, questionKey, answerValue }) => ({
        url: `/api/attempts/${attemptId}/answers`,
        method: "POST",
        body: { questionKey, answerValue },
      }),
      invalidatesTags: ["Attempts", "Answers"],
    }),
    completeAttempt: builder.mutation<LessonAttemptDto, string>({
      query: (attemptId) => ({ url: `/api/attempts/${attemptId}/complete`, method: "POST" }),
      invalidatesTags: ["Attempts"],
    }),
    getAttemptAnswers: builder.query<StudentAnswerDto[], string>({
      query: (attemptId) => `/api/attempts/${attemptId}/answers`,
      providesTags: ["Answers"],
    }),
    listMyAttempts: builder.query<StudentAttemptSummaryDto[], void>({
      query: () => "/api/attempts",
      providesTags: ["Attempts"],
    }),
    getNotes: builder.query<StudentQuestionNoteDto[], void>({
      query: () => "/api/notes",
      providesTags: ["Notes"],
    }),
    createNote: builder.mutation<StudentQuestionNoteDto, { questionKey: string; noteText: string }>({
      query: ({ questionKey, noteText }) => ({
        url: `/api/notes/${questionKey}`,
        method: "POST",
        body: { noteText },
      }),
      invalidatesTags: ["Notes"],
    }),
    updateNote: builder.mutation<StudentQuestionNoteDto, { questionKey: string; noteText: string }>({
      query: ({ questionKey, noteText }) => ({
        url: `/api/notes/${questionKey}`,
        method: "PUT",
        body: { noteText },
      }),
      invalidatesTags: ["Notes"],
    }),
    deleteNote: builder.mutation<void, string>({
      query: (questionKey) => ({ url: `/api/notes/${questionKey}`, method: "DELETE" }),
      invalidatesTags: ["Notes"],
    }),
  }),
});

export const {
  useStartAttemptMutation,
  useGetAttemptQuery,
  useGetAttemptByLessonQuery,
  useSubmitAnswerMutation,
  useCompleteAttemptMutation,
  useGetAttemptAnswersQuery,
  useListMyAttemptsQuery,
  useGetNotesQuery,
  useCreateNoteMutation,
  useUpdateNoteMutation,
  useDeleteNoteMutation,
} = studentApi;
