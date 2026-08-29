import { useState, useMemo } from "react";
import { useNavigate } from "react-router-dom";
import { useGetTeacherStudentsQuery, useStartSessionMutation, useClaimStudentMutation, useListUnassignedStudentsQuery } from "../services/teacherApi";
import { AcademicCapIcon, EnvelopeIcon, ArrowRightCircleIcon, ChevronDownIcon } from "@heroicons/react/24/outline";
import { Card, CardContent } from "../components/ui/card";
import { Button } from "../components/ui/button";

export default function TeacherPage() {
  const { data: students, isLoading } = useGetTeacherStudentsQuery();
  const { data: unassigned } = useListUnassignedStudentsQuery();
  const [startSession] = useStartSessionMutation();
  const [claimStudent, { isLoading: claiming }] = useClaimStudentMutation();
  const navigate = useNavigate();
  const [studentId, setStudentId] = useState("");
  const [search, setSearch] = useState("");
  const [dropdownOpen, setDropdownOpen] = useState(false);
  const [claimError, setClaimError] = useState("");

  const filteredUnassigned = useMemo(() => {
    if (!unassigned) return [];
    if (!search.trim()) return unassigned;
    const q = search.toLowerCase();
    return unassigned.filter(
      (u) => u.email.toLowerCase().includes(q) || u.displayName.toLowerCase().includes(q)
    );
  }, [unassigned, search]);

  const selectedStudent = unassigned?.find((u) => u.id === studentId);

  const handleStartReview = async (attemptId: string) => {
    try {
      const session = await startSession({ lessonAttemptId: attemptId }).unwrap();
      navigate(`/teacher/review/${session.id}`);
    } catch { /* handled by RTK */ }
  };

  const handleClaim = async () => {
    if (!studentId) return;
    setClaimError("");
    try {
      await claimStudent(studentId).unwrap();
      setStudentId("");
      setSearch("");
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
        ) : students && students.length === 0 ? (
          <Card className="p-6 max-w-lg">
            <CardContent className="flex flex-col items-center gap-3">
              <AcademicCapIcon className="size-10 text-parchment-300 dark:text-slate-600" />
              <p className="text-sm text-parchment-500 dark:text-slate-400 text-center">
                No students assigned yet. Select a student below to claim them.
              </p>

              {filteredUnassigned.length === 0 ? (
                <p className="text-sm text-parchment-400 dark:text-slate-500 text-center">
                  No unassigned students available.
                </p>
              ) : (
                <div className="w-full flex flex-col gap-2">
                  <div className="relative">
                    <button
                      type="button"
                      onClick={() => setDropdownOpen(!dropdownOpen)}
                      className="w-full flex items-center justify-between rounded-lg border border-parchment-200 dark:border-slate-700 bg-white dark:bg-slate-900 px-3 py-2 text-sm dark:text-white"
                    >
                      <span className={selectedStudent ? "text-parchment-900 dark:text-white" : "text-slate-400"}>
                        {selectedStudent
                          ? `${selectedStudent.displayName} (${selectedStudent.email})`
                          : "Search students..."}
                      </span>
                      <ChevronDownIcon className="size-4 ml-2 shrink-0 opacity-50" />
                    </button>
                    {dropdownOpen && (
                      <div className="absolute z-50 mt-1 w-full rounded-lg border border-parchment-200 dark:border-slate-700 bg-white dark:bg-slate-900 shadow-lg">
                        <div className="p-2 border-b border-parchment-100 dark:border-slate-800">
                          <input
                            type="text"
                            value={search}
                            onChange={(e) => setSearch(e.target.value)}
                            placeholder="Filter by name or email..."
                            autoFocus
                            className="w-full rounded-lg border border-parchment-200 dark:border-slate-700 bg-parchment-50 dark:bg-slate-800 px-3 py-1.5 text-sm focus:outline-none focus:ring-2 focus:ring-cerulean-500 dark:text-white"
                          />
                        </div>
                        <div className="max-h-48 overflow-y-auto">
                          {filteredUnassigned.map((u) => (
                            <button
                              key={u.id}
                              type="button"
                              onClick={() => {
                                setStudentId(u.id);
                                setSearch("");
                                setDropdownOpen(false);
                              }}
                              className={`w-full text-left px-3 py-2 text-sm hover:bg-parchment-50 dark:hover:bg-slate-800 ${
                                studentId === u.id ? "bg-cerulean-50 dark:bg-cerulean-900/20" : ""
                              }`}
                            >
                              <span className="font-medium text-parchment-900 dark:text-white">{u.displayName}</span>
                              <span className="ml-2 text-xs text-parchment-400 dark:text-slate-500">{u.email}</span>
                            </button>
                          ))}
                        </div>
                      </div>
                    )}
                  </div>
                  <Button onClick={handleClaim} disabled={claiming || !studentId}
                    className="bg-cerulean-600 hover:bg-cerulean-700 text-white w-full">
                    {claiming ? "Claiming..." : "Claim Student"}
                  </Button>
                  {claimError && <p className="text-sm text-red-600 w-full">{claimError}</p>}
                </div>
              )}
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
