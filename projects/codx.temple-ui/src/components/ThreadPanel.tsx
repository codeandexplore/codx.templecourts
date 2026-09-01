import { useState, useCallback } from "react";
import { useGetThreadQuery, usePostThreadMessageMutation } from "../services/threadApi";
import { useThreadHub } from "../hooks/useThreadHub";
import { ChatBubbleLeftRightIcon, LockClosedIcon } from "@heroicons/react/24/outline";
import { Button } from "./ui/button";
import { Textarea } from "./ui/textarea";

interface ThreadPanelProps {
  threadId: string;
  disabled?: boolean;
  checkQuestions?: { id: string; noteText: string }[];
}

export default function ThreadPanel({ threadId, disabled, checkQuestions }: ThreadPanelProps) {
  const { data: thread, isLoading, refetch } = useGetThreadQuery(threadId);
  const [postMessage] = usePostThreadMessageMutation();
  const [text, setText] = useState("");
  const [selectedCheckQuestion, setSelectedCheckQuestion] = useState("");
  const [posting, setPosting] = useState(false);
  const [open, setOpen] = useState(false);

  const handleMessagePosted = useCallback(() => {
    refetch();
  }, [refetch]);

  useThreadHub(threadId, handleMessagePosted);

  const isLocked = thread?.status === "Locked";

  const handlePost = async () => {
    if (!text.trim()) return;
    setPosting(true);
    try {
      await postMessage({
        threadId,
        bodyText: text.trim(),
        sourceCheckQuestionId: selectedCheckQuestion || undefined,
      }).unwrap();
      setText("");
      setSelectedCheckQuestion("");
    } catch { /* handled by RTK */ }
    setPosting(false);
  };

  if (isLoading) {
    return <p className="text-xs text-parchment-400 dark:text-slate-500">Loading conversation...</p>;
  }

  if (!open && !disabled) {
    return (
      <button
        type="button"
        onClick={() => setOpen(true)}
        className="inline-flex items-center gap-1 text-xs text-cerulean-600 dark:text-cerulean-400 hover:underline"
      >
        <ChatBubbleLeftRightIcon className="size-3.5" />
        {thread && thread.messages.length > 0 ? `Thread (${thread.messages.length})` : "Start conversation"}
      </button>
    );
  }

  const readOnly = disabled || isLocked;

  return (
    <div className="border border-parchment-200 dark:border-slate-700 rounded-lg bg-parchment-50/50 dark:bg-slate-800/30 p-3 space-y-2">
      <div className="flex items-center justify-between">
        <span className="text-xs font-medium text-parchment-500 dark:text-slate-400 uppercase tracking-wide flex items-center gap-1">
          <ChatBubbleLeftRightIcon className="size-3.5" />
          Conversation
        </span>
        {isLocked && (
          <span className="text-[10px] text-parchment-400 dark:text-slate-500 flex items-center gap-1">
            <LockClosedIcon className="size-3" />
            Locked
          </span>
        )}
      </div>

      <div className="space-y-1.5">
        {thread && thread.messages.length === 0 ? (
          <p className="text-xs text-parchment-400 dark:text-slate-500">No messages yet.</p>
        ) : (
          thread?.messages.map((m) => (
            <div key={m.id} className="text-sm">
              <span className="font-medium text-parchment-800 dark:text-slate-200">{m.authorDisplayName}</span>
              <span className="ml-2 text-[10px] text-parchment-400 dark:text-slate-500">
                {new Date(m.createdAt).toLocaleString()}
              </span>
              <p className="text-sm text-parchment-700 dark:text-slate-300 leading-relaxed">{m.bodyText}</p>
            </div>
          ))
        )}
      </div>

      {readOnly ? (
        <Button variant="ghost" size="sm" onClick={() => setOpen(false)} className="text-xs">
          Close
        </Button>
      ) : (
        <>
          {checkQuestions && checkQuestions.length > 0 && (
            <select
              value={selectedCheckQuestion}
              onChange={(e) => setSelectedCheckQuestion(e.target.value)}
              className="w-full rounded-lg border border-parchment-200 dark:border-slate-700 bg-white dark:bg-slate-900 px-3 py-1.5 text-sm focus:outline-none focus:ring-2 focus:ring-cerulean-500 dark:text-white"
            >
              <option value="">Post a message...</option>
              {checkQuestions.map((cq) => (
                <option key={cq.id} value={cq.id}>{cq.noteText}</option>
              ))}
            </select>
          )}
          <Textarea
            value={text}
            onChange={(e) => setText(e.target.value)}
            placeholder="Write a message..."
            className="min-h-[60px] text-sm"
          />
          <div className="flex gap-2">
            <Button size="sm" onClick={handlePost} disabled={posting || !text.trim()} className="bg-cerulean-600 hover:bg-cerulean-700 text-white">
              {posting ? "Posting..." : "Post"}
            </Button>
            <Button size="sm" variant="ghost" onClick={() => setOpen(false)}>
              Close
            </Button>
          </div>
        </>
      )}
    </div>
  );
}