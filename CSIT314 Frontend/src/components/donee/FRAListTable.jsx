export default function FRAListTable({ fras, search, setSearch, onSelect }) {
  const filtered = fras.filter(f =>
    f.name?.toLowerCase().includes(search.toLowerCase()) ||
    f.fraCategoryName?.toLowerCase().includes(search.toLowerCase())
  );

  return (
    <>
      <div className="admin-topbar">
        <h1>Fundraising activities</h1>
        <input
          className="admin-search"
          placeholder="Search activities..."
          value={search}
          onChange={e => setSearch(e.target.value)}
        />
      </div>

      <div className="admin-metrics">
        <div className="metric">
          <div className="metric-label">Total activities</div>
          <div className="metric-val">{filtered.length}</div>
        </div>
        <div className="metric">
          <div className="metric-label">Active</div>
          <div className="metric-val">{filtered.filter(f => !f.status).length}</div>
        </div>
        <div className="metric">
          <div className="metric-label">Completed</div>
          <div className="metric-val">{filtered.filter(f => f.status).length}</div>
        </div>
      </div>

      <div className="admin-table-card">
        <table>
          <thead>
            <tr>
              <th>Name</th>
              <th>Category</th>
              <th>Goal ($)</th>
              <th>Donated ($)</th>
              <th>Deadline</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            {filtered.length === 0 && (
              <tr><td colSpan={6} style={{ textAlign: "center", color: "#7a7d8a", padding: "2rem" }}>No activities found</td></tr>
            )}
            {filtered.map(f => (
              <tr key={f.id} onClick={() => onSelect(f)} style={{ cursor: "pointer" }}>
                <td>{f.name}</td>
                <td>{f.fraCategoryName}</td>
                <td>{f.amtRequested?.toLocaleString()}</td>
                <td>{f.amtDonated?.toLocaleString()}</td>
                <td>{f.deadline}</td>
                <td>
                  <span className={`badge ${!f.status ? "badge-active" : "badge-completed"}`}>
                    {f.status ? "Completed" : "Active"}
                  </span>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </>
  );
}