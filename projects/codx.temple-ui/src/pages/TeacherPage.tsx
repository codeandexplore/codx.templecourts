import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useGetTeacherStudentsQuery, useStartSessionMutation, useClaimStudentMutation } from "../services/teacherApi";
import { AcademicCapIcon, EnvelopeIcon, ArrowRightCircleIcon } from "@heroicons/react/24/outline";
import { Card, CardContent } from "../components/ui/card";
import { Button } from "../components/ui/button";
import { Input } from "../components/ui/input";

export default function TeacherPage() {
  const { data: students, isLoading } = useGetTeacherStudentsQuery();
  const [startSession] = useStartSessionMutation();
  const [claimStudent, { isLoading: claiming }] = useClaimStudentMutation();
  const navigate = useNavigate();
  const [studentId, setStudentId] = useState("");
  const [claimError, setClaimError] = useState("");

  const handleStartReview = async (attemptId: string) => {
    try {
      const session = await startSession({ lessonAttemptId: attemptId }).unwrap();
      navigate(`/teacher/review/${session.id}`);
    } catch { /* handled by RTK */ }
  };

  const handleClaim = async () => {
    if (!studentId.trim()) return;
    setClaimError("");
    try {
      await claimStudent(studentId.trim()).unwrap();
      setStudentId("");
    } catch (e: unknown) {
      const err = e as { data?: { error?: string } };
      setClaimError(err?.data?.error || "Failed to claim student");
    }
  };

  return (
    <div>
      <h2 className="font-serif text-2xl font-semibold text-parchment-900 dark:text-white mb-6">Teacher Dashboard</h2>
      <section>
        <h3 className="text-lg font-medium text-parchment-800 dark:text-slate-200 mb-4">My Students</h3>
        {isLoading ? (
          <p className="text-parchment-500 dark:text-slate-400">Loading...</p>
        ) : students?.length === 0 ? (
          <Card className="p-6 max-w-lg">
            <CardContent className="flex flex-col items-center gap-3">
              <AcademicCapIcon className="size-10 text-parchment-300 dark:text-slate-600" />
              <p className="text-sm text-parchment-500 dark:text-slate-400 text-center">
                No students assigned yet. Enter a student's ID below to claim them.
              </p>
              <div className="flex gap-2 w-full">
                <Input
                  value={studentId}
                  onChange={(e) => setStudentId(e.target.value)}
                  placeholder="Student ID (GUID)"
                  className="flex-1"
                />
                <Button onClick={handleClaim} disabled={claiming || !studentId.trim()}
                  className="bg-cerulean-600 hover:bg-cerulean-700 text-white shrink-0">
                  {claiming ? "Claiming..." : "Claim"}
                </Button>
              </div>
              {claimError && <p className="text-sm text-red-600 w-full">{claimError}</p>}
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
                  <div className="min-w-0 flex-1">
                    <p className="font-medium text-parchment-900 dark:text-white truncate">{s.studentDisplayName}</p>
                    <div className="flex items-center gap-1 mt-1">
                      <EnvelopeIcon className="size-3 text-parchment-400" />
                      <p className="text-xs text-parchment-500 dark:text-slate-400 truncate">{s.studentEmail}</p>
                    </div>
                    {s.latestAttemptId ? (
                      <Button
                        variant="ghost"
                        size="sm"
                        className="mt-2 text-cerulean-600 dark:text-cerulean-400 text-xs"
                        onClick={() => handleStartReview(s.latestAttemptId!)}
                      >
                        <ArrowRightCircleIcon className="size-3.5 mr-1" />
                        Start Review
                      </Button>
                    ) : (
                      <p className="text-xs text-parchment-400 dark:text-slate-500 mt-2">No active attempt</p>
                    )}
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
