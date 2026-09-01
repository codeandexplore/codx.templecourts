import { useState } from "react";
import { Link } from "react-router-dom";
import {
  useListCheckQuestionsQuery,
  useCreateCheckQuestionMutation,
  useUpdateCheckQuestionMutation,
  useDeleteCheckQuestionMutation,
  useListPublishedQuestionsQuery,
} from "../services/checkQuestionApi";
import { ArrowLeftIcon, PlusIcon, PencilIcon, TrashIcon, ExclamationTriangleIcon } from "@heroicons/react/24/outline";
import { Button } from "../components/ui/button";
import { Card, CardContent } from "../components/ui/card";
import { Badge } from "../components/ui/badge";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "../components/ui/dialog";
import { Textarea } from "../components/ui/textarea";

export default function CheckQuestionsPage() {
  const { data: items, isLoading } = useListCheckQuestionsQuery();
  const { data: publishedQuestions } = useListPublishedQuestionsQuery();
  const [createCheckQuestion] = useCreateCheckQuestionMutation();
  const [updateCheckQuestion] = useUpdateCheckQuestionMutation();
  const [deleteCheckQuestion] = useDeleteCheckQuestionMutation();
  const [editing, setEditing] = useState<{ id: string; noteText: string } | null>(null);
  const [creating, setCreating] = useState(false);

  if (isLoading) return <div className="text-parchment-500 dark:text-slate-400">Loading check questions...</div>;

  return (
    <div>
      <Link to="/teacher" className="inline-flex items-center gap-1 text-sm text-cerulean-600 hover:underline mb-6">
        <ArrowLeftIcon className="size-4" />
        Back to dashboard
      </Link>
      <div className="flex items-center justify-between mb-6">
        <h2 className="font-serif text-2xl font-semibold text-parchment-900 dark:text-white">Check-Question Bank</h2>
        <Button className="bg-cerulean-600 hover:bg-cerulean-700 text-white" onClick={() => setCreating(true)}>
          <PlusIcon className="size-4 mr-1.5" />
          New Check-Question
        </Button>
      </div>

      <div className="space-y-2">
        {!items || items.length === 0 ? (
          <p className="text-parchment-400 dark:text-slate-500 text-sm">No check-questions yet.</p>
        ) : (
          items.map((item) => (
            <Card key={item.id} className="p-4">
              <CardContent>
                <div className="flex items-center justify-between gap-3">
                  <div className="min-w-0 flex-1">
                    <p className="text-sm text-parchment-800 dark:text-slate-200 leading-relaxed">{item.noteText}</p>
                    <div className="flex items-center gap-2 mt-1">
                      {item.isOrphaned ? (
                        <Badge variant="warning" className="flex items-center gap-1 text-[10px]">
                          <ExclamationTriangleIcon className="size-3" />
                          Orphaned
                        </Badge>
                      ) : (
                        <Badge variant="success" className="text-[10px]">Active</Badge>
                      )}
                      <span className="text-[10px] text-parchment-400 dark:text-slate-500">
                        {new Date(item.createdAt).toLocaleDateString()}
                      </span>
                    </div>
                  </div>
                  <div className="flex items-center gap-1 shrink-0">
                    <Button variant="ghost" size="sm" onClick={() => setEditing({ id: item.id, noteText: item.noteText })}>
                      <PencilIcon className="size-3.5" />
                    </Button>
                    <Button variant="ghost" size="sm" className="text-red-600 dark:text-red-400" onClick={() => deleteCheckQuestion(item.id)}>
                      <TrashIcon className="size-3.5" />
                    </Button>
                  </div>
                </div>
              </CardContent>
            </Card>
          ))
        )}
      </div>

      {(creating || editing) && (
        <CheckQuestionForm
          key={editing?.id ?? "new"}
          publishedQuestions={publishedQuestions ?? []}
          existing={editing}
          onCreate={async (questionKey, noteText) => {
            await createCheckQuestion({ questionKey, noteText }).unwrap();
            setCreating(false);
          }}
          onUpdate={async (noteText) => {
            if (editing) {
              await updateCheckQuestion({ id: editing.id, noteText }).unwrap();
              setEditing(null);
            }
          }}
          onClose={() => { setCreating(false); setEditing(null); }}
        />
      )}
    </div>
  );
}

function CheckQuestionForm({
  publishedQuestions,
  existing,
  onCreate,
  onUpdate,
  onClose,
}: {
  publishedQuestions: { questionKey: string; promptText: string; lessonNumber: number; lessonTitle: string }[];
  existing: { id: string; noteText: string } | null;
  onCreate: (questionKey: string, noteText: string) => Promise<void>;
  onUpdate: (noteText: string) => Promise<void>;
  onClose: () => void;
}) {
  const [questionKey, setQuestionKey] = useState("");
  const [noteText, setNoteText] = useState(existing?.noteText ?? "");
  const [saving, setSaving] = useState(false);

  const isEdit = existing !== null;

  const handleSave = async () => {
    if (!noteText.trim()) return;
    if (!isEdit && !questionKey) return;
    setSaving(true);
    try {
      if (isEdit) await onUpdate(noteText.trim());
      else await onCreate(questionKey, noteText.trim());
    } catch { /* handled by RTK */ }
    setSaving(false);
  };

  return (
    <Dialog open onOpenChange={(o) => !o && onClose()}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle className="font-serif">{isEdit ? "Edit Check-Question" : "New Check-Question"}</DialogTitle>
        </DialogHeader>
        <div className="flex flex-col gap-3">
          {!isEdit && (
            <div className="flex flex-col gap-1.5">
              <label className="text-sm font-medium text-parchment-700 dark:text-slate-300">Question</label>
              <select
                value={questionKey}
                onChange={(e) => setQuestionKey(e.target.value)}
                className="w-full rounded-lg border border-parchment-200 dark:border-slate-700 bg-white dark:bg-slate-900 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-cerulean-500 dark:text-white"
              >
                <option value="">Select a question...</option>
                {publishedQuestions.map((q) => (
                  <option key={q.questionKey} value={q.questionKey}>
                    Lesson {q.lessonNumber} — {q.promptText.slice(0, 60)}
                  </option>
                ))}
              </select>
            </div>
          )}
          <div className="flex flex-col gap-1.5">
            <label className="text-sm font-medium text-parchment-700 dark:text-slate-300">Note Text</label>
            <Textarea
              value={noteText}
              onChange={(e) => setNoteText(e.target.value)}
              placeholder="A check-understanding prompt for the student..."
              className="min-h-[80px]"
            />
          </div>
          <div className="flex gap-2 justify-end pt-2">
            <Button variant="ghost" onClick={onClose}>Cancel</Button>
            <Button onClick={handleSave} disabled={saving || !noteText.trim() || (!isEdit && !questionKey)} className="bg-cerulean-600 hover:bg-cerulean-700 text-white">
              {saving ? "Saving..." : "Save"}
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
}