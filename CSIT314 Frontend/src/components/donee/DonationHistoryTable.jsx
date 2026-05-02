export default function DonationHistoryTable({ donations, search, setSearch }) {
  const filtered = donations.filter(d =>
    d.name?.toLowerCase().includes(search.toLowerCase()) ||
    d.fraCategoryName?.toLowerCase().includes(search.toLowerCase())
  );

  const totalDonated = filtered.reduce((sum, d) => sum + (d.userDonatedAmt ?? 0), 0);

  return (
    <>
      <div className="admin-topbar">
        <h1>Donation history</h1>
        <input
          className="admin-search"
          placeholder="Search by activity name..."
          value={search}
          onChange={e => setSearch(e.target.value)}
        />
      </div>

      <div className="admin-metrics">
        <div className="metric">
          <div className="metric-label">Total donations</div>
          <div className="metric-val">{filtered.length}</div>
        </div>
        <div className="metric">
          <div className="metric-label">Total donated ($)</div>
          <div className="metric-val">{totalDonated.toLocaleString()}</div>
        </div>
      </div>

      <div className="admin-table-card">
        <table>
          <thead>
            <tr>
              <th>Activity name</th>
              <th>Category</th>
              <th>Your donation ($)</th>
              <th>Date donated</th>
              <th>FRA status</th>
            </tr>
          </thead>
          <tbody>
            {filtered.length === 0 && (
              <tr><td colSpan={5} style={{ textAlign: "center", color: "#7a7d8a", padding: "2rem" }}>No donations yet</td></tr>
            )}
            {filtered.map((d, i) => (
              <tr key={i}>
                <td>{d.name}</td>
                <td>{d.fraCategoryName}</td>
                <td>${d.userDonatedAmt?.toLocaleString()}</td>
                <td>{d.dateDonated}</td>
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
    </>
  );
}