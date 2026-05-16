import { useState } from "react";
import FRAPopup from "./FRAPopup";

export default function MyFRATable({ fras, search, setSearch, onSuccess, onDelete, onSearch, onReset, favouriteCounts = {} }) {
  const [selectedFRA, setSelectedFRA] = useState(null);
  const [successMessage, setSuccessMessage] = useState("");  // ← fix: was displaySuccess

  function capitaliseNames(str) {
    if (!str) return str;
    return str.split(" ").map(name => name.charAt(0).toUpperCase() + name.slice(1).toLowerCase()).join(" ");
  }

  const handleSelectFRA = async (f) => {
    // ← connect to ViewOneFundraiser to get updated views
    const res = await fetch(`/api/ViewOneFundraiser?fraId=${f.id}`, {
      credentials: "include",
    });
    if (res.ok) {
      const data = await res.json();
      setSelectedFRA(data);
    } else {
      setSelectedFRA(f);  // fallback
    }
  };

  const active = fras.filter(f => f.status === false || f.status === 0);
  const completed = fras.filter(f => f.status === true || f.status === 1);
  const totalViews = fras.reduce((sum, f) => sum + (f.amtOfViews ?? 0), 0);

  return (
    <>
      <div className="admin-topbar">
        <h1>My fundraising activities</h1>
        <div style={{ display: "flex", gap: "8px" }}>
          <input
            className="admin-search"
            placeholder="Search my activities..."
            value={search}
            onChange={e => setSearch(e.target.value)}
            onKeyDown={e => e.key === "Enter" && onSearch()}
          />
          <button className="admin-btn" onClick={onSearch}>Search</button>
          <button className="admin-btn" onClick={onReset}>Reset</button>
        </div>
      </div>

      {successMessage && (
        <div className="form-success">{successMessage}</div>
      )}

      <div className="admin-metrics">
        <div className="metric"><div className="metric-label">Total</div><div className="metric-val">{fras.length}</div></div>
        <div className="metric"><div className="metric-label">Active</div><div className="metric-val">{active.length}</div></div>
        <div className="metric"><div className="metric-label">Completed</div><div className="metric-val">{completed.length}</div></div>
        <div className="metric"><div className="metric-label">Total views</div><div className="metric-val">{totalViews}</div></div>
      </div>

      <div className="admin-table-card fra-table-card">
        <table>
          <thead>
            <tr>
              <th>Name</th>
              <th>Category</th>
              <th>Goal ($)</th>
              <th>Deadline</th>
              <th>Views</th>
              <th>Favourited</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {fras.length === 0 && (
              <tr><td colSpan={7} style={{ textAlign: "center", color: "#7a7d8a" }}>No activities found</td></tr>
            )}
            {fras.map(f => (
              <tr key={f.id} onClick={() => handleSelectFRA(f)} style={{ cursor: "pointer" }}>
                <td>{capitaliseNames(f.name)}</td>
                <td>{capitaliseNames(f.fraCategoryName)}</td>
                <td>{f.amtRequested?.toLocaleString()}</td>
                <td>{f.deadlineInString}</td>
                <td>{f.amtOfViews}</td>
                <td>{favouriteCounts[f.id] ?? "—"}</td>
                <td>
                  <span className={`badge ${!f.status ? "badge-active" : "badge-completed"}`}>
                    {f.status ? "Completed" : "Active"}
                  </span>
                </td>
                <td>
                  <button
                    className="action-btn"
                    onClick={e => {
                      e.stopPropagation();
                      setSelectedFRA({ ...f, startEditing: true });
                    }}>
                    Edit
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {selectedFRA && (
        <FRAPopup
          fra={selectedFRA}
          onClose={async () => {
            setSelectedFRA(null);
          }}
          onDelete={() => {
            setSelectedFRA(null);
            onSuccess?.();
            setSuccessMessage("Activity deleted successfully!");        
            setTimeout(() => setSuccessMessage(""), 3000);             
          }}
          onSuccess={async (message) => {
            await onSuccess?.();
            setSelectedFRA(null);
            if (message) {
              setSuccessMessage(message);
              setTimeout(() => setSuccessMessage(""), 3000);
            }
          }}
        />
      )}
    </>
  );
}