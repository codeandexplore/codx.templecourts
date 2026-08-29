import { useState, useMemo } from "react";
import { useParams, Link, useNavigate } from "react-router-dom";
import { ArrowLeftIcon, ArrowRightIcon, CheckCircleIcon, FlagIcon, LockClosedIcon } from "@heroicons/react/24/outline";
import { useGetSessionQuestionsQuery, useMarkReviewedMutation, useAdvanceSessionMutation, useEndSessionMutation } from "../services/sessionApi";
import { Badge } from "../components/ui/badge";
import { Button } from "../components/ui/button";
import { Card } from "../components/ui/card";
import ConfirmDialog from "../components/ConfirmDialog";

export default function ReviewPage() {
  const { sessionId } = useParams<{ sessionId: string }>();
  const navigate = useNavigate();
  const [currentIndex, setCurrentIndex] = useState(0);
  const [endDialog, setEndDialog] = useState(false);
  const [actionError, setActionError] = useState("");
  const [actionSuccess, setActionSuccess] = useState("");

  const { data, isLoading, isError, refetch } = useGetSessionQuestionsQuery(sessionId!);
  const [markReviewed, { isLoading: marking }] = useMarkReviewedMutation();
  const [advanceSession] = useAdvanceSessionMutation();
  const [endSession] = useEndSessionMutation();

  const currentQuestion = useMemo(() => {
    if (!data?.questions) return null;
    if (currentIndex >= data.questions.length) return null;
    return data.questions[currentIndex];
  }, [data, currentIndex]);

  if (isLoading) {
    return (
      <div className="py-12 text-center">
        <div className="size-8 mx-auto mb-3 rounded-full border-2 border-cerulean-500 border-t-transparent animate-spin" />
        <p className="text-parchment-500 dark:text-slate-400 text-sm">Loading session...</p>
      </div>
    );
  }

  if (isError || !data) {
    return (
      <div className="py-12 text-center">
        <p className="text-red-600 text-sm">Failed to load session.</p>
        <Button variant="ghost" onClick={() => navigate("/teacher")} className="mt-3">Back to Students</Button>
      </div>
    );
  }

  const handleMarkReviewed = async () => {
    if (!currentQuestion?.key || !data) return;
    setActionError("");
    setActionSuccess("");
    try {
      await markReviewed({ lessonAttemptId: data.lessonAttemptId, questionKey: currentQuestion.key }).unwrap();
      setActionSuccess("Answer marked as reviewed.");
    } catch (e: unknown) {
      const err = e as { data?: { message?: string; error?: string } };
      setActionError(err?.data?.message || err?.data?.error || "Failed to mark as reviewed.");
    } finally {
      await refetch();
    }
  };

  const handleAdvance = async () => {
    if (!currentQuestion || !sessionId || !data) return;
    const nextIndex = currentIndex + 1;
    if (nextIndex >= data.questions.length) return;
    setActionError("");
    setActionSuccess("");
    try {
      await advanceSession({ sessionId, currentQuestionId: data.questions[nextIndex].key }).unwrap();
      setCurrentIndex(nextIndex);
    } catch { /* handled by RTK */ }
  };

  const handleEndSession = async () => {
    if (!sessionId) return;
    try {
      await endSession(sessionId).unwrap();
      navigate("/teacher");
    } catch { /* handled by RTK */ }
  };

  const handleQuestionClick = (index: number) => {
    if (index === currentIndex) return;
    const q = data.questions[index];
    if (!q || !sessionId) return;
    setActionError("");
    setActionSuccess("");
    advanceSession({ sessionId, currentQuestionId: q.key })
      .unwrap()
      .then(() => setCurrentIndex(index))
      .catch(() => {});
  };

  return (
    <div className="flex gap-0 h-[calc(100vh-5rem)]">
      <QuestionMap
        questions={data.questions}
        currentIndex={currentIndex}
        onQuestionClick={handleQuestionClick}
      />

      <div className="flex-1 flex flex-col min-w-0">
        <ReviewHeader
          lessonNumber={data.lessonNumber}
          lessonTitle={data.lessonTitle}
          studentName={data.studentDisplayName}
          currentIndex={currentIndex}
          totalCount={data.questions.length}
          onEndSession={() => setEndDialog(true)}
        />

        <div className="flex-1 overflow-y-auto p-6">
          {currentQuestion ? (
            <div className="flex flex-col gap-6 max-w-2xl">
              <QuestionDisplay question={currentQuestion} />

              <AnswerDisplay
                answer={currentQuestion.answer}
                isReviewed={currentQuestion.isReviewed}
                flag={currentQuestion.flag}
              />

              {actionError && (
                <p className="text-sm text-red-600 dark:text-red-400 bg-red-50 dark:bg-red-900/20 rounded-lg px-4 py-3">
                  {actionError}
                </p>
              )}
              {actionSuccess && (
                <p className="text-sm text-emerald-600 dark:text-emerald-400 bg-emerald-50 dark:bg-emerald-900/20 rounded-lg px-4 py-3">
                  {actionSuccess}
                </p>
              )}

              <ReviewControls
                currentIndex={currentIndex}
                totalCount={data.questions.length}
                isReviewed={currentQuestion.isReviewed}
                marking={marking}
                currentQuestionKey={currentQuestion.key}
                onMarkReviewed={handleMarkReviewed}
                onAdvance={handleAdvance}
              />
            </div>
          ) : (
            <div className="py-12 text-center">
              <p className="text-parchment-500 dark:text-slate-400">All questions reviewed.</p>
              <Button onClick={() => setEndDialog(true)} className="mt-4 bg-cerulean-600 hover:bg-cerulean-700 text-white">
                End Session
              </Button>
            </div>
          )}
        </div>
      </div>

      <ConfirmDialog
        open={endDialog}
        onOpenChange={setEndDialog}
        title="End Session"
        description="Are you sure you want to end this review session?"
        confirmLabel="End Session"
        variant="destructive"
        onConfirm={handleEndSession}
      />
    </div>
  );
}

