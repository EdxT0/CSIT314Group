export default function ProfilesTable({ profiles, search, setSearch, onSuspend, onEdit }) {
  const filtered = profiles.filter(p =>
    p.profileName?.toLowerCase().includes(search.toLowerCase()) ||
    p.description?.toLowerCase().includes(search.toLowerCase())
  );

  return (
    <>
      <div className="admin-topbar">
        <h1>User profiles</h1>
        <input
          className="admin-search"
          placeholder="Search profiles..."
          value={search}
          onChange={e => setSearch(e.target.value)}
        />
      </div>

      <div className="admin-metrics">
        <div className="metric">
          <div className="metric-label">Total profiles</div>
          <div className="metric-val">{filtered.length}</div>
        </div>
        <div className="metric">
          <div className="metric-label">Active</div>
          <div className="metric-val">{filtered.filter(p => p.status === "Active").length}</div>
        </div>
        <div className="metric">
          <div className="metric-label">Suspended</div>
          <div className="metric-val">{filtered.filter(p => p.status === "Suspended").length}</div>
        </div>
      </div>

      <div className="admin-table-card">
        <table>
          <thead>
            <tr>
              <th>Profile name</th>
              <th>Description</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {filtered.map(p => (
              <tr key={p.id}>
                <td>{p.profileName}</td>
                <td>{p.description}</td>
                <td>
                  <span className={`badge ${p.status === "Active" ? "badge-active" : "badge-suspended"}`}>
                    {p.status}
                  </span>
                </td>
                <td>
                  <button className="action-btn" onClick={() => onEdit(p)}>Edit</button>
                  <button
                    className={`action-btn ${p.status === "Active" ? "danger" : ""}`}
                    onClick={() => onSuspend(p.id, p.status === "Active" ? "Suspended" : "Active")}>
                    {p.status === "Active" ? "Suspend" : "Unsuspend"}
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