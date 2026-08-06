import { useState } from "react";
import type { LessonNodeDto } from "../services/lessonsApi";
import TreeNode from "./TreeNode";
import QuestionEditor from "./QuestionEditor";
import SlideOutPanel from "./SlideOutPanel";
import { useCreateNodeMutation, useUpdateNodeMutation } from "../services/nodesApi";

interface TreeEditorProps {
  versionId: string;
  versionNodes: LessonNodeDto[];
}

export default function TreeEditor({ versionId, versionNodes }: TreeEditorProps) {
  const [selectedNode, setSelectedNode] = useState<LessonNodeDto | null>(null);
  const [editorMode, setEditorMode] = useState<"node" | "question" | null>(null);
  const [addChildFor, setAddChildFor] = useState<LessonNodeDto | null>(null);
  const [questionForNode, setQuestionForNode] = useState<LessonNodeDto | null>(null);
  const [editingQuestion, setEditingQuestion] = useState<{
    nodeKey: string;
    questionKey: string;
    questionType: string;
    promptText: string;
    metadata?: Record<string, unknown>;
    referenceContext?: Record<string, unknown>;
  } | null>(null);

  return (
    <div>
      <h3 className="font-serif text-lg font-medium text-parchment-900 dark:text-white mb-4">Lesson Tree</h3>

      <div className="space-y-1">
        {versionNodes.length === 0 ? (
          <div className="text-center py-6 border-2 border-dashed border-parchment-200 dark:border-slate-700 rounded-xl">
            <p className="text-parchment-400 dark:text-slate-500 text-sm mb-3">
              No nodes yet. Add your first top-level node.
            </p>
            <AddRootNodeButton
              versionId={versionId}
              onCreated={() => {}}
            />
          </div>
        ) : (
          versionNodes.map((node, index) => {
            const siblings = versionNodes;
            return (
              <TreeNode
                key={node.key}
                node={node}
                versionId={versionId}
                depth={1}
                siblings={siblings}
                siblingIndex={index}
                onEditNode={(n) => {
                  setSelectedNode(n);
                  setEditorMode("node");
                }}
                onAddChild={(n) => {
                  setAddChildFor(n);
                  setEditorMode("node");
                  setSelectedNode(null);
                }}
                onAddQuestion={(n) => {
                  setQuestionForNode(n);
                  setEditingQuestion(null);
                }}
                onEditQuestion={(nodeKey, q) => {
                  setQuestionForNode(null);
                  setEditingQuestion({
                    nodeKey,
                    questionKey: q.key,
                    questionType: q.questionType,
                    promptText: q.promptText,
                    metadata: q.metadata as Record<string, unknown> | undefined,
                    referenceContext: q.referenceContext as Record<string, unknown> | undefined,
                  });
                }}
              />
            );
          })
        )}
      </div>

      {editorMode === "node" && (addChildFor || selectedNode) && (
        <NodeEditorPanel
          open={true}
          onOpenChange={() => {
            setEditorMode(null);
            setSelectedNode(null);
            setAddChildFor(null);
          }}
          versionId={versionId}
          parentNodeKey={addChildFor?.key}
          existingNode={addChildFor ? null : selectedNode}
        />
      )}

      {(questionForNode || editingQuestion) && (
        <QuestionEditor
          open={true}
          onOpenChange={() => {
            setQuestionForNode(null);
            setEditingQuestion(null);
          }}
          nodeKey={questionForNode?.key || editingQuestion?.nodeKey || ""}
          existingQuestion={editingQuestion}
        />
      )}
    </div>
  );
}

function AddRootNodeButton({ versionId, onCreated }: { versionId: string; onCreated: () => void }) {
  const [createNode] = useCreateNodeMutation();
  const [title, setTitle] = useState("");
  const [show, setShow] = useState(false);

  const handleCreate = async () => {
    if (!title.trim()) return;
    try {
      await createNode({ versionId, title: title.trim(), description: "" }).unwrap();
      setTitle("");
      setShow(false);
      onCreated();
    } catch { /* handled by RTK */ }
  };

  return show ? (
    <div className="flex gap-2">
      <input
        type="text"
        value={title}
        onChange={(e) => setTitle(e.target.value)}
        placeholder="Node title..."
        className="flex-1 rounded-lg border border-parchment-200 dark:border-slate-700 bg-white dark:bg-slate-900 px-3 py-1.5 text-sm focus:outline-none focus:ring-2 focus:ring-cerulean-500 dark:text-white"
        onKeyDown={(e) => {
          if (e.key === "Enter") handleCreate();
          if (e.key === "Escape") setShow(false);
        }}
        autoFocus
      />
      <button
        onClick={handleCreate}
        className="px-3 py-1.5 text-sm rounded-lg bg-cerulean-600 text-white hover:bg-cerulean-700"
      >
        Add
      </button>
    </div>
  ) : (
    <button
      onClick={() => setShow(true)}
      className="text-sm text-cerulean-600 dark:text-cerulean-400 hover:text-cerulean-700 dark:hover:text-cerulean-300"
    >
      + Add Root Node
    </button>
  );
}

