import { useEffect } from "react";
import { useState } from "react";
import { useAuth } from "../AuthContext";
import { useNavigate } from "react-router-dom";

export default function FundraiserPage() {
  const { logout } = useAuth();

  const fetchFRA = async () => {
    setError("");
    const res = await fetch("/api/ViewAllFundraiserController", 
      { credentials: "include" });
    if (!res.ok) { setError("Failed to load Fundraiser Activities"); return; }
    setAccounts(await res.json());
  };
  
  const handleLogout = async () => {
      await logout();
      navigate("/login");
    };
  
  return (
    <div style={styles.page}>
      <h1 style={styles.heading}>Fundraiser Dashboard</h1>
    </div>
  );
}