import { useState } from "react";
import { Link } from "react-router-dom";
import { ArrowLeftIcon } from "@heroicons/react/24/outline";
import LessonSelector from "../components/LessonSelector";
import TreeEditor from "../components/TreeEditor";
import { useGetVersionsQuery, useCreateDraftMutation, usePublishVersionMutation } from "../services/versionsApi";
import { Button } from "../components/ui/button";
import { Card, CardContent } from "../components/ui/card";
import { Badge } from "../components/ui/badge";

export default function EditorPage() {
  const [lessonKey, setLessonKey] = useState<string | null>(null);

  return (
    <div>
      <Link
        to="/admin"
        className="inline-flex items-center gap-1 text-sm text-parchment-500 dark:text-slate-400 hover:text-parchment-700 dark:hover:text-slate-200 mb-4 transition-colors"
      >
        <ArrowLeftIcon className="size-3.5" />
        Back to Admin
      </Link>
      <h2 className="font-serif text-2xl font-semibold text-parchment-900 dark:text-white mb-6">Lesson Editor</h2>

      <div className="mb-6">
        <label className="block text-sm font-medium text-parchment-700 dark:text-slate-300 mb-2">
          Select a lesson to edit
        </label>
        <LessonSelector value={lessonKey} onChange={setLessonKey} />
      </div>

      {lessonKey && <VersionManager lessonKey={lessonKey} />}
    </div>
  );
}

function VersionManager({ lessonKey }: { lessonKey: string }) {
  const { data: versions, isLoading } = useGetVersionsQuery(lessonKey);
  const [createDraft, { isLoading: creating }] = useCreateDraftMutation();
  const [publishVersion, { isLoading: publishing }] = usePublishVersionMutation();
  const [selectedVersionId, setSelectedVersionId] = useState<string | null>(null);
  const [error, setError] = useState("");

  if (isLoading) return <p className="text-parchment-500 dark:text-slate-400 text-sm">Loading versions...</p>;
  if (!versions || versions.length === 0) {
    return (
      <div className="text-center py-8">
        <p className="text-parchment-500 dark:text-slate-400 mb-3">No versions yet.</p>
        <Button
          onClick={async () => {
            try {
              const result = await createDraft({ lessonKey }).unwrap();
              setSelectedVersionId(result.id);
            } catch {
              setError("Failed to create draft");
            }
          }}
          disabled={creating}
          className="bg-cerulean-600 hover:bg-cerulean-700 text-white"
        >
          {creating ? "Creating..." : "Create First Draft"}
        </Button>
      </div>
    );
  }

  const handleCreateDraft = async () => {
    setError("");
    try {
      const result = await createDraft({ lessonKey }).unwrap();
      setSelectedVersionId(result.id);
    } catch {
      setError("Failed to create draft");
    }
  };

  const handlePublish = async (versionId: string) => {
    setError("");
    try {
      await publishVersion({ lessonKey, versionId }).unwrap();
    } catch {
      setError("Failed to publish version");
    }
  };

  const selectedVersion = versions.find((v) => v.id === selectedVersionId);
  const draftVersion = versions.find((v) => v.status === "Draft");

  return (
    <div>
      {error && <p className="text-sm text-red-600 mb-3">{error}</p>}

      <div className="flex items-center justify-between mb-4">
        <h3 className="font-serif text-lg font-medium text-parchment-900 dark:text-white">Versions</h3>
        <Button
          onClick={handleCreateDraft}
          disabled={creating}
          size="sm"
          className="bg-cerulean-600 hover:bg-cerulean-700 text-white"
        >
          {creating ? "Creating..." : "Create Draft"}
        </Button>
      </div>

      <div className="space-y-2 mb-6">
        {versions.map((v) => (
          <Card
            key={v.id}
            className={`p-4 cursor-pointer transition-colors hover:border-cerulean-300 dark:hover:border-cerulean-700 ${
              selectedVersionId === v.id
                ? "border-cerulean-400 dark:border-cerulean-600 bg-cerulean-50/50 dark:bg-cerulean-900/10"
                : ""
            }`}
            onClick={() => setSelectedVersionId(v.id)}
          >
            <CardContent className="flex items-center justify-between">
              <div className="flex items-center gap-3 min-w-0">
                <span className="text-sm font-medium text-parchment-900 dark:text-white">
                  v{v.versionNumber}
                </span>
                <Badge
                  variant={v.status === "Published" ? "success" : v.status === "Draft" ? "warning" : "secondary"}
                >
                  {v.status}
                </Badge>
                <span className="text-xs text-parchment-400 dark:text-slate-500">
                  {new Date(v.createdAt).toLocaleDateString()}
                </span>
              </div>
              {v.status === "Draft" && (
                <Button
                  size="sm"
                  onClick={(e) => {
                    e.stopPropagation();
                    handlePublish(v.id);
                  }}
                  disabled={publishing}
                  className="bg-gold-500 hover:bg-gold-600 text-white"
                >
                  {publishing ? "Publishing..." : "Publish"}
                </Button>
              )}
            </CardContent>
          </Card>
        ))}
      </div>

      {selectedVersion && selectedVersion.status === "Draft" && (
        <TreeEditor versionId={selectedVersion.id} versionNodes={selectedVersion.nodes} />
      )}

      {selectedVersion && selectedVersion.status !== "Draft" && (
        <div className="text-center py-6">
          <p className="text-parchment-500 dark:text-slate-400">
            Select a Draft version to edit, or create a new draft from a published version.
          </p>
          {draftVersion && (
            <Button
              variant="link"
              onClick={() => setSelectedVersionId(draftVersion.id)}
              className="mt-2 text-cerulean-600 dark:text-cerulean-400"
            >
              Switch to Draft v{draftVersion.versionNumber}
            </Button>
          )}
        </div>
      )}
    </div>
  );
}
