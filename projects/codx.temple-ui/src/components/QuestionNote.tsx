import { useState } from "react";
import { useCreateNoteMutation, useUpdateNoteMutation, useDeleteNoteMutation } from "../services/studentApi";
import { PencilSquareIcon, DocumentTextIcon, TrashIcon } from "@heroicons/react/24/outline";
import { Button } from "./ui/button";
import { Textarea } from "./ui/textarea";

interface QuestionNoteProps {
  questionKey: string;
  existingNote: string | null;
  disabled?: boolean;
}

export default function QuestionNote({ questionKey, existingNote, disabled }: QuestionNoteProps) {
  const [open, setOpen] = useState(false);
  const [text, setText] = useState("");
  const [saving, setSaving] = useState(false);
  const [createNote] = useCreateNoteMutation();
  const [updateNote] = useUpdateNoteMutation();
  const [deleteNote] = useDeleteNoteMutation();

  const hasNote = existingNote !== null;

  const openEditor = () => {
    setText(existingNote ?? "");
    setOpen(true);
  };

  const handleSave = async () => {
    if (!text.trim()) return;
    setSaving(true);
    try {
      if (hasNote) await updateNote({ questionKey, noteText: text.trim() }).unwrap();
      else await createNote({ questionKey, noteText: text.trim() }).unwrap();
      setOpen(false);
    } catch { /* handled by RTK */ }
    setSaving(false);
  };

  const handleDelete = async () => {
    setSaving(true);
    try {
      await deleteNote(questionKey).unwrap();
      setText("");
      setOpen(false);
    } catch { /* handled by RTK */ }
    setSaving(false);
  };

  if (disabled) {
    return hasNote ? (
      <div className="text-xs text-parchment-500 dark:text-slate-400 flex items-start gap-1.5">
        <DocumentTextIcon className="size-3.5 shrink-0 mt-0.5" />
        <span className="italic">{existingNote}</span>
      </div>
    ) : null;
  }

  if (!open) {
    return (
      <button
        type="button"
        onClick={openEditor}
        className="inline-flex items-center gap-1 text-xs text-parchment-500 dark:text-slate-400 hover:text-cerulean-600 dark:hover:text-cerulean-400 transition-colors"
      >
        {hasNote ? <DocumentTextIcon className="size-3.5" /> : <PencilSquareIcon className="size-3.5" />}
        {hasNote ? "Edit note" : "Add note"}
      </button>
    );
  }

  return (
    <div className="space-y-2 pt-1">
      <Textarea
        value={text}
        onChange={(e) => setText(e.target.value)}
        placeholder="Add a personal note..."
        className="min-h-[60px] text-sm"
        autoFocus
      />
      <div className="flex gap-2">
        <Button size="sm" onClick={handleSave} disabled={saving || !text.trim()} className="bg-cerulean-600 hover:bg-cerulean-700 text-white">
          {saving ? "Saving..." : "Save"}
        </Button>
        <Button size="sm" variant="ghost" onClick={() => setOpen(false)}>
          Cancel
        </Button>
        {hasNote && (
          <Button size="sm" variant="ghost" onClick={handleDelete} disabled={saving} className="text-red-600 dark:text-red-400">
            <TrashIcon className="size-3.5 mr-1" />
            Delete
          </Button>
        )}
      </div>
    </div>
  );
}