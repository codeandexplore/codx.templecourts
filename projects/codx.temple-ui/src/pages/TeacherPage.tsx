import { useGetTeacherStudentsQuery } from "../services/teacherApi";

export default function TeacherPage() {
  const { data: students, isLoading } = useGetTeacherStudentsQuery();

  return (
    <div>
      <h2 className="text-2xl font-semibold text-gray-900 dark:text-white mb-6">Teacher Dashboard</h2>
      <div className="grid gap-4">
        <section>
          <h3 className="text-lg font-medium text-gray-800 dark:text-gray-200 mb-3">My Students</h3>
          {isLoading ? (
            <p className="text-gray-600">Loading...</p>
          ) : students?.length === 0 ? (
            <p className="text-gray-500 dark:text-gray-400 text-sm">No students assigned yet. Claim a student to get started.</p>
          ) : (
            <div className="space-y-2">
              {students?.map((s) => (
                <div key={s.id} className="rounded-lg border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-900 p-4">
                  <p className="font-medium text-gray-900 dark:text-white">{s.studentDisplayName}</p>
                  <p className="text-sm text-gray-500 dark:text-gray-400">{s.studentEmail}</p>
                </div>
              ))}
            </div>
          )}
        </section>
      </div>
    </div>
  );
}
