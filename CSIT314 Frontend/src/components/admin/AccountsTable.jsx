import { useState } from "react";
import AccountPopup from "./AccountPopup";

export default function AccountsTable({ accounts, profiles, search, setSearch, onSuspend, onSearch, onReset, onSuccess }) {
  const [selectedAccount, setSelectedAccount] = useState(null);

  function capitaliseNames(str) {
    if (!str) return str;
    return str
      .split(" ")
      .map(name => name.charAt(0).toUpperCase() + name.slice(1).toLowerCase())
      .join(" ");
  }

  return (
    <>
      <div className="admin-topbar">
        <h1>User accounts</h1>
        <div style={{ display: "flex", gap: "8px" }}>
          <input
            className="admin-search"
            placeholder="Search name, email or phone..."
            value={search}
            onChange={e => setSearch(e.target.value)}
            onKeyDown={e => e.key === "Enter" && onSearch()}
          />
          <button className="admin-btn" onClick={onSearch}>Search</button>
          <button className="admin-btn" onClick={onReset}>Reset</button>
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
            {accounts.length === 0 && (
              <tr><td colSpan={6} style={{ textAlign: "center", color: "#7a7d8a", padding: "2rem" }}>No accounts found</td></tr>
            )}
            {accounts.map((account, index) => (
              <tr
                key={`${account.id}-${index}`}
                onClick={() => setSelectedAccount(account)}
                style={{ cursor: "pointer" }}
              >
                <td>{capitaliseNames(account.name)}</td>
                <td>{account.email}</td>
                <td>{account.phoneNumber}</td>
                <td>{capitaliseNames(account.profileName)}</td>
                <td>
                  <span className={`badge ${account.isSuspended ? "badge-suspended" : "badge-active"}`}>
                    {account.isSuspended ? "Suspended" : "Active"}
                  </span>
                </td>
                <td>
                  <button
                    className="action-btn"
                    onClick={e => {
                      e.stopPropagation();
                      setSelectedAccount({ ...account, startEditing: true });
                    }}>
                    Edit
                  </button>
                  <button
                    className={`action-btn ${!account.isSuspended ? "danger" : ""}`}
                    onClick={e => {
                      e.stopPropagation();
                      onSuspend(account.id, !account.isSuspended);
                    }}>
                    {account.isSuspended ? "Unsuspend" : "Suspend"}
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {selectedAccount && (
        <AccountPopup
          acc={selectedAccount}
          onClose={async () => {
            await onSuccess?.();              // ← refetch before closing
            setSelectedAccount(null);
          }}
          onSuspend={onSuspend}
          profiles={profiles}
          onSuccess={async (message) => {
            await onSuccess?.(message);
          }}
        />
      )}    
    </>
  );
}