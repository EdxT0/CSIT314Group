import { useState, useEffect } from "react";
import { useAuth } from "../AuthContext";
import { useNavigate } from "react-router-dom";
import AccountsTable from "../components/admin/AccountsTable";
import CreateAccountForm from "../components/admin/CreateAccountForm";
import EditAccountForm from "../components/admin/EditAccountForm";
import "../styles/adminpage.css";

export default function AdminPage() {
  const { logout } = useAuth();
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState("accounts");
  const [accounts, setAccounts] = useState([]);
  const [profiles, setProfiles] = useState([]);
  const [search, setSearch] = useState("");
  const [error, setError] = useState("");
  const [editingAccount, setEditingAccount] = useState(null);

  useEffect(() => {
    fetchAccounts();
    fetchProfiles();
  }, []);

  const fetchAccounts = async () => {
    setError("");
    const res = await fetch("/api/ViewAllUserAccount", { credentials: "include" });
    if (!res.ok) { setError("Failed to load accounts"); return; }
    setAccounts(await res.json());
  };

  const fetchProfiles = async () => {
    const res = await fetch("/api/ViewAllUserProfile", { credentials: "include" });
    if (!res.ok) return;
    setProfiles(await res.json());
  };

  const handleSuspend = async (email, suspend) => {
    setError("");
    const res = await fetch("/api/SuspendUserAccount", {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify({ email, suspendUser: suspend }),
    });
    if (!res.ok) { setError(await res.text()); return; }
    fetchAccounts();
  };

  const handleLogout = async () => {
    await logout();
    navigate("/login");
  };

  return (
    <div className="admin-wrap">
      <aside className="admin-sidebar">
        <div className="sidebar-logo">Admin panel</div>

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
            search={search}
            setSearch={setSearch}
            onSuspend={handleSuspend}
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
      </main>
    </div>
  );
}