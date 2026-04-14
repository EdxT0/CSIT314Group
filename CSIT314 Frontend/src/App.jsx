import { BrowserRouter, Routes, Route } from "react-router-dom";
import LoginPage from "./pages/LoginPage";
import AdminPage from "./pages/AdminPage";
import DoneePage from "./pages/DoneePage";
import FundraiserPage from "./pages/FundraiserPage";
import PlatformManagerPage from "./pages/PlatformManagerPage";

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<LoginPage />} />
        <Route path="/admin" element={<AdminPage />} />
        <Route path="/donee" element={<DoneePage />} />
        <Route path="/fundraiser" element={<FundraiserPage />} />
        <Route path="/platform-manager" element={<PlatformManagerPage />} />
      </Routes>
    </BrowserRouter>
  );
}