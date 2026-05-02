export default function CategoryTable({ categories, search, setSearch, onEdit, onDelete }) {
  const filtered = categories.filter(c =>
    c.name?.toLowerCase().includes(search.toLowerCase()) ||
    c.description?.toLowerCase().includes(search.toLowerCase())
  );

  return (
    <>
      <div className="admin-topbar">
        <h1>Fundraising categories</h1>
        <input
          className="admin-search"
          placeholder="Search categories..."
          value={search}
          onChange={e => setSearch(e.target.value)}
        />
      </div>

      <div className="admin-metrics">
        <div className="metric">
          <div className="metric-label">Total categories</div>
          <div className="metric-val">{filtered.length}</div>
        </div>
      </div>

      <div className="admin-table-card">
        <table>
          <thead>
            <tr>
              <th>Name</th>
              <th>Description</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {filtered.length === 0 && (
              <tr>
                <td colSpan={3} style={{ textAlign: "center", color: "#7a7d8a", padding: "2rem" }}>
                  No categories found
                </td>
              </tr>
            )}
            {filtered.map(c => (
              <tr key={c.id}>
                <td>{c.name}</td>
                <td>{c.description}</td>
                <td>
                  <button className="action-btn" onClick={() => onEdit(c)}>Edit</button>
                  <button className="action-btn danger" onClick={() => onDelete(c.id)}>Delete</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </>
  );
}