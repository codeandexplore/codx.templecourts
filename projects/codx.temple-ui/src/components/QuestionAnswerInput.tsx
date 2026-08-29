import { Input } from "./ui/input";
import { Textarea } from "./ui/textarea";
import { Button } from "./ui/button";
import type { QuestionDto } from "../services/lessonsApi";

interface QuestionAnswerInputProps {
  question: QuestionDto;
  value: string;
  onChange: (value: string) => void;
}

export default function QuestionAnswerInput({ question, value, onChange }: QuestionAnswerInputProps) {
  switch (question.questionType) {
    case "TrueFalse":
      return <ChoiceButtons options={["True", "False"]} value={value} onChange={onChange} />;
    case "YesNo":
      return <ChoiceButtons options={["Yes", "No"]} value={value} onChange={onChange} />;
    case "FillBlank":
      return (
        <Input
          value={value}
          onChange={(e) => onChange(e.target.value)}
          placeholder="Type your answer..."
          autoFocus
        />
      );
    case "SelectEmbedded": {
      const metadata = question.metadata as { options?: string[] } | null;
      const options = metadata?.options ?? [];
      if (options.length === 0) {
        return (
          <Input
            value={value}
            onChange={(e) => onChange(e.target.value)}
            placeholder="Type your answer..."
          />
        );
      }
      return <ChoiceButtons options={options} value={value} onChange={onChange} />;
    }
    default:
      return (
        <Textarea
          value={value}
          onChange={(e) => onChange(e.target.value)}
          className="min-h-[100px]"
          placeholder="Type your answer..."
        />
      );
  }
}

function ChoiceButtons({
  options,
  value,
  onChange,
}: {
  options: string[];
  value: string;
  onChange: (value: string) => void;
}) {
  return (
    <div className="flex flex-wrap gap-2">
      {options.map((opt) => (
        <Button
          key={opt}
          type="button"
          variant={value === opt ? "default" : "outline"}
          className={value === opt ? "bg-cerulean-600 hover:bg-cerulean-700 text-white" : ""}
          onClick={() => onChange(opt)}
        >
          {opt}
        </Button>
      ))}
    </div>
  );
}