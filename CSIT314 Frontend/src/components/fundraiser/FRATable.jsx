export default function FRATable({ fras, search, setSearch, onSelect }) {
  const filtered = fras.filter(f =>
    f.name?.toLowerCase().includes(search.toLowerCase()) ||
    f.fraCategoryName?.toLowerCase().includes(search.toLowerCase())
  );

  const active = filtered.filter(f => f.status === false || f.status === 0);
  const completed = filtered.filter(f => f.status === true || f.status === 1);
  const totalViews = filtered.reduce((sum, f) => sum + (f.amtOfViews ?? 0), 0);

  return (
    <>
      <div className="admin-topbar">
        <h1>My fundraising activities</h1>
        <input
          className="admin-search"
          placeholder="Search activities..."
          value={search}
          onChange={e => setSearch(e.target.value)}
        />
      </div>

      <div className="admin-metrics">
        <div className="metric">
          <div className="metric-label">Total</div>
          <div className="metric-val">{filtered.length}</div>
        </div>
        <div className="metric">
          <div className="metric-label">Active</div>
          <div className="metric-val">{active.length}</div>
        </div>
        <div className="metric">
          <div className="metric-label">Completed</div>
          <div className="metric-val">{completed.length}</div>
        </div>
        <div className="metric">
          <div className="metric-label">Total views</div>
          <div className="metric-val">{totalViews}</div>
        </div>
      </div>

      <div className="admin-table-card">
        <table>
          <thead>
            <tr>
              <th>Name</th>
              <th>Category</th>
              <th>Goal ($)</th>
              <th>Deadline</th>
              <th>Views</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            {filtered.length === 0 && (
              <tr><td colSpan={6} style={{ textAlign: "center", color: "#7a7d8a" }}>No activities found</td></tr>
            )}
            {filtered.map(f => (
              <tr key={f.id} onClick={() => onSelect(f)} style={{ cursor: "pointer" }}>
                <td>{f.name}</td>
                <td>{f.fraCategoryName}</td>
                <td>{f.amtRequested?.toLocaleString()}</td>
                <td>{f.deadline}</td>
                <td>{f.amtOfViews}</td>
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