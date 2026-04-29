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
  const [error, setError] = useState("");
  const [editingAccount, setEditingAccount] = useState(null);
  const [editingProfile, setEditingProfile] = useState(null);
  

  useEffect(() => {
    fetchAccounts();
    fetchProfiles();
  }, []);

  const fetchAccounts = async () => {
    setError("");
    const res = await fetch("/api/ViewAllUserAccount", 
      { credentials: "include" });
    if (!res.ok) { setError("Failed to load accounts"); return; }
    setAccounts(await res.json());
  };

  const fetchProfiles = async () => {
    const res = await fetch("/api/ViewAllUserProfile", {
      credentials: "include" 
    });
    if (!res.ok) return;
    const data = await res.json();
    setProfiles(data);
  };

  const handleSuspendAccount = async (id, suspend) => {
    setError("");
    const res = await fetch("/api/SuspendUserAccount", {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify({ userId: id, SuspendUser: suspend }),
    });
    if (!res.ok) { setError(await res.text()); return; }
    fetchAccounts();
  };
  
  const handleSuspendProfile = async (id, isSuspend) => {
    setError("");
    const res = await fetch("/api/SuspendUserProfile", {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
<<<<<<< HEAD
      body: JSON.stringify({ Id, isSuspend }),
=======
      body: JSON.stringify({ userId, isSuspend }),
>>>>>>> dafa3f2 (FRA)
    });
    if (!res.ok) { setError(await res.text()); return; }
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
            onEdit={(acc) => { setEditingAccount(acc); setActiveTab("editAccount"); }}
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