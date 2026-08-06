import { useState, useMemo } from "react";
import { useListLessonsQuery } from "../services/lessonsApi";
import { CheckIcon, ChevronDownIcon } from "@heroicons/react/24/outline";
import { Button } from "./ui/button";

interface LessonSelectorProps {
  value: string | null;
  onChange: (lessonKey: string) => void;
}

export default function LessonSelector({ value, onChange }: LessonSelectorProps) {
  const { data: lessons, isLoading } = useListLessonsQuery();
  const [open, setOpen] = useState(false);
  const [search, setSearch] = useState("");

  const filtered = useMemo(() => {
    if (!lessons) return [];
    if (!search.trim()) return lessons;
    const q = search.toLowerCase();
    return lessons.filter(
      (l) => l.title.toLowerCase().includes(q) || String(l.number).includes(q)
    );
  }, [lessons, search]);

  const selected = lessons?.find((l) => l.key === value);

  if (isLoading) return <p className="text-parchment-500 dark:text-slate-400 text-sm">Loading lessons...</p>;

  return (
    <div className="relative">
      <Button
        variant="outline"
        className="w-full justify-between font-normal"
        onClick={() => setOpen(!open)}
      >
        <span className={selected ? "" : "text-slate-400"}>{selected ? `#${selected.number} ${selected.title}` : "Select a lesson..."}</span>
        <ChevronDownIcon className="size-4 ml-2 shrink-0 opacity-50" />
      </Button>
      {open && (
        <div className="absolute z-50 mt-1 w-full rounded-xl border border-parchment-200 dark:border-slate-700 bg-white dark:bg-slate-900 shadow-lg max-h-64 overflow-y-auto">
          <div className="p-2">
            <input
              type="text"
              placeholder="Search lessons..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              className="w-full rounded-lg border border-parchment-200 dark:border-slate-700 bg-parchment-50 dark:bg-slate-800 px-3 py-1.5 text-sm focus:outline-none focus:ring-2 focus:ring-cerulean-500 dark:text-white"
            />
          </div>
          <div className="border-t border-parchment-100 dark:border-slate-800">
            {filtered.length === 0 ? (
              <p className="px-4 py-3 text-sm text-slate-400">No lessons found</p>
            ) : (
              filtered.map((l) => (
                <button
                  key={l.key}
                  onClick={() => {
                    onChange(l.key);
                    setOpen(false);
                    setSearch("");
                  }}
                  className={`w-full flex items-center justify-between px-4 py-2 text-sm text-left hover:bg-parchment-50 dark:hover:bg-slate-800 ${
                    value === l.key ? "bg-cerulean-50 dark:bg-cerulean-900/20" : ""
                  }`}
                >
                  <span className="text-parchment-900 dark:text-white">
                    <span className="text-xs text-parchment-400 mr-2">#{l.number}</span>
                    {l.title}
                  </span>
                  {value === l.key && <CheckIcon className="size-4 text-cerulean-600 dark:text-cerulean-400 shrink-0" />}
                </button>
              ))
            )}
          </div>
        </div>
      )}
    </div>
  );
}
