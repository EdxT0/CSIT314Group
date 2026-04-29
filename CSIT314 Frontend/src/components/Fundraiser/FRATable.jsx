import "../../styles/adminpage.css";

// Receives: fras[], search, setSearch, onView, onEdit, onDelete, loading
export default function FRATable({ fras, search, setSearch, onView, onEdit, onDelete, loading }) {

  function statusBadge(status) {
    const map = {
      0: { label: "Active",    cls: "badge-active" },
      1: { label: "Completed", cls: "badge-completed" },
      2: { label: "Paused",    cls: "badge-paused" },
    };
    // accept int or string
    const entry = map[status] ?? map[status?.toString()] ?? { label: status ?? "Unknown", cls: "badge-draft" };
    return <span className={`badge ${entry.cls}`}>{entry.label}</span>;
  }

  return (
    <>
      <div className="admin-topbar">
        <h1>My Fundraising Activities</h1>
      </div>

      <div style={{ display: "flex", gap: "10px", marginBottom: "1rem" }}>
        <input
          className="admin-search"
          type="text"
          placeholder="Search by name…"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />
      </div>

      <div className="admin-table-card">
        <table>
          <thead>
            <tr>
              <th style={{ width: "28%" }}>Name</th>
              <th style={{ width: "16%" }}>Category</th>
              <th style={{ width: "10%" }}>Views</th>
              <th style={{ width: "10%" }}>Shortlists</th>
              <th style={{ width: "10%" }}>Status</th>
              <th style={{ width: "12%" }}>Deadline</th>
              <th style={{ width: "14%" }}>Actions</th>
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr>
                <td colSpan={7} style={{ textAlign: "center", color: "#555870", padding: "2rem" }}>
                  Loading…
                </td>
              </tr>
            ) : fras.length === 0 ? (
              <tr>
                <td colSpan={7} style={{ textAlign: "center", color: "#555870", padding: "2rem" }}>
                  No fundraising activities found.
                </td>
              </tr>
            ) : (
              fras.map((fra) => (
                <tr key={fra.id}>
                  <td title={fra.name}>{fra.name}</td>
                  <td>{fra.categoryName ?? fra.fraCategoryId}</td>
                  <td>{fra.views ?? 0}</td>
                  <td>{fra.shortlists ?? 0}</td>
                  <td>{statusBadge(fra.status)}</td>
                  <td>{fra.deadline ? new Date(fra.deadline).toLocaleDateString("en-GB") : "—"}</td>
                  <td>
                    <button className="action-btn" onClick={() => onView(fra)}>View</button>
                    <button className="action-btn" onClick={() => onEdit(fra)}>Edit</button>
                    <button className="action-btn danger" onClick={() => onDelete(fra)}>Delete</button>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
    </>
  );
}
