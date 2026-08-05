import { apiSlice } from "../store/apiSlice";

export interface LessonDto {
  id: string;
  key: string;
  number: number;
  title: string;
  status: string;
  currentPublishedVersionId: string | null;
}

export interface QuestionDto {
  id: string;
  key: string;
  lessonNodeId: string;
  order: number;
  questionType: string;
  promptText: string;
  metadata: unknown;
  referenceContext: unknown;
}

export interface LessonNodeDto {
  id: string;
  key: string;
  lessonVersionId: string;
  parentNodeId: string | null;
  depth: number;
  order: number;
  title: string;
  description: string;
  requiresPriorSiblingAnswered: boolean;
  children: LessonNodeDto[];
  questions: QuestionDto[];
}

export interface LessonVersionDto {
  id: string;
  lessonId: string;
  versionNumber: number;
  status: string;
  changeNotes: string | null;
  publishedAt: string | null;
  createdAt: string;
  nodes: LessonNodeDto[];
}

const lessonsApi = apiSlice.injectEndpoints({
  endpoints: (builder) => ({
    listLessons: builder.query<LessonDto[], void>({
      query: () => "/api/lessons",
      providesTags: ["Lessons"],
    }),
    getLessonTree: builder.query<LessonVersionDto, string>({
      query: (key) => `/api/lessons/${key}`,
    }),
    createLesson: builder.mutation<LessonDto, { number: number; title: string }>({
      query: (body) => ({ url: "/api/lessons", method: "POST", body }),
      invalidatesTags: ["Lessons"],
    }),
  }),
});

export const { useListLessonsQuery, useGetLessonTreeQuery, useCreateLessonMutation } = lessonsApi;
