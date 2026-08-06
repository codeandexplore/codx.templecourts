import { useGetTeacherStudentsQuery } from "../services/teacherApi";
import { AcademicCapIcon, EnvelopeIcon } from "@heroicons/react/24/outline";
import { Card, CardContent } from "../components/ui/card";

export default function TeacherPage() {
  const { data: students, isLoading } = useGetTeacherStudentsQuery();

  return (
    <div>
      <h2 className="font-serif text-2xl font-semibold text-parchment-900 dark:text-white mb-6">Teacher Dashboard</h2>
      <section>
        <h3 className="text-lg font-medium text-parchment-800 dark:text-slate-200 mb-4">My Students</h3>
        {isLoading ? (
          <p className="text-parchment-500 dark:text-slate-400">Loading...</p>
        ) : students?.length === 0 ? (
          <Card className="p-6 text-center max-w-md">
            <CardContent className="flex flex-col items-center gap-3">
              <AcademicCapIcon className="size-10 text-parchment-300 dark:text-slate-600" />
              <p className="text-sm text-parchment-500 dark:text-slate-400">
                No students assigned yet. Claim a student to get started.
              </p>
            </CardContent>
          </Card>
        ) : (
          <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-3">
            {students?.map((s) => (
              <Card key={s.id} className="p-5">
                <CardContent className="flex items-start gap-4">
                  <div className="size-10 rounded-full bg-cerulean-100 dark:bg-cerulean-900 flex items-center justify-center shrink-0">
                    <AcademicCapIcon className="size-5 text-cerulean-600 dark:text-cerulean-400" />
                  </div>
                  <div className="min-w-0">
                    <p className="font-medium text-parchment-900 dark:text-white truncate">{s.studentDisplayName}</p>
                    <div className="flex items-center gap-1 mt-1">
                      <EnvelopeIcon className="size-3 text-parchment-400" />
                      <p className="text-xs text-parchment-500 dark:text-slate-400 truncate">{s.studentEmail}</p>
                    </div>
                  </div>
                </CardContent>
              </Card>
            ))}
          </div>
        )}
      </section>
    </div>
  );
}
