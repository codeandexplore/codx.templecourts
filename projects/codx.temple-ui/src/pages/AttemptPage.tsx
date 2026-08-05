import { useParams, Link } from "react-router-dom";
import { useState } from "react";
import { useGetAttemptQuery, useSubmitAnswerMutation } from "../services/studentApi";
import { useGetLessonTreeQuery, type QuestionDto } from "../services/lessonsApi";

export default function AttemptPage() {
  const { attemptId } = useParams<{ attemptId: string }>();
  const { data: attempt, isLoading } = useGetAttemptQuery(attemptId!);
  const [submitAnswer] = useSubmitAnswerMutation();
  const { data: tree } = useGetLessonTreeQuery(attempt?.lessonKey ?? "", { skip: !attempt });
  const [activeQuestion, setActiveQuestion] = useState<string | null>(null);
  const [answer, setAnswer] = useState("");
  const [submitting, setSubmitting] = useState(false);

  if (isLoading || !attempt) return <div className="text-gray-600">Loading...</div>;

  const allQuestions = flattenQuestions(tree?.nodes ?? []);

  const handleSubmit = async (q: QuestionDto) => {
    if (!answer.trim()) return;
    setSubmitting(true);
    try {
      await submitAnswer({ attemptId: attemptId!, questionKey: q.key, answerValue: answer }).unwrap();
      setAnswer("");
      setActiveQuestion(null);
    } catch { /* ignored */ }
    setSubmitting(false);
  };

  const completed = allQuestions.filter(q => attempt.answeredQuestionKeys.includes(q.key)).length;

  return (
    <div>
      <Link to={`/lessons/${attempt.lessonKey}`} className="text-sm text-blue-600 hover:underline mb-4 inline-block">&larr; Back to lesson</Link>
      <div className="flex items-center justify-between mb-6">
        <h2 className="text-2xl font-semibold text-gray-900 dark:text-white">Lesson Runner</h2>
        <span className="text-sm text-gray-600 dark:text-gray-400">{completed} / {allQuestions.length} answered</span>
      </div>
      <div className="space-y-4">
        {allQuestions.map((q, i) => {
          const answered = attempt.answeredQuestionKeys.includes(q.key);
          const isActive = activeQuestion === q.key;
          return (
            <div key={q.key} className={`rounded-lg border p-4 ${answered ? "border-green-300 dark:border-green-700 bg-green-50 dark:bg-green-950" : "border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-900"}`}>
              <div className="flex items-center justify-between mb-2">
                <span className="text-xs font-medium text-gray-500 uppercase">Q{i + 1} &middot; {q.questionType}</span>
                {answered && <span className="text-xs text-green-600">Answered</span>}
              </div>
              <p className="text-sm text-gray-800 dark:text-gray-200 mb-3">{q.promptText}</p>
              {isActive ? (
                <div className="space-y-2">
                  <textarea value={answer} onChange={e => setAnswer(e.target.value)} className="w-full rounded-lg border border-gray-300 dark:border-gray-700 bg-white dark:bg-gray-800 px-3 py-2 text-sm text-gray-900 dark:text-white" rows={3} placeholder="Type your answer..." />
                  <div className="flex gap-2">
                    <button onClick={() => handleSubmit(q)} disabled={submitting || !answer.trim()} className="rounded-lg bg-blue-600 px-3 py-1 text-sm text-white hover:bg-blue-700 disabled:opacity-50">
                      {submitting ? "Saving..." : "Submit"}
                    </button>
                    <button onClick={() => { setActiveQuestion(null); setAnswer(""); }} className="rounded-lg px-3 py-1 text-sm text-gray-600 hover:bg-gray-100 dark:hover:bg-gray-800">Cancel</button>
                  </div>
                </div>
              ) : (
                <button onClick={() => setActiveQuestion(q.key)} className="text-sm text-blue-600 hover:underline">
                  {answered ? "Edit answer" : "Answer"}
                </button>
              )}
            </div>
          );
        })}
      </div>
    </div>
  );
}

function flattenQuestions(nodes: { key: string; questions: QuestionDto[]; children: typeof nodes }[]): QuestionDto[] {
  const result: QuestionDto[] = [];
  for (const node of nodes) {
    result.push(...node.questions);
    result.push(...flattenQuestions(node.children));
  }
  return result;
}
