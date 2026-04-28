export default function AccountsTable({ accounts, search, setSearch, onSuspend, onEdit }) {
  const filtered = accounts.filter(acc =>
    acc.name?.toLowerCase().includes(search.toLowerCase()) ||
    acc.email?.toLowerCase().includes(search.toLowerCase()) ||
    acc.phoneNumber?.toLowerCase().includes(search.toLowerCase()) ||
    acc.profileName?.toLowerCase().includes(search.toLowerCase())
  );

  return (
    <>
      <div className="admin-topbar">
        <h1>User accounts</h1>
        <input
          className="admin-search"
          placeholder="Search name, email or phone..."
          value={search}
          onChange={e => setSearch(e.target.value)}
        />
      </div>

      <div className="admin-metrics">
        <div className="metric">
          <div className="metric-label">Total accounts</div>
          <div className="metric-val">{filtered.length}</div>
        </div>
        <div className="metric">
          <div className="metric-label">Active</div>
          <div className="metric-val">{filtered.filter(a => !a.isSuspended).length}</div>
        </div>
        <div className="metric">
          <div className="metric-label">Suspended</div>
          <div className="metric-val">{filtered.filter(a => a.isSuspended).length}</div>
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
            {filtered.map(acc => (
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
                  <button className="action-btn" onClick={() => onEdit(acc)}>Edit</button>
                  <button
                    className={`action-btn ${!acc.isSuspended ? "danger" : ""}`}
                    onClick={() => onSuspend(acc.id, !acc.isSuspended)}>
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