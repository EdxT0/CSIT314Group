import { useState } from "react";
import FRADetailPopup from "./FRADetailPopup";
import { formatDeadline } from "../../utils/formatDeadline";

export default function FavouritesTable({ favourites, search, setSearch, onUnfavourite, favouriteIds = [], onSearch, onReset, onSuccess }) {
  const [selectedFRA, setSelectedFRA] = useState(null);

  return (
    <>
      <div className="admin-topbar">
        <h1>My favourites</h1>
        <div style={{ display: "flex", gap: "8px" }}>
          <input
            className="admin-search"
            placeholder="Search favourites..."
            value={search}
            onChange={e => setSearch(e.target.value)}
            onKeyDown={e => e.key === "Enter" && onSearch()}
          />
          <button className="admin-btn" onClick={onSearch}>Search</button>
          <button className="admin-btn" onClick={onReset}>Reset</button>
        </div>
      </div>

      <div className="admin-metrics">
        <div className="metric"><div className="metric-label">Total favourites</div><div className="metric-val">{favourites.length}</div></div>
        <div className="metric"><div className="metric-label">Active</div><div className="metric-val">{favourites.filter(f => !f.status).length}</div></div>
        <div className="metric"><div className="metric-label">Completed</div><div className="metric-val">{favourites.filter(f => f.status).length}</div></div>
      </div>

      <div className="admin-table-card">
        <table>
          <thead>
            <tr>
              <th>Name</th><th>Category</th><th>Goal ($)</th>
              <th>Deadline</th><th>Status</th><th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {favourites.length === 0 && (
              <tr><td colSpan={6} style={{ textAlign: "center", color: "#7a7d8a", padding: "2rem" }}>No favourites yet</td></tr>
            )}
            {favourites.map(f => (
              <tr key={f.id} onClick={() => setSelectedFRA(f)} style={{ cursor: "pointer" }}>
                <td>{f.name}</td>
                <td>{f.fraCategoryName}</td>
                <td>{f.amtRequested?.toLocaleString()}</td>
                <td>{f.deadlineInString}</td>
                <td>
                  <span className={`badge ${!f.status ? "badge-active" : "badge-completed"}`}>
                    {f.status ? "Completed" : "Active"}
                  </span>
                </td>
                <td>
                  <button
                    className="action-btn danger"
                    onClick={e => { e.stopPropagation(); onUnfavourite(f.id); }}>
                    Remove
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {selectedFRA && (
        <FRADetailPopup
          fra={selectedFRA}
          onClose={() => setSelectedFRA(null)}
          isFavourited={true}
          onSuccess={onSuccess}
        />
      )}
    </>
  );
}