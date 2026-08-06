import { useForm } from "react-hook-form";
import { z } from "zod/v4";
import { zodResolver } from "@hookform/resolvers/zod";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";
import { useState, useEffect } from "react";
import { EnvelopeIcon, LockClosedIcon } from "@heroicons/react/24/outline";
import { Button } from "../components/ui/button";
import { Input } from "../components/ui/input";
import { Card, CardHeader, CardTitle, CardContent, CardFooter } from "../components/ui/card";

const schema = z.object({
  email: z.string().email(),
  password: z.string().min(8),
});

type FormData = z.infer<typeof schema>;

export default function LoginPage() {
  const { login, isAuthenticated } = useAuth();
  const navigate = useNavigate();
  const [error, setError] = useState("");
  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm<FormData>({ resolver: zodResolver(schema) });

  useEffect(() => {
    if (isAuthenticated) navigate("/", { replace: true });
  }, [isAuthenticated, navigate]);

  const onSubmit = async (data: FormData) => {
    setError("");
    try {
      await login(data.email, data.password);
      navigate("/", { replace: true });
    } catch (e: unknown) {
      const err = e as { data?: { error?: string } };
      setError(err?.data?.error || "Login failed");
    }
  };

  return (
    <div className="flex min-h-screen items-center justify-center bg-parchment-50 dark:bg-slate-950 px-4">
      <Card className="w-full max-w-md p-8">
        <CardHeader>
          <CardTitle className="font-serif text-2xl text-center text-parchment-900 dark:text-white">Sign in</CardTitle>
        </CardHeader>
        <CardContent>
          {error && <p className="text-sm text-red-600 text-center mb-4">{error}</p>}
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
            <div className="space-y-1.5">
              <label htmlFor="login-email" className="text-sm font-medium text-parchment-700 dark:text-slate-300">Email</label>
              <div className="relative">
                <EnvelopeIcon className="absolute left-3 top-1/2 -translate-y-1/2 size-4 text-parchment-400" />
                <Input id="login-email" {...register("email")} type="email" className="pl-9" placeholder="you@example.com" />
              </div>
              {errors.email && <p className="text-sm text-red-600">{errors.email.message}</p>}
            </div>
            <div className="space-y-1.5">
              <label htmlFor="login-password" className="text-sm font-medium text-parchment-700 dark:text-slate-300">Password</label>
              <div className="relative">
                <LockClosedIcon className="absolute left-3 top-1/2 -translate-y-1/2 size-4 text-parchment-400" />
                <Input id="login-password" {...register("password")} type="password" className="pl-9" placeholder="••••••••" />
              </div>
              {errors.password && <p className="text-sm text-red-600">{errors.password.message}</p>}
            </div>
            <Button type="submit" disabled={isSubmitting} className="w-full bg-cerulean-600 hover:bg-cerulean-700 text-white h-10">
              {isSubmitting ? "Signing in..." : "Sign in"}
            </Button>
          </form>
        </CardContent>
        <CardFooter>
          <p className="text-center text-sm text-parchment-500 dark:text-slate-400 w-full">
            Don&apos;t have an account? <Link to="/register" className="text-cerulean-600 hover:underline">Register</Link>
          </p>
        </CardFooter>
      </Card>
    </div>
  );
}
