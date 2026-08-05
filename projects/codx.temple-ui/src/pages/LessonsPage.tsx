import { Link } from "react-router-dom";
import { useListLessonsQuery } from "../services/lessonsApi";

export default function LessonsPage() {
  const { data: lessons, isLoading, error } = useListLessonsQuery();

  if (isLoading) return <div className="text-gray-600">Loading lessons...</div>;
  if (error) return <div className="text-red-600">Failed to load lessons.</div>;

  return (
    <div>
      <h2 className="text-2xl font-semibold text-gray-900 dark:text-white mb-6">Lessons</h2>
      <div className="grid gap-3">
        {lessons?.map((lesson) => (
          <Link
            key={lesson.key}
            to={`/lessons/${lesson.key}`}
            className="block rounded-lg border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-900 p-4 hover:border-blue-300 dark:hover:border-blue-600 transition-colors"
          >
            <div className="flex items-center justify-between">
              <div>
                <span className="text-sm text-gray-500 dark:text-gray-400">Lesson {lesson.number}</span>
                <h3 className="text-lg font-medium text-gray-900 dark:text-white">{lesson.title}</h3>
              </div>
              <span className={`text-xs px-2 py-1 rounded-full ${
                lesson.currentPublishedVersionId ? "bg-green-100 text-green-700 dark:bg-green-900 dark:text-green-300" : "bg-yellow-100 text-yellow-700 dark:bg-yellow-900 dark:text-yellow-300"
              }`}>
                {lesson.currentPublishedVersionId ? "Published" : "Draft"}
              </span>
            </div>
          </Link>
        ))}
      </div>
    </div>
  );
}
