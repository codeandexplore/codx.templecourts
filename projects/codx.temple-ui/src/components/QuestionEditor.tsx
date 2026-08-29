import { useState } from "react";
import SlideOutPanel from "./SlideOutPanel";
import { useCreateQuestionMutation, useUpdateQuestionMutation, useDeleteQuestionMutation } from "../services/questionsApi";
import ConfirmDialog from "./ConfirmDialog";

const QUESTION_TYPES = [
  { value: "Essay", label: "Essay" },
  { value: "YesNo", label: "Yes/No" },
  { value: "TrueFalse", label: "True/False" },
  { value: "FillBlank", label: "Fill in the Blank" },
  { value: "SelectEmbedded", label: "Multiple Choice" },
];

interface QuestionEditorProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  nodeKey: string;
  existingQuestion: {
    questionKey: string;
    questionType: string;
    promptText: string;
    metadata?: Record<string, unknown>;
    referenceContext?: Record<string, unknown>;
  } | null;
}

export default function QuestionEditor({
  open,
  onOpenChange,
  nodeKey,
  existingQuestion,
}: QuestionEditorProps) {
  const [createQuestion] = useCreateQuestionMutation();
  const [updateQuestion] = useUpdateQuestionMutation();
  const [deleteQuestion] = useDeleteQuestionMutation();

  const [questionType, setQuestionType] = useState(existingQuestion?.questionType || "Essay");
  const [promptText, setPromptText] = useState(existingQuestion?.promptText || "");
  const [options, setOptions] = useState<string[]>(
    existingQuestion?.questionType === "SelectEmbedded"
      ? ((existingQuestion.metadata as { options?: string[] } | undefined)?.options ?? [])
      : []
  );
  const [newOption, setNewOption] = useState("");
  const [referenceContext, setReferenceContext] = useState(
    existingQuestion?.referenceContext
      ? JSON.stringify(existingQuestion.referenceContext, null, 2)
      : ""
  );
  const [confirmDelete, setConfirmDelete] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");

  const isNew = !existingQuestion;

  const addOption = () => {
    if (!newOption.trim()) return;
    setOptions([...options, newOption.trim()]);
    setNewOption("");
  };

  const handleSave = async () => {
    if (!promptText.trim()) {
      setError("Prompt text is required");
      return;
    }
    setError("");
    setSaving(true);

    let refCtx: Record<string, unknown> | undefined;
    if (referenceContext.trim()) {
      try {
        refCtx = JSON.parse(referenceContext);
      } catch {
        setError("Reference context must be valid JSON");
        setSaving(false);
        return;
      }
    }

    let metadata: Record<string, unknown> | undefined;
    if (questionType === "SelectEmbedded") {
      const cleanOptions = options.map((o) => o.trim()).filter((o) => o.length > 0);
      if (cleanOptions.length === 0) {
        setError("Multiple choice questions need at least one option");
        setSaving(false);
        return;
      }
      metadata = { options: cleanOptions };
    }

    try {
      if (isNew) {
        await createQuestion({
          nodeKey,
          questionType,
          promptText: promptText.trim(),
          metadata,
          referenceContext: refCtx,
        }).unwrap();
      } else {
        await updateQuestion({
          nodeKey,
          questionKey: existingQuestion.questionKey,
          promptText: promptText.trim(),
          metadata,
          referenceContext: refCtx,
        }).unwrap();
      }
      onOpenChange(false);
    } catch {
      setError("Failed to save question");
    }
    setSaving(false);
  };

  const handleDelete = async () => {
    if (!existingQuestion) return;
    try {
      await deleteQuestion({ nodeKey, questionKey: existingQuestion.questionKey }).unwrap();
      setConfirmDelete(false);
      onOpenChange(false);
    } catch {
      setError("Failed to delete question");
    }
  };

  return (
    <>
      <SlideOutPanel
        open={open}
        onOpenChange={onOpenChange}
        title={isNew ? "Add Question" : "Edit Question"}
      >
        <div className="flex flex-col gap-5">
          {error && <p className="text-sm text-red-600 bg-red-50 dark:bg-red-900/20 rounded-lg p-2">{error}</p>}

          {/* Type selector */}
          <div className="flex flex-col gap-1.5">
            <label htmlFor="q-type" className="text-sm font-medium text-parchment-700 dark:text-slate-300">Question Type</label>
            <select
              id="q-type"
              value={questionType}
              onChange={(e) => setQuestionType(e.target.value)}
              className="w-full rounded-lg border border-parchment-200 dark:border-slate-700 bg-white dark:bg-slate-900 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-cerulean-500 dark:text-white"
            >
              {QUESTION_TYPES.map((t) => (
                <option key={t.value} value={t.value}>{t.label}</option>
              ))}
            </select>
            <p className="text-xs text-parchment-400 dark:text-slate-500">
              Select the response format for this question.
            </p>
          </div>

          {/* Prompt */}
          <div className="flex flex-col gap-1.5">
            <label htmlFor="q-prompt" className="text-sm font-medium text-parchment-700 dark:text-slate-300">Prompt</label>
            <input
              id="q-prompt"
              type="text"
              value={promptText}
              onChange={(e) => setPromptText(e.target.value)}
              placeholder="e.g., What does this passage teach us about..."
              className="w-full rounded-lg border border-parchment-200 dark:border-slate-700 bg-white dark:bg-slate-900 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-cerulean-500 dark:text-white"
            />
          </div>

          {/* Options (multiple choice) */}
          {questionType === "SelectEmbedded" && (
            <div className="flex flex-col gap-1.5">
              <label className="text-sm font-medium text-parchment-700 dark:text-slate-300">Options</label>
              <div className="space-y-2">
                {options.map((opt, idx) => (
                  <div key={idx} className="flex items-center gap-2">
                    <input
                      type="text"
                      value={opt}
                      onChange={(e) => {
                        const next = [...options];
                        next[idx] = e.target.value;
                        setOptions(next);
                      }}
                      className="flex-1 rounded-lg border border-parchment-200 dark:border-slate-700 bg-white dark:bg-slate-900 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-cerulean-500 dark:text-white"
                      placeholder="Option text"
                    />
                    <button
                      type="button"
                      onClick={() => setOptions(options.filter((_, i) => i !== idx))}
                      className="p-2 text-parchment-400 dark:text-slate-500 hover:text-red-600 dark:hover:text-red-400"
                      title="Remove option"
                    >
                      &times;
                    </button>
                  </div>
                ))}
              </div>
              <div className="flex gap-2">
                <input
                  type="text"
                  value={newOption}
                  onChange={(e) => setNewOption(e.target.value)}
                  onKeyDown={(e) => {
                    if (e.key === "Enter") {
                      e.preventDefault();
                      addOption();
                    }
                  }}
                  placeholder="Add an option..."
                  className="flex-1 rounded-lg border border-parchment-200 dark:border-slate-700 bg-white dark:bg-slate-900 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-cerulean-500 dark:text-white"
                />
                <button
                  type="button"
                  onClick={addOption}
                  className="px-3 py-2 text-sm font-medium rounded-lg bg-parchment-100 dark:bg-slate-800 text-parchment-700 dark:text-slate-300 hover:bg-parchment-200 dark:hover:bg-slate-700 transition-colors"
                >
                  Add
                </button>
              </div>
            </div>
          )}

          {/* Reference context (teacher guidance) */}
          <div className="flex flex-col gap-1.5">
            <label htmlFor="q-refctx" className="text-sm font-medium text-parchment-700 dark:text-slate-300">
              Reference Context
              <span className="ml-1 text-xs text-gold-600 dark:text-gold-400 font-normal">(Teacher guidance only)</span>
            </label>
            <textarea
              id="q-refctx"
              value={referenceContext}
              onChange={(e) => setReferenceContext(e.target.value)}
              placeholder='{ "expectedAnswer": "...", "guidance": "..." }'
              rows={6}
              className="w-full rounded-lg border border-parchment-200 dark:border-slate-700 bg-white dark:bg-slate-900 px-3 py-2 text-sm font-mono focus:outline-none focus:ring-2 focus:ring-cerulean-500 dark:text-white resize-none"
            />
            <p className="text-xs text-parchment-400 dark:text-slate-500">
              Valid JSON. Never shown to students. Provides guidance for teachers reviewing answers.
            </p>
          </div>

          {/* Actions */}
          <div className="flex gap-2 justify-between pt-4 border-t border-parchment-100 dark:border-slate-800">
            {!isNew && (
              <button
                onClick={() => setConfirmDelete(true)}
                className="px-3 py-2 text-sm font-medium rounded-lg text-red-600 dark:text-red-400 hover:bg-red-50 dark:hover:bg-red-900/20 transition-colors"
              >
                Delete
              </button>
            )}
            <div className="flex gap-2 ml-auto">
              <button
                onClick={() => onOpenChange(false)}
                className="px-4 py-2 text-sm font-medium rounded-lg text-parchment-600 dark:text-slate-300 hover:bg-parchment-100 dark:hover:bg-slate-800 transition-colors"
              >
                Cancel
              </button>
              <button
                onClick={handleSave}
                disabled={saving || !promptText.trim()}
                className="px-4 py-2 text-sm font-medium rounded-lg bg-cerulean-600 text-white hover:bg-cerulean-700 disabled:opacity-50 transition-colors"
              >
                {saving ? "Saving..." : "Save"}
              </button>
            </div>
          </div>
        </div>
      </SlideOutPanel>

      <ConfirmDialog
        open={confirmDelete}
        onOpenChange={setConfirmDelete}
        title="Delete Question"
        description="Are you sure you want to delete this question?"
        confirmLabel="Delete"
        variant="destructive"
        onConfirm={handleDelete}
      />
    </>
  );
}
