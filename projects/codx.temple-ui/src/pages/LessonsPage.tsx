import { Link } from "react-router-dom";
import { useListLessonsQuery } from "../services/lessonsApi";
import { useListMyAttemptsQuery } from "../services/studentApi";
import { BookOpenIcon, CheckCircleIcon, PlayCircleIcon } from "@heroicons/react/24/outline";
import { Badge } from "../components/ui/badge";
import { Card, CardContent } from "../components/ui/card";

export default function LessonsPage() {
  const { data: lessons, isLoading, error } = useListLessonsQuery();
  const { data: attempts } = useListMyAttemptsQuery();

  if (isLoading) return <div className="text-parchment-500 dark:text-slate-400">Loading lessons...</div>;
  if (error) return <div className="text-red-600">Failed to load lessons.</div>;

  const latestByLesson = new Map<string, { status: string; id: string }>();
  for (const a of attempts ?? []) {
    if (!latestByLesson.has(a.lessonKey)) {
      latestByLesson.set(a.lessonKey, { status: a.status, id: a.id });
    }
  }

  return (
    <div>
      <h2 className="font-serif text-2xl font-semibold text-parchment-900 dark:text-white mb-6">Lessons</h2>
      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {lessons?.map((lesson) => {
          const attemptState = latestByLesson.get(lesson.key);
          return (
            <Link key={lesson.key} to={`/lessons/${lesson.key}`}>
              <Card className="h-full p-6 hover:border-cerulean-200 dark:hover:border-cerulean-700 hover:shadow-md transition-all cursor-pointer">
                <CardContent className="flex flex-col gap-3">
                  <div className="flex items-center gap-2">
                    <BookOpenIcon className="size-5 text-cerulean-500" />
                    <span className="text-xs font-medium text-parchment-500 dark:text-slate-400 uppercase tracking-wider">Lesson {lesson.number}</span>
                  </div>
                  <h3 className="font-serif text-lg font-medium text-parchment-900 dark:text-white leading-snug">{lesson.title}</h3>
                  <div className="flex items-center gap-2 flex-wrap">
                    <Badge variant={lesson.currentPublishedVersionId ? "success" : "warning"}>
                      {lesson.currentPublishedVersionId ? "Published" : "Draft"}
                    </Badge>
                    {attemptState?.status === "Completed" && (
                      <Badge variant="success" className="flex items-center gap-1">
                        <CheckCircleIcon className="size-3" />
                        Completed
                      </Badge>
                    )}
                    {attemptState?.status === "InProgress" && (
                      <Badge variant="secondary" className="flex items-center gap-1">
                        <PlayCircleIcon className="size-3" />
                        In progress
                      </Badge>
                    )}
                  </div>
                </CardContent>
              </Card>
            </Link>
          );
        })}
      </div>
    </div>
  );
}