function QuestionMap({
  questions,
  currentIndex,
  onQuestionClick,
}: {
  questions: { key: string; order: number; isReviewed: boolean; flag: { type: string } | null }[];
  currentIndex: number;
  onQuestionClick: (index: number) => void;
}) {
  return (
    <div className="w-[280px] shrink-0 border-r border-parchment-200 dark:border-slate-700 bg-white dark:bg-slate-900 overflow-y-auto p-4">
      <p className="text-xs font-medium text-parchment-400 dark:text-slate-500 uppercase tracking-wide mb-3">Questions</p>
      <div className="space-y-1">
        {questions.map((q, i) => {
          const isCurrent = i === currentIndex;
          const isFlagged = !!q.flag;
          return (
            <button
              key={q.key}
              onClick={() => onQuestionClick(i)}
              className={`w-full flex items-center gap-2 px-3 py-2 rounded-lg text-sm text-left transition-colors ${
                isCurrent
                  ? "bg-cerulean-100 dark:bg-cerulean-900/30 text-cerulean-700 dark:text-cerulean-300 font-medium"
                  : q.isReviewed
                  ? "text-parchment-400 dark:text-slate-500 hover:bg-parchment-50 dark:hover:bg-slate-800"
                  : "text-parchment-700 dark:text-slate-300 hover:bg-parchment-50 dark:hover:bg-slate-800"
              }`}
            >
              <StatusDot reviewed={q.isReviewed} flagged={isFlagged} current={isCurrent} />
              <span className="truncate flex-1">#{q.order + 1}</span>
              {isFlagged && <FlagIcon className="size-3.5 text-gold-500 shrink-0" />}
              {q.isReviewed && <CheckCircleIcon className="size-3.5 text-emerald-500 shrink-0" />}
            </button>
          );
        })}
      </div>
    </div>
  );
}

function StatusDot({ reviewed, flagged, current }: { reviewed: boolean; flagged: boolean; current: boolean }) {
  if (current) return <div className="size-2 rounded-full bg-cerulean-500 ring-2 ring-cerulean-200 dark:ring-cerulean-800 shrink-0" />;
  if (reviewed) return <div className="size-2 rounded-full bg-emerald-400 shrink-0" />;
  if (flagged) return <div className="size-2 rounded-full bg-gold-400 shrink-0" />;
  return <div className="size-2 rounded-full border border-parchment-300 dark:border-slate-600 shrink-0" />;
}

