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
          <div className="metric-val">{filtered.filter(p => p.status == 0).length}</div>
        </div>
        <div className="metric">
          <div className="metric-label">Suspended</div>
          <div className="metric-val">{filtered.filter(p => p.status == 1).length}</div>
        </div>
      </div>

      <div className="admin-table-card">
        <table>
          <thead>
            <tr>
              <th>Profile name</th>
              <th>Description</th>
              <th>Status</th>
              <th>Actions</th>  {/* ← separate column for actions */}
            </tr>
          </thead>
          <tbody>
            {filtered.map(p => (
              <tr key={p.id}>
                <td>{p.profileName}</td>
                <td>{p.description}</td>
                <td>
                  <span className={`badge ${p.status == 0 ? "badge-active" : "badge-suspended"}`}>
                    {p.status == 0 ? "Active" : "Suspended"}
                  </span>
                </td>
                <td>  {/* ← actions in their own column */}
                  <button className="action-btn" onClick={() => onEdit(p)}>Edit</button>
                  <button
                    className={`action-btn ${p.status == 1 ? "danger" : ""}`}
                    onClick={() => onSuspend(p.id, p.status == 0)}>
                    {p.status == 0 ? "Suspend" : "Unsuspend"}
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