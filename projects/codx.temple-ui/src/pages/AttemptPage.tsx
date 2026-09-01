import { useParams, Link } from "react-router-dom";
import { useState } from "react";
import { useGetAttemptQuery, useSubmitAnswerMutation, useCompleteAttemptMutation, useGetAttemptAnswersQuery, useGetNotesQuery } from "../services/studentApi";
import { useGetLessonTreeQuery, type QuestionDto } from "../services/lessonsApi";
import { useGetSessionQuestionsQuery } from "../services/sessionApi";
import { CheckBadgeIcon, PencilIcon, ArrowLeftIcon, QuestionMarkCircleIcon, EyeIcon } from "@heroicons/react/24/outline";
import { Button } from "../components/ui/button";
import { Card, CardContent } from "../components/ui/card";
import { Badge } from "../components/ui/badge";
import QuestionAnswerInput from "../components/QuestionAnswerInput";
import QuestionNote from "../components/QuestionNote";
import ThreadPanel from "../components/ThreadPanel";
import ConfirmDialog from "../components/ConfirmDialog";
import { useSessionHub } from "../hooks/useSessionHub";

function ReviewBanner({ sessionId }: { sessionId: string }) {
  const { data } = useGetSessionQuestionsQuery(sessionId);
  const [ended, setEnded] = useState(false);
  const [liveQuestionId, setLiveQuestionId] = useState<string | null>(null);

  useSessionHub(sessionId, {
    onSessionAdvanced: (questionId) => {
      setLiveQuestionId(questionId);
    },
    onSessionEnded: () => {
      setEnded(true);
    },
  });

  if (ended) return null;

  const currentQuestionId = liveQuestionId ?? data?.currentQuestionId ?? null;
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
  const [completeAttempt] = useCompleteAttemptMutation();
  const { data: tree } = useGetLessonTreeQuery(attempt?.lessonKey ?? "", { skip: !attempt });
  const { data: answers } = useGetAttemptAnswersQuery(attemptId!, { skip: !attempt });
  const { data: notes } = useGetNotesQuery();
  const [activeQuestion, setActiveQuestion] = useState<string | null>(null);
  const [answer, setAnswer] = useState("");
  const [submitting, setSubmitting] = useState(false);
  const [finishDialog, setFinishDialog] = useState(false);

  if (isLoading || !attempt) return <div className="text-parchment-500 dark:text-slate-400">Loading...</div>;

  const allQuestions = flattenQuestions(tree?.nodes ?? []);
  const isCompleted = attempt.status === "Completed";
  const answersByKey = new Map((answers ?? []).map((a) => [a.questionKey, String(a.answerValue)]));
  const threadByKey = new Map((answers ?? []).filter((a) => a.threadId).map((a) => [a.questionKey, a.threadId!]));
  const notesByKey = new Map((notes ?? []).map((n) => [n.questionKey, n.noteText]));
  const completed = allQuestions.filter((q) => attempt.answeredQuestionKeys.includes(q.key)).length;

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

  const handleStartAnswer = (q: QuestionDto) => {
    setAnswer(answersByKey.get(q.key) ?? "");
    setActiveQuestion(q.key);
  };

  const handleFinish = async () => {
    setFinishDialog(false);
    try {
      await completeAttempt(attemptId!).unwrap();
    } catch { /* handled by RTK */ }
  };

  return (
    <div>
      {attempt.activeSessionId && <ReviewBanner sessionId={attempt.activeSessionId} />}
      <Link to={`/lessons/${attempt.lessonKey}`} className="inline-flex items-center gap-1 text-sm text-cerulean-600 hover:underline mb-6">
        <ArrowLeftIcon className="size-4" />
        Back to lesson
      </Link>
      <div className="flex items-center justify-between mb-8">
        <h2 className="font-serif text-2xl font-semibold text-parchment-900 dark:text-white">Lesson Runner</h2>
        <div className="flex items-center gap-3">
          <Badge variant={completed === allQuestions.length ? "success" : "secondary"} className="text-sm px-3 py-1">
            {completed} / {allQuestions.length} answered
          </Badge>
          {!isCompleted && (
            <Button variant="outline" onClick={() => setFinishDialog(true)}>
              Finish Lesson
            </Button>
          )}
        </div>
      </div>

      {isCompleted && (
        <div className="mb-6 p-4 rounded-xl bg-emerald-50 dark:bg-emerald-900/20 border border-emerald-200 dark:border-emerald-700 flex items-center gap-3">
          <CheckBadgeIcon className="size-5 text-emerald-600 dark:text-emerald-400 shrink-0" />
          <div className="flex-1">
            <p className="text-sm font-medium text-emerald-700 dark:text-emerald-300">Lesson complete</p>
            <p className="text-xs text-emerald-500 dark:text-emerald-400">{completed} of {allQuestions.length} questions answered</p>
          </div>
          <Link to="/lessons" className="text-sm text-cerulean-600 hover:underline">Back to Lessons</Link>
        </div>
      )}

      <div className="space-y-4">
        {allQuestions.map((q, i) => {
          const answered = attempt.answeredQuestionKeys.includes(q.key);
          const isActive = activeQuestion === q.key;
          const existingAnswer = answersByKey.get(q.key);
          const note = notesByKey.get(q.key) ?? null;
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

                {isCompleted ? (
                  existingAnswer !== undefined && (
                    <div className="text-sm text-parchment-700 dark:text-slate-300 bg-parchment-50 dark:bg-slate-800/50 rounded-lg px-3 py-2">
                      {existingAnswer}
                    </div>
                  )
                ) : isActive ? (
                  <div className="space-y-3 pt-2">
                    <QuestionAnswerInput question={q} value={answer} onChange={setAnswer} />
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
                  <Button variant="ghost" className="justify-start gap-1.5 w-fit h-auto py-1 px-0 text-cerulean-600 hover:text-cerulean-700" onClick={() => handleStartAnswer(q)}>
                    <PencilIcon className="size-4" />
                    {answered ? "Edit answer" : "Answer"}
                  </Button>
                )}

                <QuestionNote questionKey={q.key} existingNote={note} disabled={isCompleted} />
                {threadByKey.has(q.key) && <ThreadPanel threadId={threadByKey.get(q.key)!} />}
              </CardContent>
            </Card>
          );
        })}
      </div>

      <ConfirmDialog
        open={finishDialog}
        onOpenChange={setFinishDialog}
        title="Finish Lesson"
        description={`You have answered ${completed} of ${allQuestions.length} questions. Finish the lesson now?`}
        confirmLabel="Finish"
        variant="default"
        onConfirm={handleFinish}
      />
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