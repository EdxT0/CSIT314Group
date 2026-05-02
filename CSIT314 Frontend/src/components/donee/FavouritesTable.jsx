export default function FavouritesTable({ favourites, search, setSearch, onSelect, onUnfavourite }) {
  const filtered = favourites.filter(f =>
    f.name?.toLowerCase().includes(search.toLowerCase()) ||
    f.fraCategoryName?.toLowerCase().includes(search.toLowerCase())
  );

  return (
    <>
      <div className="admin-topbar">
        <h1>My favourites</h1>
        <input
          className="admin-search"
          placeholder="Search favourites..."
          value={search}
          onChange={e => setSearch(e.target.value)}
        />
      </div>

      <div className="admin-metrics">
        <div className="metric">
          <div className="metric-label">Total favourites</div>
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
              <th>Deadline</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {filtered.length === 0 && (
              <tr><td colSpan={6} style={{ textAlign: "center", color: "#7a7d8a", padding: "2rem" }}>No favourites yet</td></tr>
            )}
            {filtered.map(f => (
              <tr key={f.id}>
                <td style={{ cursor: "pointer" }} onClick={() => onSelect(f)}>{f.name}</td>
                <td>{f.fraCategoryName}</td>
                <td>{f.amtRequested?.toLocaleString()}</td>
                <td>{f.deadline}</td>
                <td>
                  <span className={`badge ${!f.status ? "badge-active" : "badge-completed"}`}>
                    {f.status ? "Completed" : "Active"}
                  </span>
                </td>
                <td>
                  <button className="action-btn danger" onClick={() => onUnfavourite(f.id)}>
                    Remove
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