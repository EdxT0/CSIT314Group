import { useState } from "react";
import FRAPopup from "./FRAPopup";

export default function FRATable({ fras, search, setSearch, onSuccess, onSearch, onReset }) {
  const [selectedFRA, setSelectedFRA] = useState(null);

  function capitaliseNames(str) {
    if (!str) return str;
    return str
      .split(" ")
      .map(name => name.charAt(0).toUpperCase() + name.slice(1).toLowerCase())
      .join(" ");
  }

  const active = fras.filter(f => f.status === false || f.status === 0);
  const completed = fras.filter(f => f.status === true || f.status === 1);
  const totalViews = fras.reduce((sum, f) => sum + (f.amtOfViews ?? 0), 0);

  return (
    <>
      <div className="admin-topbar">
        <h1>Browse all activities</h1>
        <div style={{ display: "flex", gap: "8px" }}>
          <input
            className="admin-search"
            placeholder="Search activities..."
            value={search}
            onChange={e => setSearch(e.target.value)}
            onKeyDown={e => e.key === "Enter" && onSearch()}
          />
          <button className="admin-btn" onClick={onSearch}>Search</button>
          <button className="admin-btn" onClick={onReset}>Reset</button>
        </div>
      </div>

      <div className="admin-metrics">
        <div className="metric">
          <div className="metric-label">Total</div>
          <div className="metric-val">{fras.length}</div>
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
            {fras.length === 0 && (
              <tr><td colSpan={6} style={{ textAlign: "center", color: "#7a7d8a" }}>No activities found</td></tr>
            )}
            {fras.map(f => (
              <tr
                key={f.id}
                onClick={() => setSelectedFRA(f)}
                style={{ cursor: "pointer" }}
              >
                <td>{capitaliseNames(f.name)}</td>
                <td>{capitaliseNames(f.fraCategoryName)}</td>
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

      {/* Read-only popup — no edit or delete */}
      {selectedFRA && (
        <FRAPopup
          fra={selectedFRA}
          onClose={() => setSelectedFRA(null)}
          readOnly={true}
          onSuccess={async () => {
            await onSuccess?.();
            setSelectedFRA(null);
          }}
        />
      )}
    </>
  );
}