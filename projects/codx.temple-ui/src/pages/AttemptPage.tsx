import { useParams, Link } from "react-router-dom";
import { useState, useEffect } from "react";
import { useGetAttemptQuery, useSubmitAnswerMutation } from "../services/studentApi";
import { useGetLessonTreeQuery, type QuestionDto } from "../services/lessonsApi";
import { useGetSessionQuestionsQuery } from "../services/sessionApi";
import { CheckBadgeIcon, PencilIcon, ArrowLeftIcon, QuestionMarkCircleIcon, EyeIcon } from "@heroicons/react/24/outline";
import { Button } from "../components/ui/button";
import { Card, CardContent } from "../components/ui/card";
import { Badge } from "../components/ui/badge";
import { Textarea } from "../components/ui/textarea";
import { useSessionHub } from "../hooks/useSessionHub";

function ReviewBanner({ sessionId }: { sessionId: string }) {
  const { data } = useGetSessionQuestionsQuery(sessionId);
  const [ended, setEnded] = useState(false);
  const [currentQuestionId, setCurrentQuestionId] = useState<string | null>(null);

  useEffect(() => {
    if (data?.currentQuestionId) {
      setCurrentQuestionId(data.currentQuestionId);
    }
  }, [data?.currentQuestionId]);

  useSessionHub(sessionId, {
    onSessionAdvanced: (questionId) => {
      setCurrentQuestionId(questionId);
    },
    onSessionEnded: () => {
      setEnded(true);
    },
  });

  if (ended) return null;

  const total = data?.questions.length ?? 0;
  const currentIndex = currentQuestionId
    ? data?.questions.findIndex((q) => q.key === currentQuestionId) ?? -1
    : -1;

  return (
    <div className="mb-6 p-4 rounded-xl bg-cerulean-50 dark:bg-cerulean-900/20 border border-cerulean-200 dark:border-cerulean-700 flex items-center gap-3">
      <EyeIcon className="size-5 text-cerulean-600 dark:text-cerulean-400 shrink-0" />
      <div className="flex-1">
        <p className="text-sm font-medium text-cerulean-700 dark:text-cerulean-300">
          Teacher is reviewing your answers
          {currentIndex >= 0 && (
            <span className="ml-2 font-semibold">Question {currentIndex + 1} of {total}</span>
          )}
        </p>
        <p className="text-xs text-cerulean-500 dark:text-cerulean-400">
          Your answers are being reviewed in a live session.
        </p>
      </div>
    </div>
  );
}

export default function AttemptPage() {
  const { attemptId } = useParams<{ attemptId: string }>();
  const { data: attempt, isLoading } = useGetAttemptQuery(attemptId!);
  const [submitAnswer] = useSubmitAnswerMutation();
  const { data: tree } = useGetLessonTreeQuery(attempt?.lessonKey ?? "", { skip: !attempt });
  const [activeQuestion, setActiveQuestion] = useState<string | null>(null);
  const [answer, setAnswer] = useState("");
  const [submitting, setSubmitting] = useState(false);

  if (isLoading || !attempt) return <div className="text-parchment-500 dark:text-slate-400">Loading...</div>;

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
      {attempt.activeSessionId && <ReviewBanner sessionId={attempt.activeSessionId} />}
      <Link to={`/lessons/${attempt.lessonKey}`} className="inline-flex items-center gap-1 text-sm text-cerulean-600 hover:underline mb-6">
        <ArrowLeftIcon className="size-4" />
        Back to lesson
      </Link>
      <div className="flex items-center justify-between mb-8">
        <h2 className="font-serif text-2xl font-semibold text-parchment-900 dark:text-white">Lesson Runner</h2>
        <Badge variant={completed === allQuestions.length ? "success" : "secondary"} className="text-sm px-3 py-1">
          {completed} / {allQuestions.length} answered
        </Badge>
      </div>
      <div className="space-y-4">
        {allQuestions.map((q, i) => {
          const answered = attempt.answeredQuestionKeys.includes(q.key);
          const isActive = activeQuestion === q.key;
          return (
            <Card key={q.key} className={`p-5 ${answered ? "border-emerald-300 dark:border-emerald-700 bg-emerald-50/30 dark:bg-emerald-950/15" : ""}`}>
              <CardContent className="flex flex-col gap-3">
                <div className="flex items-center justify-between">
                  <div className="flex items-center gap-2">
                    <QuestionMarkCircleIcon className="size-4 text-parchment-400" />
                    <span className="text-xs font-medium text-parchment-500 dark:text-slate-400 uppercase tracking-wider">Q{i + 1} &middot; {q.questionType}</span>
                  </div>
                  {answered && (
                    <Badge variant="success" className="flex items-center gap-1">
                      <CheckBadgeIcon className="size-3" />
                      Answered
                    </Badge>
                  )}
                </div>
                <p className="text-sm text-parchment-800 dark:text-slate-200 leading-relaxed">{q.promptText}</p>
                {isActive ? (
                  <div className="space-y-3 pt-2">
                    <Textarea value={answer} onChange={e => setAnswer(e.target.value)} className="min-h-[100px]" placeholder="Type your answer..." />
                    <div className="flex gap-2">
                      <Button className="bg-cerulean-600 hover:bg-cerulean-700 text-white" disabled={submitting || !answer.trim()} onClick={() => handleSubmit(q)}>
                        {submitting ? "Saving..." : "Submit"}
                      </Button>
                      <Button variant="ghost" onClick={() => { setActiveQuestion(null); setAnswer(""); }}>
                        Cancel
                      </Button>
                    </div>
                  </div>
                ) : (
                  <Button variant="ghost" className="justify-start gap-1.5 w-fit h-auto py-1 px-0 text-cerulean-600 hover:text-cerulean-700" onClick={() => setActiveQuestion(q.key)}>
                    <PencilIcon className="size-4" />
                    {answered ? "Edit answer" : "Answer"}
                  </Button>
                )}
              </CardContent>
            </Card>
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
