import { Suspense } from "react";
import { RouterProvider } from "react-router-dom";
import { Provider } from "react-redux";
import { store } from "./store/store";
import router from "./router";

export default function App() {
  return (
    <Provider store={store}>
      <Suspense fallback={<div className="flex min-h-screen items-center justify-center text-gray-600">Loading...</div>}>
        <RouterProvider router={router} />
      </Suspense>
    </Provider>
  );
}
