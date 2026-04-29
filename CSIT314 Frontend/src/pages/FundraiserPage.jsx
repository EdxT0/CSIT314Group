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
/*        public int Id { get; set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Deadline { get; private set; }
        public bool Status { get; private set; }
        public double AmtRequested { get; private set; }
        public double AmtDonated { get; private set; }
        public int AmtOfViews { get; private set; }

        public string FraCategoryName { get; private set; } */

const styles = {
  page: {
    height: "100vh",
    display: "flex",
    justifyContent: "center",
    alignItems: "center",
    backgroundColor: "#f9fafb",
  },
  heading: {
    fontSize: "32px",
    fontWeight: "bold",
    color: "black",
  },
};