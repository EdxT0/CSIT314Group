import { useState } from "react";
import FRADetailPopup from "./FRADetailPopup";
import { formatDeadline } from "../../utils/formatDeadline";

export default function DoneeFRATable({ fras, search, setSearch, favouriteIds = [], onSearch, onReset, onSuccess }) {
  const [selectedFRA, setSelectedFRA] = useState(null);

  function capitaliseNames(str) {
    if (!str) return str;
    return str.split(" ").map(n => n.charAt(0).toUpperCase() + n.slice(1).toLowerCase()).join(" ");
  }

  const handleSelectFRA = async (f) => {
    const res = await fetch(`/api/ViewOneFundraiser?fraId=${f.id}`, {
      credentials: "include",
    });
    if (res.ok) {
      const data = await res.json();
      setSelectedFRA(data);
    } else {
      setSelectedFRA(f);
    }
  };



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
            onKeyDown={e => e.key === "Enter" && onSearch()}
          />
          <button className="admin-btn" onClick={onSearch}>Search</button>
          <button className="admin-btn" onClick={onReset}>Reset</button>
        </div>
      </div>

      <div className="admin-metrics">
        <div className="metric"><div className="metric-label">Total activities</div><div className="metric-val">{fras.length}</div></div>
        <div className="metric"><div className="metric-label">Active</div><div className="metric-val">{fras.filter(f => !f.status).length}</div></div>
        <div className="metric"><div className="metric-label">Completed</div><div className="metric-val">{fras.filter(f => f.status).length}</div></div>
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
            {fras.length === 0 && (
              <tr><td colSpan={6} style={{ textAlign: "center", color: "#7a7d8a", padding: "2rem" }}>No activities found</td></tr>
            )}
            {fras.map(f => (
              <tr key={f.id} onClick={() => handleSelectFRA(f)} style={{ cursor: "pointer" }}>
                <td>{capitaliseNames(f.name)}</td>
                <td>{capitaliseNames(f.fraCategoryName)}</td>
                <td>{f.amtRequested?.toLocaleString()}</td>
                <td>{f.amtDonated?.toLocaleString()}</td>
                <td>{f.deadlineInString}</td>
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

      {selectedFRA && (
        <FRADetailPopup
          fra={selectedFRA}
          onClose={() => setSelectedFRA(null)}
          isFavourited={favouriteIds.includes(selectedFRA.id)}
          onSuccess={onSuccess}
        />
      )}
    </>
  );
}