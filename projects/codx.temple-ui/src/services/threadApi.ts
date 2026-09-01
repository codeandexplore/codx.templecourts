import { apiSlice } from "../store/apiSlice";

export interface ThreadMessageDto {
  id: string;
  authorId: string;
  authorDisplayName: string;
  bodyText: string;
  sourceCheckQuestionId: string | null;
  createdAt: string;
}

export interface AnswerThreadDto {
  id: string;
  studentAnswerId: string;
  status: string;
  lockedAt: string | null;
  messages: ThreadMessageDto[];
}

const threadApi = apiSlice.injectEndpoints({
  endpoints: (builder) => ({
    getThread: builder.query<AnswerThreadDto, string>({
      query: (threadId) => `/api/threads/${threadId}`,
      providesTags: ["Threads"],
    }),
    postThreadMessage: builder.mutation<ThreadMessageDto, { threadId: string; bodyText: string; sourceCheckQuestionId?: string }>({
      query: ({ threadId, ...body }) => ({
        url: `/api/threads/${threadId}/messages`,
        method: "POST",
        body,
      }),
      invalidatesTags: ["Threads"],
    }),
  }),
});

export const { useGetThreadQuery, usePostThreadMessageMutation } = threadApi;