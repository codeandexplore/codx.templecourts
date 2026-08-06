import { useForm } from "react-hook-form";
import { z } from "zod/v4";
import { zodResolver } from "@hookform/resolvers/zod";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";
import { useState, useEffect } from "react";
import { EnvelopeIcon, LockClosedIcon, UserIcon } from "@heroicons/react/24/outline";
import { Button } from "../components/ui/button";
import { Input } from "../components/ui/input";
import { Card, CardHeader, CardTitle, CardContent, CardFooter } from "../components/ui/card";

const schema = z.object({
  email: z.string().email(),
  displayName: z.string().min(1, "Display name is required").max(100),
  password: z.string().min(8),
});

type FormData = z.infer<typeof schema>;

export default function RegisterPage() {
  const { register: registerUser, isAuthenticated } = useAuth();
  const navigate = useNavigate();
  const [error, setError] = useState("");
  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm<FormData>({ resolver: zodResolver(schema) });

  useEffect(() => {
    if (isAuthenticated) navigate("/", { replace: true });
  }, [isAuthenticated, navigate]);

  const onSubmit = async (data: FormData) => {
    setError("");
    try {
      await registerUser(data.email, data.password, data.displayName);
      navigate("/", { replace: true });
    } catch (e: unknown) {
      const err = e as { data?: { error?: string } };
      setError(err?.data?.error || "Registration failed");
    }
  };

  return (
    <div className="flex min-h-screen items-center justify-center bg-parchment-50 dark:bg-slate-950 px-4">
      <Card className="w-full max-w-md p-8">
        <CardHeader>
          <CardTitle className="font-serif text-2xl text-center text-parchment-900 dark:text-white">Create account</CardTitle>
        </CardHeader>
        <CardContent>
          {error && <p className="text-sm text-red-600 text-center mb-4">{error}</p>}
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
            <div className="space-y-1.5">
              <label htmlFor="register-displayname" className="text-sm font-medium text-parchment-700 dark:text-slate-300">Display Name</label>
              <div className="relative">
                <UserIcon className="absolute left-3 top-1/2 -translate-y-1/2 size-4 text-parchment-400" />
                <Input id="register-displayname" {...register("displayName")} className="pl-9" placeholder="Your name" />
              </div>
              {errors.displayName && <p className="text-sm text-red-600">{errors.displayName.message}</p>}
            </div>
            <div className="space-y-1.5">
              <label htmlFor="register-email" className="text-sm font-medium text-parchment-700 dark:text-slate-300">Email</label>
              <div className="relative">
                <EnvelopeIcon className="absolute left-3 top-1/2 -translate-y-1/2 size-4 text-parchment-400" />
                <Input id="register-email" {...register("email")} type="email" className="pl-9" placeholder="you@example.com" />
              </div>
              {errors.email && <p className="text-sm text-red-600">{errors.email.message}</p>}
            </div>
            <div className="space-y-1.5">
              <label htmlFor="register-password" className="text-sm font-medium text-parchment-700 dark:text-slate-300">Password</label>
              <div className="relative">
                <LockClosedIcon className="absolute left-3 top-1/2 -translate-y-1/2 size-4 text-parchment-400" />
                <Input id="register-password" {...register("password")} type="password" className="pl-9" placeholder="••••••••" />
              </div>
              {errors.password && <p className="text-sm text-red-600">{errors.password.message}</p>}
            </div>
            <Button type="submit" disabled={isSubmitting} className="w-full bg-cerulean-600 hover:bg-cerulean-700 text-white h-10">
              {isSubmitting ? "Creating account..." : "Create account"}
            </Button>
          </form>
        </CardContent>
        <CardFooter>
          <p className="text-center text-sm text-parchment-500 dark:text-slate-400 w-full">
            Already have an account? <Link to="/login" className="text-cerulean-600 hover:underline">Sign in</Link>
          </p>
        </CardFooter>
      </Card>
    </div>
  );
}
