import { useParams, Link, useNavigate } from "react-router-dom";
import { useState } from "react";
import { useGetLessonTreeQuery, type QuestionDto } from "../services/lessonsApi";
import { useStartAttemptMutation, useGetAttemptQuery } from "../services/studentApi";
import { LockClosedIcon, CheckBadgeIcon, QuestionMarkCircleIcon, ArrowLeftIcon } from "@heroicons/react/24/outline";
import { Button } from "../components/ui/button";
import { Card, CardContent } from "../components/ui/card";
import { Badge } from "../components/ui/badge";

export default function LessonDetailPage() {
  const { key } = useParams<{ key: string }>();
  const navigate = useNavigate();
  const { data: tree, isLoading } = useGetLessonTreeQuery(key!);
  const [startAttempt] = useStartAttemptMutation();
  const [attemptId, setAttemptId] = useState<string | null>(null);
  const { data: attempt } = useGetAttemptQuery(attemptId!, { skip: !attemptId });

  const handleStart = async () => {
    try {
      const result = await startAttempt(key!).unwrap();
      setAttemptId(result.id);
    } catch { /* ignored */ }
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
        {!attemptId ? (
          <Button className="bg-cerulean-600 hover:bg-cerulean-700 text-white" onClick={handleStart}>
            Start Lesson
          </Button>
        ) : (
          <Button className="bg-emerald-600 hover:bg-emerald-700 text-white" onClick={() => navigate(`/attempt/${attemptId}`)}>
            Continue Lesson
          </Button>
        )}
      </div>
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
