import { useParams, Link, useNavigate } from "react-router-dom";
import { useState } from "react";
import { useGetLessonTreeQuery, type QuestionDto } from "../services/lessonsApi";
import { useStartAttemptMutation, useGetAttemptByLessonQuery, useListMyAttemptsQuery } from "../services/studentApi";
import { LockClosedIcon, CheckBadgeIcon, QuestionMarkCircleIcon, ArrowLeftIcon, CheckCircleIcon } from "@heroicons/react/24/outline";
import { Button } from "../components/ui/button";
import { Card, CardContent } from "../components/ui/card";
import { Badge } from "../components/ui/badge";

export default function LessonDetailPage() {
  const { key } = useParams<{ key: string }>();
  const navigate = useNavigate();
  const { data: tree, isLoading } = useGetLessonTreeQuery(key!);
  const { data: attempt, refetch: refetchAttempt } = useGetAttemptByLessonQuery(key!);
  const { data: myAttempts } = useListMyAttemptsQuery();
  const [startAttempt, { isLoading: starting }] = useStartAttemptMutation();
  const [error, setError] = useState("");

  const lessonAttempts = (myAttempts ?? []).filter((a) => a.lessonKey === key);
  const hasCompleted = lessonAttempts.some((a) => a.status === "Completed");

  const handleStart = async () => {
    setError("");
    try {
      await startAttempt(key!).unwrap();
      refetchAttempt();
    } catch (e: unknown) {
      const err = e as { data?: { message?: string; error?: string } };
      setError(err?.data?.message || err?.data?.error || "Failed to start lesson.");
    }
  };

  if (isLoading) return <div className="text-parchment-500 dark:text-slate-400">Loading lesson...</div>;
  if (!tree) return null;

  return (
    <div>
      <Link to="/lessons" className="inline-flex items-center gap-1 text-sm text-cerulean-600 hover:underline mb-6">
        <ArrowLeftIcon className="size-4" />
        Back to lessons
      </Link>
      <div className="flex items-center justify-between mb-8">
        <h2 className="font-serif text-2xl font-semibold text-parchment-900 dark:text-white">Lesson Content</h2>
        {attempt ? (
          <Button className="bg-emerald-600 hover:bg-emerald-700 text-white" onClick={() => navigate(`/attempt/${attempt.id}`)}>
            Continue Lesson
          </Button>
        ) : (
          <div className="flex items-center gap-2">
            {hasCompleted && (
              <Badge variant="success" className="flex items-center gap-1">
                <CheckCircleIcon className="size-3" />
                Completed
              </Badge>
            )}
            <Button className="bg-cerulean-600 hover:bg-cerulean-700 text-white" onClick={handleStart} disabled={starting}>
              {starting ? "Starting..." : hasCompleted ? "Start Again" : "Start Lesson"}
            </Button>
          </div>
        )}
      </div>
      {error && (
        <p className="text-sm text-red-600 dark:text-red-400 bg-red-50 dark:bg-red-900/20 rounded-lg px-4 py-3 mb-6">
          {error}
        </p>
      )}

      {lessonAttempts.length > 0 && (
        <div className="mb-8">
          <h3 className="text-sm font-medium text-parchment-700 dark:text-slate-300 mb-3">Attempt History</h3>
          <div className="space-y-2">
            {lessonAttempts.map((a) => (
              <Card
                key={a.id}
                className="p-4 hover:border-cerulean-200 dark:hover:border-cerulean-700 hover:shadow-sm transition-all cursor-pointer"
                onClick={() => navigate(`/attempt/${a.id}`)}
              >
                <CardContent className="flex items-center justify-between">
                  <div className="flex items-center gap-3">
                    <Badge variant={a.status === "Completed" ? "success" : "secondary"}>{a.status}</Badge>
                    <span className="text-sm text-parchment-700 dark:text-slate-300">
                      {a.answeredCount} answered
                    </span>
                  </div>
                  <div className="flex items-center gap-2">
                    <span className="text-xs text-parchment-400 dark:text-slate-500">
                      {new Date(a.startedAt).toLocaleDateString()}
                      {a.completedAt && ` — ${new Date(a.completedAt).toLocaleDateString()}`}
                    </span>
                    <span className="text-xs text-cerulean-600 dark:text-cerulean-400">View</span>
                  </div>
                </CardContent>
              </Card>
            ))}
          </div>
        </div>
      )}

      <div className="space-y-6">
        {tree.nodes.map((node) => (
          <NodeRenderer key={node.id} node={node} isRoot attempt={attempt} />
        ))}
      </div>
    </div>
  );
}

function NodeRenderer({ node, isRoot = false, attempt }: { node: { depth: number; title: string; description: string; requiresPriorSiblingAnswered: boolean; children: typeof node[]; questions: QuestionDto[]; id: string }; isRoot?: boolean; attempt?: { answeredQuestionKeys: string[] } | null }) {
  const depthClasses: Record<number, string> = { 1: "ml-0", 2: "ml-6 border-l-2 border-parchment-200 dark:border-slate-700 pl-6", 3: "ml-12 border-l-2 border-parchment-200 dark:border-slate-700 pl-6" };

  return (
    <div className={isRoot ? "" : (depthClasses[node.depth] || "")}>
      <div className="mb-4">
        <div className="flex items-center gap-2">
          <h3 className="font-serif text-lg font-medium text-parchment-900 dark:text-white">{node.title}</h3>
          {node.requiresPriorSiblingAnswered && (
            <Badge variant="secondary" className="flex items-center gap-1">
              <LockClosedIcon className="size-3" />
              Gated
            </Badge>
          )}
        </div>
        {node.description && (
          <p className="text-sm text-parchment-600 dark:text-slate-400 mt-1 leading-relaxed">{node.description}</p>
        )}
      </div>
      {node.questions.length > 0 && (
        <div className="space-y-3 mb-4">
          {node.questions.map((q) => <QuestionCard key={q.id} question={q} attempt={attempt} />)}
        </div>
      )}
      {node.children.length > 0 && (
        <div className="space-y-4">
          {node.children.map((child) => <NodeRenderer key={child.id} node={child} attempt={attempt} />)}
        </div>
      )}
    </div>
  );
}

function QuestionCard({ question, attempt }: { question: QuestionDto; attempt?: { answeredQuestionKeys: string[] } | null }) {
  const isAnswered = attempt?.answeredQuestionKeys.includes(question.key);
  const typeLabel: Record<string, string> = { Essay: "Essay", YesNo: "Yes / No", TrueFalse: "True / False", FillBlank: "Fill in the Blank", SelectEmbedded: "Multiple Choice" };

  return (
    <Card className={`p-4 ${isAnswered ? "border-emerald-300 dark:border-emerald-700 bg-emerald-50/50 dark:bg-emerald-950/20" : ""}`}>
      <CardContent className="flex flex-col gap-2">
        <div className="flex items-center gap-2">
          <QuestionMarkCircleIcon className="size-4 text-parchment-400" />
          <span className="text-xs font-medium text-parchment-500 dark:text-slate-400 uppercase tracking-wider">{typeLabel[question.questionType] || question.questionType}</span>
          {isAnswered && (
            <Badge variant="success" className="flex items-center gap-1 ml-auto">
              <CheckBadgeIcon className="size-3" />
              Answered
            </Badge>
          )}
        </div>
        <p className="text-sm text-parchment-800 dark:text-slate-200 leading-relaxed">{question.promptText}</p>
      </CardContent>
    </Card>
  );
}
