import { Link } from "react-router-dom";
import { BookOpenIcon } from "@heroicons/react/24/outline";
import { Card, CardContent } from "./ui/card";
import { Badge } from "./ui/badge";

interface LessonCardProps {
  lessonKey: string;
  number: number;
  title: string;
  isPublished: boolean;
}

export function LessonCard({ lessonKey, number, title, isPublished }: LessonCardProps) {
  return (
    <Link to={`/lessons/${lessonKey}`}>
      <Card className="h-full p-6 hover:border-cerulean-200 dark:hover:border-cerulean-700 hover:shadow-md transition-all cursor-pointer">
        <CardContent className="flex flex-col gap-3">
          <div className="flex items-center gap-2">
            <BookOpenIcon className="size-5 text-cerulean-500" />
            <span className="text-xs font-medium text-parchment-500 dark:text-slate-400 uppercase tracking-wider">Lesson {number}</span>
          </div>
          <h3 className="font-serif text-lg font-medium text-parchment-900 dark:text-white leading-snug">{title}</h3>
          <div>
            <Badge variant={isPublished ? "success" : "warning"}>
              {isPublished ? "Published" : "Draft"}
            </Badge>
          </div>
        </CardContent>
      </Card>
    </Link>
  );
}
