// components/admin/AccountsTab.jsx
export function AccountsTab({ accounts, search, setSearch, onSearch, onSuspend, onRefresh }) {
  return (
    <>
      <div className="admin-topbar">
        <h1>User accounts</h1>
        <div style={{ display: "flex", gap: "8px" }}>
          <input placeholder="Search name, email, phone..."
            value={search} onChange={e => setSearch(e.target.value)}
            onKeyDown={e => e.key === "Enter" && onSearch()}
            className="admin-search" />
          <button onClick={onSearch} className="admin-btn">Search</button>
          <button onClick={onRefresh} className="admin-btn">Reset</button>
        </div>
      </div>

      <div className="admin-metrics">
        <div className="metric"><div className="metric-label">Total</div><div className="metric-val">{accounts.length}</div></div>
        <div className="metric"><div className="metric-label">Active</div><div className="metric-val">{accounts.filter(a => !a.isSuspended).length}</div></div>
        <div className="metric"><div className="metric-label">Suspended</div><div className="metric-val">{accounts.filter(a => a.isSuspended).length}</div></div>
      </div>

      <div className="admin-table-card">
        <table>
          <thead>
            <tr><th>Name</th><th>Email</th><th>Phone</th><th>Profile</th><th>Status</th><th>Actions</th></tr>
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
                  <button className="action-btn" onClick={() => onSuspend(acc.email, !acc.isSuspended)}>
                    {acc.isSuspended ? "Unsuspend" : "Suspend"}
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </>
  );
}