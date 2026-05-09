import { useState, useEffect } from "react";
import { useAuth } from "../AuthContext";
import { useNavigate } from "react-router-dom";
import AccountsTable from "../components/admin/AccountsTable";
import CreateAccountForm from "../components/admin/CreateAccountForm";
import EditAccountForm from "../components/admin/EditAccountForm";
import ProfilesTable from "../components/admin/ProfilesTable";
import CreateProfileForm from "../components/admin/CreateProfileForm";
import EditProfileForm from "../components/admin/EditProfileForm";
import "../styles/adminpage.css";

export default function AdminPage() {
  const { logout } = useAuth();
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState("accounts");
  const [accounts, setAccounts] = useState([]);
  const [profiles, setProfiles] = useState([]);
  const [accountSearch, setAccountSearch] = useState("");
  const [profileSearch, setProfileSearch] = useState("");
  const [error, displayError] = useState("");
  const [editingAccount, setEditingAccount] = useState(null);
  const [editingProfile, setEditingProfile] = useState(null);
  

  useEffect(() => {
    fetchAccounts();
    fetchProfiles();
  }, []);

  const fetchAccounts = async () => {
    displayError("");
    const res = await fetch("/api/ViewAllUserAccount", 
      { credentials: "include" });
    if (!res.ok) { displayError("Failed to load accounts"); return; }
    setAccounts(await res.json());
  };

  const fetchProfiles = async () => {
    const res = await fetch("/api/ViewAllUserProfile", {
      credentials: "include" 
    });
    if (!res.ok) { displayError("Failed to load profiles"); return; }
    const data = await res.json();
    setProfiles(data);
  };

  const handleSuspendAccount = async (id, suspend) => {
    displayError("");
    const res = await fetch(`/api/SuspendUserAccount?userId=${encodeURIComponent(id)}&suspendUser=${encodeURIComponent(suspend)}`, {
      method: "PUT",
      credentials: "include",
    });
    if (!res.ok) { displayError(await res.text()); return; }
    fetchAccounts();
  };
  
  const handleSearch = async () => {
    if (!accountSearch.trim()) { fetchAccounts(); return; }
    displayError("");
    setAccounts([]);
    const res = await fetch(`/api/SearchUserAccount?query=${encodeURIComponent(accountSearch)}`, {
      method: "POST",
      credentials: "include",
    });
    if (!res.ok) { displayError(await res.text()); return; }
    const data = await res.json();
    console.log("Search results:", data);
    setAccounts(Array.isArray(data) ? data : [data]);
  };

  const handleReset = () => {
    setAccountSearch("");
    fetchAccounts();
  };

  const handleSuspendProfile = async (id, suspend) => {
    displayError("");
    const res = await fetch(`/api/SuspendUserProfile`, {
      method: "PUT",
      credentials: "include",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ Id: id, Status: suspend })
    });
    if (!res.ok) { displayError(await res.text()); return; }
    fetchProfiles();
  };

  const handleLogout = async () => {
    await logout();
    navigate("/login");
  };

  return (
    <div className="admin-wrap">
      <aside className="admin-sidebar">
        <div className="sidebar-logo">Admin panel</div>

        <div className="sidebar-section">Profiles</div>
        <div className={`nav-item ${activeTab === "profiles" ? "active" : ""}`}
          onClick={() => setActiveTab("profiles")}>
          View profiles
        </div>
        <div className={`nav-item ${activeTab === "createProfile" ? "active" : ""}`}
          onClick={() => setActiveTab("createProfile")}>
          Create profile
        </div>

        <div className="sidebar-section">Accounts</div>
        <div className={`nav-item ${activeTab === "accounts" ? "active" : ""}`}
          onClick={() => setActiveTab("accounts")}>
          View accounts
        </div>
        <div className={`nav-item ${activeTab === "createAccount" ? "active" : ""}`}
          onClick={() => setActiveTab("createAccount")}>
          Create account
        </div>

        <div className="sidebar-bottom">
          <div className="logout-btn" onClick={handleLogout}>Log out</div>
        </div>
      </aside>

      <main className="admin-main">
        {error && <div className="form-error">{error}</div>}

        {activeTab === "accounts" && (
          <AccountsTable
            accounts={accounts}
            search={accountSearch}
            setSearch={setAccountSearch}
            onSuspend={handleSuspendAccount}
            onSuccess={fetchAccounts}
            onSearch={handleSearch}
            onReset={handleReset}
            profiles={profiles}
          />
        )}

        {activeTab === "createAccount" && (
          <CreateAccountForm
            profiles={profiles}
            onSuccess={() => { setActiveTab("accounts"); fetchAccounts(); }}
            onCancel={() => setActiveTab("accounts")}
          />
        )}

        {activeTab === "editAccount" && editingAccount && (
          <EditAccountForm
            account={editingAccount}
            profiles={profiles}
            onSuccess={() => { setActiveTab("accounts"); fetchAccounts(); }}
            onCancel={() => setActiveTab("accounts")}
          />
        )}

        {activeTab === "profiles" && (
          <ProfilesTable
            profiles={profiles}
            search={profileSearch}
            setSearch={setProfileSearch}
            onSuspend={handleSuspendProfile}
            onEdit={(p) => { setEditingProfile(p); setActiveTab("editProfile"); }}
          />
        )}

        {activeTab === "createProfile" && (
          <CreateProfileForm
            onSuccess={() => { setActiveTab("profiles"); fetchProfiles(); }}
            onCancel={() => setActiveTab("profiles")}
          />
        )}

        {activeTab === "editProfile" && editingProfile && (
          <EditProfileForm
            profile={editingProfile}
            onSuccess={() => { setActiveTab("profiles"); fetchProfiles(); }}
            onCancel={() => setActiveTab("profiles")}
          />
        )}
      </main>
    </div>
  );
}