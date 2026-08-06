import { AcademicCapIcon, EnvelopeIcon } from "@heroicons/react/24/outline";
import { Card, CardContent } from "./ui/card";

interface StudentCardProps {
  displayName: string;
  email: string;
}

export function StudentCard({ displayName, email }: StudentCardProps) {
  return (
    <Card className="p-5">
      <CardContent className="flex items-start gap-4">
        <div className="size-10 rounded-full bg-cerulean-100 dark:bg-cerulean-900 flex items-center justify-center shrink-0">
          <AcademicCapIcon className="size-5 text-cerulean-600 dark:text-cerulean-400" />
        </div>
        <div className="min-w-0">
          <p className="font-medium text-parchment-900 dark:text-white truncate">{displayName}</p>
          <div className="flex items-center gap-1 mt-1">
            <EnvelopeIcon className="size-3 text-parchment-400" />
            <p className="text-xs text-parchment-500 dark:text-slate-400 truncate">{email}</p>
          </div>
        </div>
      </CardContent>
    </Card>
  );
}
