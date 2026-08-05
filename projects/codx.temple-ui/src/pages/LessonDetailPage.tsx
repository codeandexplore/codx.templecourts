import { useParams, Link, useNavigate } from "react-router-dom";
import { useState } from "react";
import { useGetLessonTreeQuery, type QuestionDto } from "../services/lessonsApi";
import { useStartAttemptMutation, useGetAttemptQuery } from "../services/studentApi";

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

  if (isLoading) return <div className="text-gray-600">Loading lesson...</div>;
  if (!tree) return null;

  return (
    <div>
      <Link to="/lessons" className="text-sm text-blue-600 hover:underline mb-4 inline-block">&larr; Back to lessons</Link>
      <div className="flex items-center justify-between mb-6">
        <h2 className="text-2xl font-semibold text-gray-900 dark:text-white">Lesson Content</h2>
        {!attemptId ? (
          <button onClick={handleStart} className="rounded-lg bg-blue-600 px-4 py-2 text-sm text-white font-medium hover:bg-blue-700">
            Start Lesson
          </button>
        ) : (
          <button onClick={() => navigate(`/attempt/${attemptId}`)} className="rounded-lg bg-green-600 px-4 py-2 text-sm text-white font-medium hover:bg-green-700">
            Continue Lesson
          </button>
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
  const depthClasses: Record<number, string> = { 1: "ml-0", 2: "ml-6 border-l-2 border-gray-200 dark:border-gray-700 pl-4", 3: "ml-12 border-l-2 border-gray-200 dark:border-gray-700 pl-4" };

  return (
    <div className={isRoot ? "" : (depthClasses[node.depth] || "")}>
      <div className="mb-3">
        <h3 className="text-lg font-medium text-gray-900 dark:text-white">
          {node.title}
          {node.requiresPriorSiblingAnswered && <span className="ml-2 text-xs bg-blue-100 text-blue-700 dark:bg-blue-900 dark:text-blue-300 px-2 py-0.5 rounded">Gated</span>}
        </h3>
        <p className="text-sm text-gray-600 dark:text-gray-400 mt-1">{node.description}</p>
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
    <div className={`rounded-lg border p-3 ${isAnswered ? "border-green-300 dark:border-green-700 bg-green-50 dark:bg-green-950" : "border-gray-200 dark:border-gray-700 bg-gray-50 dark:bg-gray-800"}`}>
      <div className="flex items-center gap-2 mb-1">
        <span className="text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">{typeLabel[question.questionType] || question.questionType}</span>
        {isAnswered && <span className="text-xs text-green-600 dark:text-green-400">Answered</span>}
      </div>
      <p className="text-sm text-gray-800 dark:text-gray-200">{question.promptText}</p>
    </div>
  );
}
