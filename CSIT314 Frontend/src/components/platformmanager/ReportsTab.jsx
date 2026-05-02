import { useState } from "react";

export default function ReportsTab({ fras }) {
  const [period, setPeriod] = useState("daily");

  const now = new Date();

  const getStartDate = () => {
    const d = new Date(now);
    if (period === "daily") d.setDate(d.getDate() - 1);
    if (period === "weekly") d.setDate(d.getDate() - 7);
    if (period === "monthly") d.setMonth(d.getMonth() - 1);
    return d;
  };

  const parseDeadline = (str) => {
    if (!str) return null;
    // handles dd-MM-yyyy or ISO format
    if (str.includes("-") && str.length === 10 && str[2] === "-") {
      const [d, m, y] = str.split("-");
      return new Date(`${y}-${m}-${d}`);
    }
    return new Date(str);
  };

  const startDate = getStartDate();

  const filteredFRAs = fras.filter(f => {
    const deadline = parseDeadline(f.deadline);
    return deadline && deadline >= startDate && deadline <= now;
  });

  const totalGoal = filteredFRAs.reduce((sum, f) => sum + (f.amtRequested ?? 0), 0);
  const totalDonated = filteredFRAs.reduce((sum, f) => sum + (f.amtDonated ?? 0), 0);
  const totalViews = filteredFRAs.reduce((sum, f) => sum + (f.amtOfViews ?? 0), 0);
  const completed = filteredFRAs.filter(f => f.status).length;
  const active = filteredFRAs.filter(f => !f.status).length;

  const periodLabel = { daily: "Daily", weekly: "Weekly", monthly: "Monthly" }[period];

  return (
    <>
      <div className="admin-topbar">
        <h1>{periodLabel} report</h1>
        <div style={{ display: "flex", gap: "8px" }}>
          {["daily", "weekly", "monthly"].map(p => (
            <button
              key={p}
              className={`admin-btn ${period === p ? "admin-btn-primary" : ""}`}
              onClick={() => setPeriod(p)}
            >
              {p.charAt(0).toUpperCase() + p.slice(1)}
            </button>
          ))}
        </div>
      </div>

      <div style={{ fontSize: "13px", color: "#7a7d8a", marginBottom: "1.25rem" }}>
        Showing activities with deadlines from {startDate.toLocaleDateString()} to {now.toLocaleDateString()}
      </div>

      <div className="admin-metrics" style={{ gridTemplateColumns: "repeat(5, minmax(0, 1fr))" }}>
        <div className="metric">
          <div className="metric-label">Activities</div>
          <div className="metric-val">{filteredFRAs.length}</div>
        </div>
        <div className="metric">
          <div className="metric-label">Active</div>
          <div className="metric-val">{active}</div>
        </div>
        <div className="metric">
          <div className="metric-label">Completed</div>
          <div className="metric-val">{completed}</div>
        </div>
        <div className="metric">
          <div className="metric-label">Total goal ($)</div>
          <div className="metric-val">{totalGoal.toLocaleString()}</div>
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
              <th>Name</th>
              <th>Category</th>
              <th>Goal ($)</th>
              <th>Donated ($)</th>
              <th>Views</th>
              <th>Deadline</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            {filteredFRAs.length === 0 && (
              <tr>
                <td colSpan={7} style={{ textAlign: "center", color: "#7a7d8a", padding: "2rem" }}>
                  No activities in this period
                </td>
              </tr>
            )}
            {filteredFRAs.map(f => (
              <tr key={f.id}>
                <td>{f.name}</td>
                <td>{f.fraCategoryName}</td>
                <td>{f.amtRequested?.toLocaleString()}</td>
                <td>{f.amtDonated?.toLocaleString()}</td>
                <td>{f.amtOfViews}</td>
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