function ReviewHeader({
  lessonNumber,
  lessonTitle,
  studentName,
  currentIndex,
  totalCount,
  onEndSession,
}: {
  lessonNumber: number;
  lessonTitle: string;
  studentName: string;
  currentIndex: number;
  totalCount: number;
  onEndSession: () => void;
}) {
  return (
    <div className="p-5 border-b border-parchment-200 dark:border-slate-700 bg-white dark:bg-slate-900 shrink-0">
      <div className="flex items-center justify-between mb-1">
        <Link to="/teacher" className="inline-flex items-center gap-1 text-sm text-parchment-500 dark:text-slate-400 hover:text-parchment-700 dark:hover:text-slate-200 transition-colors">
          <ArrowLeftIcon className="size-3.5" />
          Back to Students
        </Link>
        <div className="flex items-center gap-3">
          <Badge variant="secondary">{currentIndex + 1} of {totalCount}</Badge>
          <Button variant="ghost" size="sm" onClick={onEndSession}>
            <LockClosedIcon className="size-3.5 mr-1" />
            End Session
          </Button>
        </div>
      </div>
      <h2 className="font-serif text-lg font-semibold text-parchment-900 dark:text-white mt-1">
        Lesson {lessonNumber}: {lessonTitle}
      </h2>
      <p className="text-sm text-parchment-500 dark:text-slate-400">Student: {studentName}</p>
    </div>
  );
}

function QuestionDisplay({
  question,
}: {
  question: { parentNodeTitle: string; questionType: string; promptText: string };
}) {
  return (
    <div className="space-y-3">
      <div className="flex items-center gap-2 text-xs text-parchment-500 dark:text-slate-400">
        <span>{question.parentNodeTitle}</span>
      </div>
      <Badge variant="secondary" className="text-[10px]">{question.questionType}</Badge>
      <p className="font-serif text-xl text-parchment-900 dark:text-white leading-relaxed">{question.promptText}</p>
    </div>
  );
}

function AnswerDisplay({
  answer,
  isReviewed,
  flag,
}: {
  answer: { value: string } | null;
  isReviewed: boolean;
  flag: { type: string } | null;
}) {
  return (
    <Card className={`p-5 ${isReviewed ? "border-emerald-200 dark:border-emerald-800" : ""}`}>
      <div className="flex items-center justify-between mb-3">
        <span className="text-xs font-medium text-parchment-400 dark:text-slate-500 uppercase tracking-wide">Student Answer</span>
        {isReviewed && (
          <Badge variant="success" className="text-[10px]">
            <CheckCircleIcon className="size-3 mr-1" />
            Reviewed
          </Badge>
        )}
        {flag && (
          <Badge variant="warning" className="text-[10px]">
            <FlagIcon className="size-3 mr-1" />
            {flag.type}
          </Badge>
        )}
      </div>
      {answer ? (
        <p className="text-sm text-parchment-800 dark:text-slate-200 whitespace-pre-wrap leading-relaxed">{answer.value}</p>
      ) : (
        <div className="py-6 text-center">
          <div className="size-10 mx-auto mb-2 rounded-full bg-parchment-100 dark:bg-slate-800 flex items-center justify-center">
            <span className="text-parchment-400 dark:text-slate-500 text-lg">?</span>
          </div>
          <p className="text-sm text-parchment-400 dark:text-slate-500">Not yet answered</p>
        </div>
      )}
    </Card>
  );
}

function ReviewControls({
  currentIndex,
  totalCount,
  isReviewed,
  marking,
  currentQuestionKey,
  onMarkReviewed,
  onAdvance,
}: {
  currentIndex: number;
  totalCount: number;
  isReviewed: boolean;
  marking: boolean;
  currentQuestionKey: string;
  onMarkReviewed: () => void;
  onAdvance: () => void;
}) {
  const isLast = currentIndex >= totalCount - 1;

  return (
    <div className="flex items-center gap-3 pt-4 border-t border-parchment-100 dark:border-slate-800">
      <Button
        variant={isReviewed ? "ghost" : "default"}
        onClick={onMarkReviewed}
        disabled={marking || !currentQuestionKey}
        className={isReviewed ? "" : "bg-cerulean-600 hover:bg-cerulean-700 text-white"}
      >
        {marking ? "Marking..." : isReviewed ? "Reviewed" : "Mark Reviewed"}
      </Button>

      <Button
        variant="ghost"
        onClick={onAdvance}
        disabled={isLast}
        className={isLast ? "" : "text-cerulean-600 dark:text-cerulean-400"}
      >
        Advance
        <ArrowRightIcon className="size-4 ml-1" />
      </Button>
    </div>
  );
}
