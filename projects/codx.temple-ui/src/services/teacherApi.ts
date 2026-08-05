import { apiSlice } from "../store/apiSlice";

export interface TeacherAssignmentDto {
  id: string;
  studentId: string;
  studentEmail: string;
  studentDisplayName: string;
  primaryTeacherId: string;
  status: string;
  assignedAt: string;
}

export interface StudySessionDto {
  id: string;
  lessonAttemptId: string;
  sequenceNumber: number;
  startQuestionId: string | null;
  endQuestionId: string | null;
  currentQuestionId: string | null;
  status: string;
  startedAt: string | null;
  endedAt: string | null;
}

export interface StartSessionRequest {
  lessonAttemptId: string;
}

export interface AdvanceSessionRequest {
  currentQuestionId: string;
}

export interface MarkReviewedRequest {
  lessonAttemptId: string;
  questionKey: string;
}

const teacherApi = apiSlice.injectEndpoints({
  endpoints: (builder) => ({
    getTeacherStudents: builder.query<TeacherAssignmentDto[], void>({
      query: () => "/api/teacher/students",
    }),
    startSession: builder.mutation<StudySessionDto, StartSessionRequest>({
      query: (body) => ({ url: "/api/study-sessions", method: "POST", body }),
    }),
    advanceSession: builder.mutation<StudySessionDto, { sessionId: string; currentQuestionId: string }>({
      query: ({ sessionId, currentQuestionId }) => ({
        url: `/api/study-sessions/${sessionId}/advance`,
        method: "PUT",
        body: { currentQuestionId },
      }),
    }),
    endSession: builder.mutation<StudySessionDto, string>({
      query: (sessionId) => ({ url: `/api/study-sessions/${sessionId}/end`, method: "PUT" }),
    }),
    markReviewed: builder.mutation<void, MarkReviewedRequest>({
      query: (body) => ({ url: "/api/study-sessions/mark-reviewed", method: "POST", body }),
    }),
  }),
});

export const {
  useGetTeacherStudentsQuery,
  useStartSessionMutation,
  useAdvanceSessionMutation,
  useEndSessionMutation,
  useMarkReviewedMutation,
} = teacherApi;
