import { createBrowserRouter } from "react-router-dom";
import AppLayout from "./layouts/AppLayout";
import ProtectedRoute from "./components/ProtectedRoute";
import RequireRole from "./components/RequireRole";
import HomePage from "./pages/HomePage";
import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";
import LessonsPage from "./pages/LessonsPage";
import LessonDetailPage from "./pages/LessonDetailPage";
import AttemptPage from "./pages/AttemptPage";
import TeacherPage from "./pages/TeacherPage";
import AdminPage from "./pages/AdminPage";
import EditorPage from "./pages/EditorPage";
import ReviewPage from "./pages/ReviewPage";
import CheckQuestionsPage from "./pages/CheckQuestionsPage";

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
          { path: "lessons", element: <LessonsPage /> },
          { path: "lessons/:key", element: <LessonDetailPage /> },
          { path: "attempt/:attemptId", element: <AttemptPage /> },
          {
            element: <RequireRole role="Teacher" />,
            children: [
              { path: "teacher", element: <TeacherPage /> },
              { path: "teacher/review/:sessionId", element: <ReviewPage /> },
              { path: "teacher/check-questions", element: <CheckQuestionsPage /> },
            ],
          },
          {
            element: <RequireRole role="Admin" />,
            children: [
              { path: "admin", element: <AdminPage /> },
              { path: "admin/editor", element: <EditorPage /> },
            ],
          },
        ],
      },
    ],
  },
]);
