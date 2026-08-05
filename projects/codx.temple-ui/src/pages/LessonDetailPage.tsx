import { useParams, Link } from "react-router-dom";
import { useGetLessonTreeQuery, type LessonNodeDto, type QuestionDto } from "../services/lessonsApi";

export default function LessonDetailPage() {
  const { key } = useParams<{ key: string }>();
  const { data: tree, isLoading, error } = useGetLessonTreeQuery(key!);

  if (isLoading) return <div className="text-gray-600">Loading lesson...</div>;
  if (error) return <div className="text-red-600">Failed to load lesson.</div>;
  if (!tree) return null;

  return (
    <div>
      <Link to="/lessons" className="text-sm text-blue-600 hover:underline mb-4 inline-block">&larr; Back to lessons</Link>
      <h2 className="text-2xl font-semibold text-gray-900 dark:text-white mb-6">Lesson Content</h2>
      <div className="space-y-6">
        {tree.nodes.map((node) => (
          <NodeRenderer key={node.id} node={node} isRoot />
        ))}
      </div>
    </div>
  );
}

function NodeRenderer({ node, isRoot = false }: { node: LessonNodeDto; isRoot?: boolean }) {
  const depthClasses = {
    1: "ml-0 border-l-0",
    2: "ml-6 border-l-2 border-gray-200 dark:border-gray-700 pl-4",
    3: "ml-12 border-l-2 border-gray-200 dark:border-gray-700 pl-4",
  };

  return (
    <div className={isRoot ? "" : (depthClasses[node.depth as keyof typeof depthClasses] || "")}>
      <div className="mb-3">
        <h3 className="text-lg font-medium text-gray-900 dark:text-white">
          {node.title}
          {node.requiresPriorSiblingAnswered && (
            <span className="ml-2 text-xs bg-blue-100 text-blue-700 dark:bg-blue-900 dark:text-blue-300 px-2 py-0.5 rounded">Gated</span>
          )}
        </h3>
        <p className="text-sm text-gray-600 dark:text-gray-400 mt-1">{node.description}</p>
      </div>
      {node.questions.length > 0 && (
        <div className="space-y-3 mb-4">
          {node.questions.map((q) => (
            <QuestionCard key={q.id} question={q} />
          ))}
        </div>
      )}
      {node.children.length > 0 && (
        <div className="space-y-4">
          {node.children.map((child) => (
            <NodeRenderer key={child.id} node={child} />
          ))}
        </div>
      )}
    </div>
  );
}

function QuestionCard({ question }: { question: QuestionDto }) {
  const typeLabel: Record<string, string> = {
    Essay: "Essay",
    YesNo: "Yes / No",
    TrueFalse: "True / False",
    FillBlank: "Fill in the Blank",
    SelectEmbedded: "Multiple Choice",
  };

  return (
    <div className="rounded-lg border border-gray-200 dark:border-gray-700 bg-gray-50 dark:bg-gray-800 p-3">
      <div className="flex items-center gap-2 mb-1">
        <span className="text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">
          {typeLabel[question.questionType] || question.questionType}
        </span>
      </div>
      <p className="text-sm text-gray-800 dark:text-gray-200">{question.promptText}</p>
    </div>
  );
}
