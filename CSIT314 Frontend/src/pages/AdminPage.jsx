import { useState, useEffect } from "react";
import { useAuth } from "../AuthContext";
import { useNavigate } from "react-router-dom";
import "../styles/adminpage.css";

export default function AdminPage() {
  const { logout } = useAuth();
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState("accounts");
  const [accounts, setAccounts] = useState([]);
  const [search, setSearch] = useState("");
  const [error, setError] = useState("");
  const [editingAccount, setEditingAccount] = useState(null);
  const [createForm, setCreateForm] = useState({
    name: "", email: "", phoneNumber: "",
    password: "", profileName: "", isSuspended: false
  });
  const [updateForm, setUpdateForm] = useState({
    id: "", name: "", email: "",
    phoneNumber: "", profileName: "", password: ""
  });

  useEffect(() => {
    if (activeTab === "accounts") fetchAccounts();
  }, [activeTab]);

  // ── Fetch all accounts ──────────────────────────────
  const fetchAccounts = async () => {
    setError("");
    const res = await fetch("/api/ViewAllUserAccount", {
      credentials: "include"
    });
    if (!res.ok) { setError("Failed to load accounts"); return; }
    const data = await res.json();
    setAccounts(data);
  };

  // ── Search ──────────────────────────────────────────
  const handleSearch = async () => {
    if (!search.trim()) { fetchAccounts(); return; }
    setError("");
    const res = await fetch("/api/SearchUserAccount", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify({ nameOrEmailOrPhone: search }),
    });
    if (res.status === 404) { setError("User not found"); setAccounts([]); return; }
    const data = await res.json();
    setAccounts([data]); // returns single user, wrap in array
  };

  // ── Create account ──────────────────────────────────
  const handleCreate = async () => {
    setError("");
    const res = await fetch("/api/CreateUserAccount", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify(createForm),
    });
    const text = await res.text();
    if (!res.ok) { setError(text); return; }
    setCreateForm({ name: "", email: "", phoneNumber: "", password: "", profileName: "", isSuspended: false });
    setActiveTab("accounts");
    fetchAccounts();
  };

  // ── Update account ──────────────────────────────────
  const handleUpdate = async () => {
    setError("");
    const res = await fetch("/api/UpdateUserAccount", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify(updateForm),
    });
    const text = await res.text();
    if (!res.ok) { setError(text); return; }
    setEditingAccount(null);
    setActiveTab("accounts");
    fetchAccounts();
  };

  // ── Suspend / unsuspend ─────────────────────────────
  const handleSuspend = async (email, suspend) => {
    setError("");
    const res = await fetch("/api/SuspendUserAccount", {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify({ email, suspendUser: suspend }),
    });
    if (!res.ok) { const text = await res.text(); setError(text); return; }
    fetchAccounts();
  };

  const handleLogout = async () => {
    await logout();
    navigate("/login");
  };

  const startEdit = (acc) => {
    setUpdateForm({ id: acc.id, name: acc.name, email: acc.email,
      phoneNumber: acc.phoneNumber, profileName: acc.profileName, password: "" });
    setEditingAccount(acc);
    setActiveTab("editAccount");
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

        {/* ── View accounts tab ── */}
        {activeTab === "accounts" && (
          <>
            <div className="admin-topbar">
              <h1>User accounts</h1>
              <div style={{ display: "flex", gap: "8px" }}>
                <input
                  className="admin-search"
                  placeholder="Search name, email or phone..."
                  value={search}
                  onChange={e => setSearch(e.target.value)}
                  onKeyDown={e => e.key === "Enter" && handleSearch()}
                />
                <button className="admin-btn" onClick={handleSearch}>Search</button>
                <button className="admin-btn" onClick={() => { setSearch(""); fetchAccounts(); }}>Reset</button>
              </div>
            </div>

            <div className="admin-metrics">
              <div className="metric">
                <div className="metric-label">Total accounts</div>
                <div className="metric-val">{accounts.length}</div>
              </div>
              <div className="metric">
                <div className="metric-label">Active</div>
                <div className="metric-val">{accounts.filter(a => !a.isSuspended).length}</div>
              </div>
              <div className="metric">
                <div className="metric-label">Suspended</div>
                <div className="metric-val">{accounts.filter(a => a.isSuspended).length}</div>
              </div>
            </div>

            <div className="admin-table-card">
              <table>
                <thead>
                  <tr>
                    <th>Name</th>
                    <th>Email</th>
                    <th>Phone</th>
                    <th>Profile</th>
                    <th>Status</th>
                    <th>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {accounts.map(acc => (
                    <tr key={acc.id}>
                      <td>{acc.name}</td>
                      <td>{acc.email}</td>
                      <td>{acc.phoneNumber}</td>
                      <td>{acc.profileName}</td>
                      <td>
                        <span className={`badge ${acc.isSuspended ? "badge-suspended" : "badge-active"}`}>
                          {acc.isSuspended ? "Suspended" : "Active"}
                        </span>
                      </td>
                      <td>
                        <button className="action-btn" onClick={() => startEdit(acc)}>Edit</button>
                        <button
                          className={`action-btn ${!acc.isSuspended ? "danger" : ""}`}
                          onClick={() => handleSuspend(acc.email, !acc.isSuspended)}>
                          {acc.isSuspended ? "Unsuspend" : "Suspend"}
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </>
        )}

        {/* ── Create account tab ── */}
        {activeTab === "createAccount" && (
          <div className="admin-form-card">
            <h2>Create account</h2>
            {["name", "email", "phoneNumber", "password", "profileName"].map(field => (
              <div className="form-field" key={field}>
                <label>{field.charAt(0).toUpperCase() + field.slice(1)}</label>
                <input
                  type={field === "password" ? "password" : "text"}
                  value={createForm[field]}
                  placeholder={field === "profileName" ? "e.g. admin, donee, fundraiser" : ""}
                  onChange={e => setCreateForm({ ...createForm, [field]: e.target.value })}
                />
              </div>
            ))}
            <div style={{ display: "flex", gap: "8px", marginTop: "0.5rem" }}>
              <button className="submit-btn" onClick={handleCreate}>Create account</button>
              <button className="admin-btn" onClick={() => setActiveTab("accounts")}>Cancel</button>
            </div>
          </div>
        )}

        {/* ── Edit account tab ── */}
        {activeTab === "editAccount" && (
          <div className="admin-form-card">
            <h2>Edit account</h2>
            <p style={{ fontSize: "13px", color: "#7a7d8a", marginBottom: "1.25rem" }}>
              Only fill in fields you want to change.
            </p>
            {["name", "email", "phoneNumber", "profileName", "password"].map(field => (
              <div className="form-field" key={field}>
                <label>{field.charAt(0).toUpperCase() + field.slice(1)}</label>
                <input
                  type={field === "password" ? "password" : "text"}
                  value={updateForm[field]}
                  placeholder={field === "password" ? "Leave blank to keep current" : ""}
                  onChange={e => setUpdateForm({ ...updateForm, [field]: e.target.value })}
                />
              </div>
            ))}
            <div style={{ display: "flex", gap: "8px", marginTop: "0.5rem" }}>
              <button className="submit-btn" onClick={handleUpdate}>Save changes</button>
              <button className="admin-btn" onClick={() => setActiveTab("accounts")}>Cancel</button>
            </div>
          </div>
        )}
      </main>
    </div>
  );
}