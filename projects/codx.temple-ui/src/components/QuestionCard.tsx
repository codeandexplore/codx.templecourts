import { CheckBadgeIcon, QuestionMarkCircleIcon } from "@heroicons/react/24/outline";
import { Card, CardContent } from "./ui/card";
import { Badge } from "./ui/badge";

interface QuestionCardProps {
  questionType: string;
  promptText: string;
  isAnswered: boolean;
  children?: React.ReactNode;
}

const typeLabel: Record<string, string> = {
  Essay: "Essay",
  YesNo: "Yes / No",
  TrueFalse: "True / False",
  FillBlank: "Fill in the Blank",
  SelectEmbedded: "Multiple Choice",
};

export function QuestionCard({ questionType, promptText, isAnswered, children }: QuestionCardProps) {
  return (
    <Card className={`p-4 ${isAnswered ? "border-emerald-300 dark:border-emerald-700 bg-emerald-50/30 dark:bg-emerald-950/15" : ""}`}>
      <CardContent className="flex flex-col gap-2">
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-2">
            <QuestionMarkCircleIcon className="size-4 text-parchment-400" />
            <span className="text-xs font-medium text-parchment-500 dark:text-slate-400 uppercase tracking-wider">
              {typeLabel[questionType] || questionType}
            </span>
          </div>
          {isAnswered && (
            <Badge variant="success" className="flex items-center gap-1">
              <CheckBadgeIcon className="size-3" />
              Answered
            </Badge>
          )}
        </div>
        <p className="text-sm text-parchment-800 dark:text-slate-200 leading-relaxed">{promptText}</p>
        {children}
      </CardContent>
    </Card>
  );
}
