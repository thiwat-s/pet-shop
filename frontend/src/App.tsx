import { useState } from "react";
import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom";
import { DashboardPage } from "./pages/DashboardPage";
import { LoginPage } from "./pages/LoginPage";
import type { LoginResult } from "./types/pet";

const stored = sessionStorage.getItem("pet-shop-session");

export default function App() {
  const [session, setSession] = useState<LoginResult | null>(
    stored ? (JSON.parse(stored) as LoginResult) : null,
  );
  const signIn = (result: LoginResult) => {
    sessionStorage.setItem("pet-shop-session", JSON.stringify(result));
    setSession(result);
  };
  const signOut = () => {
    sessionStorage.removeItem("pet-shop-session");
    setSession(null);
  };
  return (
    <BrowserRouter>
      <Routes>
        <Route
          path="/login"
          element={
            session ? (
              <Navigate to="/dashboard" replace />
            ) : (
              <LoginPage onLogin={signIn} />
            )
          }
        />
        <Route
          path="/dashboard"
          element={
            session ? (
              <DashboardPage
                token={session.token}
                username={session.username}
                onLogout={signOut}
              />
            ) : (
              <Navigate to="/login" replace />
            )
          }
        />
        <Route
          path="*"
          element={<Navigate to={session ? "/dashboard" : "/login"} replace />}
        />
      </Routes>
    </BrowserRouter>
  );
}
