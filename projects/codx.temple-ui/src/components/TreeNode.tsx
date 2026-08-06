import { useState } from "react";
import {
  PlusIcon,
  TrashIcon,
  ChevronUpIcon,
  ChevronDownIcon,
  LockClosedIcon,
  QuestionMarkCircleIcon,
} from "@heroicons/react/24/outline";
import type { LessonNodeDto, QuestionDto } from "../services/lessonsApi";
import { useDeleteNodeMutation, useReorderNodesMutation } from "../services/nodesApi";
import { Badge } from "./ui/badge";
import ConfirmDialog from "./ConfirmDialog";

interface TreeNodeProps {
  node: LessonNodeDto;
  versionId: string;
  depth: number;
  siblings: LessonNodeDto[];
  siblingIndex: number;
  onEditNode: (node: LessonNodeDto) => void;
  onAddChild: (node: LessonNodeDto) => void;
  onAddQuestion: (node: LessonNodeDto) => void;
  onEditQuestion: (nodeKey: string, question: QuestionDto) => void;
}

const depthClasses: Record<number, string> = {
  1: "ml-0 border-l-0",
  2: "ml-6 border-l-2 border-parchment-200 dark:border-slate-700",
  3: "ml-12 border-l-2 border-parchment-200 dark:border-slate-700",
};

const depthBg: Record<number, string> = {
  1: "bg-white dark:bg-slate-900",
  2: "bg-parchment-50/50 dark:bg-slate-900/50",
  3: "bg-parchment-100/30 dark:bg-slate-800/30",
};

export default function TreeNode({
  node,
  versionId,
  depth,
  siblings,
  siblingIndex,
  onEditNode,
  onAddChild,
  onAddQuestion,
  onEditQuestion,
}: TreeNodeProps) {
  const [deleteNode] = useDeleteNodeMutation();
  const [reorderNodes] = useReorderNodesMutation();
  const [confirmDelete, setConfirmDelete] = useState(false);
  const maxDepth = 3;
  const hasChildren = node.children.length > 0;
  const hasQuestions = node.questions.length > 0;
  const isLeaf = depth >= maxDepth || hasQuestions;

  const handleDelete = async () => {
    try {
      await deleteNode({ versionId, nodeKey: node.key }).unwrap();
      setConfirmDelete(false);
    } catch { /* handled by RTK */ }
  };

  const handleReorder = async (direction: "up" | "down") => {
    const keys = siblings.map((s) => s.key);
    const from = siblingIndex;
    const to = direction === "up" ? from - 1 : from + 1;
    if (to < 0 || to >= keys.length) return;
    [keys[from], keys[to]] = [keys[to], keys[from]];
    try {
      await reorderNodes({
        versionId,
        parentNodeKey: node.parentNodeId ?? undefined,
        orderedKeys: keys,
      }).unwrap();
    } catch { /* handled by RTK */ }
  };

  return (
    <div>
      <div className={`flex items-start gap-3 rounded-xl border border-parchment-200 dark:border-slate-700 p-4 ${depthBg[depth]} ${depthClasses[depth]}`}>
        {/* Gating indicator */}
        <div className="shrink-0 mt-0.5">
          {node.requiresPriorSiblingAnswered ? (
            <LockClosedIcon className="size-4 text-gold-500 dark:text-gold-400" title="Sibling gating enabled" />
          ) : (
            <div className="size-4" />
          )}
        </div>

        {/* Content */}
        <div className="flex-1 min-w-0">
          <div className="flex items-center gap-2" onClick={() => onEditNode(node)} style={{ cursor: "pointer" }}>
            <span className="text-sm font-medium text-parchment-900 dark:text-white truncate">{node.title}</span>
            <Badge variant="secondary" className="shrink-0 text-[10px]">Depth {depth}</Badge>
            {node.requiresPriorSiblingAnswered && (
              <span className="text-[10px] text-gold-600 dark:text-gold-400 font-medium">Gated</span>
            )}
          </div>
          {node.description && (
            <p className="text-xs text-parchment-500 dark:text-slate-400 mt-1 line-clamp-2">{node.description}</p>
          )}

          {/* Questions display */}
          {hasQuestions && (
            <div className="mt-2 space-y-1">
              {node.questions.map((q) => (
                <div
                  key={q.key}
                  onClick={() => onEditQuestion(node.key, q)}
                  className="flex items-center gap-2 text-xs p-2 rounded-lg bg-cerulean-50 dark:bg-cerulean-900/20 border border-cerulean-100 dark:border-cerulean-800 cursor-pointer hover:bg-cerulean-100 dark:hover:bg-cerulean-900/40 transition-colors"
                >
                  <QuestionMarkCircleIcon className="size-3 text-cerulean-500 dark:text-cerulean-400 shrink-0" />
                  <span className="text-parchment-700 dark:text-slate-300 truncate">{q.promptText || `Question #${q.order}`}</span>
                  <Badge variant="secondary" className="shrink-0 text-[9px] ml-auto">{q.questionType}</Badge>
                </div>
              ))}
            </div>
          )}
        </div>

        {/* Actions */}
        <div className="flex items-center gap-0.5 shrink-0">
          <ActionButton onClick={() => onAddChild(node)} disabled={isLeaf} title="Add Child">
            <PlusIcon className="size-3.5" />
          </ActionButton>
          <ActionButton onClick={() => onAddQuestion(node)} disabled={hasChildren} title="Add Question">
            <QuestionMarkCircleIcon className="size-3.5" />
          </ActionButton>
          <ActionButton onClick={() => setConfirmDelete(true)} title="Delete">
            <TrashIcon className="size-3.5" />
          </ActionButton>
          <ActionButton onClick={() => handleReorder("up")} disabled={siblingIndex === 0} title="Move Up">
            <ChevronUpIcon className="size-3.5" />
          </ActionButton>
          <ActionButton onClick={() => handleReorder("down")} disabled={siblingIndex === siblings.length - 1} title="Move Down">
            <ChevronDownIcon className="size-3.5" />
          </ActionButton>
        </div>
      </div>

      {/* Recursive children */}
      {hasChildren && (
        <div className="space-y-1 mt-1">
          {node.children.map((child, idx) => (
            <TreeNode
              key={child.key}
              node={child}
              versionId={versionId}
              depth={depth + 1}
              siblings={node.children}
              siblingIndex={idx}
              onEditNode={onEditNode}
              onAddChild={onAddChild}
              onAddQuestion={onAddQuestion}
              onEditQuestion={onEditQuestion}
            />
          ))}
        </div>
      )}

      {/* Delete confirmation */}
      <ConfirmDialog
        open={confirmDelete}
        onOpenChange={setConfirmDelete}
        title="Delete Node"
        description={
          hasChildren
            ? "This node has child nodes and/or questions. Deleting it will remove the entire subtree."
            : "Are you sure you want to delete this node?"
        }
        confirmLabel="Delete"
        variant="destructive"
        onConfirm={handleDelete}
      />
    </div>
  );
}

function ActionButton({
  children,
  onClick,
  disabled,
  title,
}: {
  children: React.ReactNode;
  onClick: () => void;
  disabled?: boolean;
  title: string;
}) {
  if (disabled) return null;
  return (
    <button
      onClick={(e) => {
        e.stopPropagation();
        onClick();
      }}
      title={title}
      className="p-1.5 rounded-lg text-parchment-400 dark:text-slate-500 hover:text-parchment-700 dark:hover:text-slate-300 hover:bg-parchment-100 dark:hover:bg-slate-800 transition-colors"
    >
      {children}
    </button>
  );
}