function NodeEditorPanel({
  open,
  onOpenChange,
  versionId,
  parentNodeKey,
  existingNode,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  versionId: string;
  parentNodeKey?: string;
  existingNode: LessonNodeDto | null;
}) {
  const [createNode] = useCreateNodeMutation();
  const [updateNode] = useUpdateNodeMutation();
  const [title, setTitle] = useState(existingNode?.title ?? "");
  const [description, setDescription] = useState(existingNode?.description ?? "");
  const [gating, setGating] = useState(existingNode?.requiresPriorSiblingAnswered ?? false);
  const [saving, setSaving] = useState(false);

  const isNew = !existingNode;

  const handleSave = async () => {
    if (!title.trim()) return;
    setSaving(true);
    try {
      if (isNew) {
        await createNode({
          versionId,
          parentNodeKey,
          title: title.trim(),
          description: description.trim(),
          requiresPriorSiblingAnswered: gating,
        }).unwrap();
      } else {
        await updateNode({
          versionId,
          nodeKey: existingNode.key,
          title: title.trim(),
          description: description.trim(),
          requiresPriorSiblingAnswered: gating,
        }).unwrap();
      }
      onOpenChange(false);
    } catch { /* handled by RTK */ }
    setSaving(false);
  };

  return (
    <SlideOutPanel
      open={open}
      onOpenChange={onOpenChange}
      title={isNew ? "Add Node" : "Edit Node"}
      description={parentNodeKey ? "Adding a child node" : undefined}
    >
      <div className="flex flex-col gap-5">
        <div className="flex flex-col gap-1.5">
          <label htmlFor="node-title" className="text-sm font-medium text-parchment-700 dark:text-slate-300">Title</label>
          <input
            id="node-title"
            type="text"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            placeholder="Node title"
            className="w-full rounded-lg border border-parchment-200 dark:border-slate-700 bg-white dark:bg-slate-900 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-cerulean-500 dark:text-white"
            autoFocus
          />
        </div>
        <div className="flex flex-col gap-1.5">
          <label htmlFor="node-desc" className="text-sm font-medium text-parchment-700 dark:text-slate-300">Description</label>
          <textarea
            id="node-desc"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            placeholder="Node description"
            rows={3}
            className="w-full rounded-lg border border-parchment-200 dark:border-slate-700 bg-white dark:bg-slate-900 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-cerulean-500 dark:text-white resize-none"
          />
        </div>
        <label className="flex items-center gap-2 text-sm text-parchment-700 dark:text-slate-300">
          <input
            type="checkbox"
            checked={gating}
            onChange={(e) => setGating(e.target.checked)}
            className="rounded border-parchment-300 dark:border-slate-600 text-cerulean-600 focus:ring-cerulean-500"
          />
          Require prior sibling answered (gating)
        </label>
        <div className="flex gap-2 justify-end pt-4 border-t border-parchment-100 dark:border-slate-800">
          <button
            onClick={() => onOpenChange(false)}
            className="px-4 py-2 text-sm font-medium rounded-lg text-parchment-600 dark:text-slate-300 hover:bg-parchment-100 dark:hover:bg-slate-800 transition-colors"
          >
            Cancel
          </button>
          <button
            onClick={handleSave}
            disabled={saving || !title.trim()}
            className="px-4 py-2 text-sm font-medium rounded-lg bg-cerulean-600 text-white hover:bg-cerulean-700 disabled:opacity-50 transition-colors"
          >
            {saving ? "Saving..." : "Save"}
          </button>
        </div>
      </div>
    </SlideOutPanel>
  );
}
