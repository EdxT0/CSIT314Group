import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { AuthProvider, useAuth } from "./AuthContext";
import { ProtectedRoute } from "./components/ProtectedRoute";
import LoginPage from "./pages/LoginPage";
import AdminPage from "./pages/AdminPage";
import DoneePage from "./pages/DoneePage";
import FundraiserPage from "./pages/FundraiserPage";
import PlatformManagerPage from "./pages/PlatformManagerPage";

function RoleRouter() {
  const { user } = useAuth();
  if (!user) return <Navigate to="/login" replace />;

  const routes = {
    admin: "/admin",
    donee: "/donee",
    "fundraiser manager": "/fundraiser",
    "platform manager": "/platform",
  };

  return <Navigate to={routes[user.role] ?? "/login"} replace />;
}

export default function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<RoleRouter />} />
          <Route path="/login" element={<LoginPage />} />

          <Route path="/admin" element={
            <ProtectedRoute allowedRoles={["admin"]}>
              <AdminPage />
            </ProtectedRoute>
          } />

          <Route path="/donee" element={
            <ProtectedRoute allowedRoles={["donee"]}>
              <DoneePage />
            </ProtectedRoute>
          } />

          <Route path="/fundraiser" element={
            <ProtectedRoute allowedRoles={["fundraiser manager"]}>
              <FundraiserPage />
            </ProtectedRoute>
          } />

          <Route path="/platform" element={
            <ProtectedRoute allowedRoles={["platform manager"]}>
              <PlatformManagerPage />
            </ProtectedRoute>
          } />

          <Route path="/unauthorized" element={<div>Access denied.</div>} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}