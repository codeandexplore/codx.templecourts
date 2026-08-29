import { apiSlice } from "../store/apiSlice";

export interface StudentAnswerInfoDto {
  value: string;
  submittedAt: string;
}

export interface AnswerFlagInfoDto {
  type: string;
  createdAt: string;
}

export interface QuestionWithAnswerDto {
  key: string;
  order: number;
  depth: number;
  parentNodeTitle: string;
  questionType: string;
  promptText: string;
  answer: StudentAnswerInfoDto | null;
  isReviewed: boolean;
  flag: AnswerFlagInfoDto | null;
}

export interface SessionQuestionsDto {
  sessionId: string;
  lessonAttemptId: string;
  status: string;
  currentQuestionId: string | null;
  studentDisplayName: string;
  lessonNumber: number;
  lessonTitle: string;
  questions: QuestionWithAnswerDto[];
}

const sessionApi = apiSlice.injectEndpoints({
  endpoints: (builder) => ({
    getSessionQuestions: builder.query<SessionQuestionsDto, string>({
      query: (sessionId) => `/api/study-sessions/${sessionId}/questions`,
      providesTags: ["Session"],
    }),
    markReviewed: builder.mutation<void, { lessonAttemptId: string; questionKey: string }>({
      query: (body) => ({ url: "/api/study-sessions/mark-reviewed", method: "POST", body }),
      invalidatesTags: ["Session"],
    }),
    advanceSession: builder.mutation<void, { sessionId: string; currentQuestionId: string }>({
      query: ({ sessionId, currentQuestionId }) => ({
        url: `/api/study-sessions/${sessionId}/advance`,
        method: "PUT",
        body: { currentQuestionId },
      }),
      invalidatesTags: ["Session"],
    }),
    endSession: builder.mutation<void, string>({
      query: (sessionId) => ({ url: `/api/study-sessions/${sessionId}/end`, method: "PUT" }),
      invalidatesTags: ["Session"],
    }),
  }),
});

export const {
  useGetSessionQuestionsQuery,
  useMarkReviewedMutation,
  useAdvanceSessionMutation,
  useEndSessionMutation,
} = sessionApi;
