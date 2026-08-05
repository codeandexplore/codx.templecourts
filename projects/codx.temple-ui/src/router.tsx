import { lazy } from "react";
import { createBrowserRouter } from "react-router-dom";
import AppLayout from "./layouts/AppLayout";
import ProtectedRoute from "./components/ProtectedRoute";
import RequireRole from "./components/RequireRole";
import HomePage from "./pages/HomePage";

const LoginPage = lazy(() => import("./pages/LoginPage"));
const RegisterPage = lazy(() => import("./pages/RegisterPage"));

export default createBrowserRouter([
  {
    path: "/login",
    element: <LoginPage />,
  },
  {
    path: "/register",
    element: <RegisterPage />,
  },
  {
    element: <ProtectedRoute />,
    children: [
      {
        element: <AppLayout />,
        children: [
          { index: true, element: <HomePage /> },
          { path: "lessons", lazy: () => import("./pages/LessonsPage") },
          { path: "lessons/:key", lazy: () => import("./pages/LessonDetailPage") },
          {
            element: <RequireRole role="Teacher" />,
            children: [
              { path: "teacher", lazy: () => import("./pages/TeacherPage") },
            ],
          },
          {
            element: <RequireRole role="Admin" />,
            children: [
              { path: "admin", lazy: () => import("./pages/AdminPage") },
            ],
          },
        ],
      },
    ],
  },
]);
