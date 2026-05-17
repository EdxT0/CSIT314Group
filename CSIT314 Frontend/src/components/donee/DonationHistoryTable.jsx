import { useState } from "react";
import FRADetailPopup from "./FRADetailPopup";
import { formatDeadline } from "../../utils/formatDeadline";

export default function DonationHistoryTable({ donations, search, setSearch, favouriteIds = [], onSearch, onReset, onSuccess }) {
  const [selectedFRA, setSelectedFRA] = useState(null);

  const totalDonated = donations.reduce((sum, d) => sum + (d.userDonatedAmt ?? 0), 0);

const handleSelectFRA = (d) => {
  setSelectedFRA(d);  
};

  return (
    <>
      <div className="admin-topbar">
        <h1>Donation history</h1>
        <div style={{ display: "flex", gap: "8px" }}>
          <input
            className="admin-search"
            placeholder="Search by activity name..."
            value={search}
            onChange={e => setSearch(e.target.value)}
            onKeyDown={e => e.key === "Enter" && onSearch()}
          />
          <button className="admin-btn" onClick={onSearch}>Search</button>
          <button className="admin-btn" onClick={onReset}>Reset</button>
        </div>
      </div>

      <div className="admin-metrics">
        <div className="metric"><div className="metric-label">Total donations</div><div className="metric-val">{donations.length}</div></div>
        <div className="metric"><div className="metric-label">Total donated ($)</div><div className="metric-val">{totalDonated.toLocaleString()}</div></div>
      </div>

      <div className="admin-table-card">
        <table>
          <thead>
            <tr>
              <th>Activity name</th><th>Category</th>
              <th>Your donation ($)</th><th>Date donated</th><th>FRA status</th>
            </tr>
          </thead>
          <tbody>
            {donations.length === 0 && (
              <tr><td colSpan={5} style={{ textAlign: "center", color: "#7a7d8a", padding: "2rem" }}>No donations yet</td></tr>
            )}
            {donations.map((d, i) => (
              <tr key={i} onClick={() => handleSelectFRA(d)} style={{ cursor: "pointer" }}>
                <td>{d.name}</td>
                <td>{d.fraCategoryName}</td>
                <td>${d.userDonatedAmt?.toLocaleString()}</td>
                <td>{formatDeadline(d.dateDonated)}</td>
                <td>
                  <span className={`badge ${!d.status ? "badge-active" : "badge-completed"}`}>
                    {d.status ? "Completed" : "Active"}
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