import { useState } from "react";
import ProfilePopup from "./ProfilePopup";

export default function ProfilesTable({ profiles, search, setSearch, onSuspend, onSearch, onReset, onSuccess, successMessage }) {
  const [selectedProfile, setSelectedProfile] = useState(null);

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
        <h1>User profiles</h1>
        <div style={{ display: "flex", gap: "8px" }}>
          <input
            className="admin-search"
            placeholder="Search profiles..."
            value={search}
            onChange={e => setSearch(e.target.value)}
            onKeyDown={e => e.key === "Enter" && onSearch()}
          />
          <button className="admin-btn" onClick={onSearch}>Search</button>
          <button className="admin-btn" onClick={onReset}>Reset</button>
        </div>
      </div>

      {success && <div className="form-success">{success}</div>}  


      <div className="admin-metrics">
        <div className="metric">
          <div className="metric-label">Total profiles</div>
          <div className="metric-val">{profiles.length}</div>
        </div>
        <div className="metric">
          <div className="metric-label">Active</div>
          <div className="metric-val">{profiles.filter(p => p.status == 0).length}</div>
        </div>
        <div className="metric">
          <div className="metric-label">Suspended</div>
          <div className="metric-val">{profiles.filter(p => p.status == 1).length}</div>
        </div>
      </div>

      <div className="admin-table-card profile-table-card">
        <table>
          <thead>
            <tr>
              <th>Profile name</th>
              <th>Description</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {profiles.length === 0 && (
              <tr><td colSpan={4} style={{ textAlign: "center", color: "#7a7d8a", padding: "2rem" }}>No profiles found</td></tr>
            )}
            {profiles.map((p, index) => (
              <tr
                key={`${p.id}-${index}`}
                onClick={() => setSelectedProfile(p)}
                style={{ cursor: "pointer" }}
              >
                <td>{capitaliseNames(p.profileName)}</td>
                <td>{p.description}</td>
                <td>
                  <span className={`badge ${p.status == 0 ? "badge-active" : "badge-suspended"}`}>
                    {p.status == 0 ? "Active" : "Suspended"}
                  </span>
                </td>
                <td>
                  <button className="action-btn"
                    onClick={e => {
                      e.stopPropagation();
                      setSelectedProfile({ ...p, startEditing: true });
                    }}>
                    Edit
                  </button>
                  <button
                    className={`action-btn ${p.status == 1 ? "danger" : ""}`}
                    onClick={e => {
                      e.stopPropagation();
                      onSuspend(p.id, p.status == 0);
                    }}>
                    {p.status == 0 ? "Suspend" : "Unsuspend"}
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {selectedProfile && (
        <ProfilePopup
          profile={selectedProfile}
          onClose={() => setSelectedProfile(null)}
          onSuspend={onSuspend}
          onSuccess={async (message) => {
            await onSuccess?.(message); 
          }}
        />
      )}
    </>
  );
}