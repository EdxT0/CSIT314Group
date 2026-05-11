import { useState } from "react";

export default function DoneeFRATable({ fras, search, setSearch, onSelect}) {
  const [activeSearch, setActiveSearch] = useState("");
  const [successMessage, setSuccessMessage] = useState("");

  const filtered = fras.filter(f =>
    f.name?.toLowerCase().includes(activeSearch.toLowerCase()) ||
    f.fraCategoryName?.toLowerCase().includes(activeSearch.toLowerCase())
  );

  const handleSearch = () => {
    setActiveSearch(search);
  };

    const handleReset = () => {
    setSearch("");
    setActiveSearch("");
  };

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
        <h1>Fundraising activities</h1>
          <div style={{ display: "flex", gap: "8px" }}>
            <input
              className="admin-search"
              placeholder="Search activities..."
              value={search}
              onChange={e => setSearch(e.target.value)}
              onKeyDown={e => e.key === "Enter" && handleSearch()}
              />
            <button className="admin-btn" onClick={handleSearch}>Search</button>
            <button className="admin-btn" onClick={handleReset}>Reset</button>
          </div>
      </div>

      {successMessage && (
        <div style={{
          background: "#0f2e1a",
          border: "0.5px solid #1d9e75",
          borderRadius: "8px",
          padding: "10px 12px",
          fontSize: "13px",
          color: "#5dcaa5",
          marginBottom: "1rem"
        }}>
          {successMessage}
        </div>
      )}
      
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
                <td>{capitaliseNames(f.name)}</td>
                <td>{capitaliseNames(f.fraCategoryName)}</td>
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