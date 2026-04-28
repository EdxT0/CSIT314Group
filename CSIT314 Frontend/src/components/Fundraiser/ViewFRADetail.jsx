import { useState, useEffect } from "react";
import { fraApi } from "../fraApi";
import "../../styles/adminpage.css";

// Receives: fra (the FRA object), onBack(), onEdit(fra)
export default function ViewFRADetail({ fra, onBack, onEdit }) {
  const [detail, setDetail] = useState(null);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fraApi.getOne(fra.id)
      .then(async (res) => {
        if (!res.ok) { setError("Failed to load details."); return; }
        setDetail(await res.json());
      })
      .catch(() => setError("Network error."))
      .finally(() => setLoading(false));
  }, [fra.id]);

  const d = detail ?? fra; // fall back to list data while loading

  const statusLabels = { 0: "Active", 1: "Completed", 2: "Paused" };
  const statusClasses = { 0: "badge-active", 1: "badge-completed", 2: "badge-paused" };

  return (
    <>
      <div className="admin-topbar">
        <h1>Fundraising Activity Detail</h1>
      </div>

      {error && <div className="form-error">{error}</div>}

      <div className="admin-form-card" style={{ maxWidth: "560px" }}>
        {loading ? (
          <p style={{ color: "#9a9daa", fontSize: "13px" }}>Loading…</p>
        ) : (
          <>
            <h2 style={{ marginBottom: "1.5rem" }}>{d.name}</h2>

            {/* Interest metrics */}
            <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: "10px", marginBottom: "1.5rem" }}>
              <div style={metricBox}>
                <div style={metricVal}>👁 {d.views ?? 0}</div>
                <div style={metricLabel}>Total Views</div>
              </div>
              <div style={metricBox}>
                <div style={metricVal}>★ {d.shortlists ?? 0}</div>
                <div style={metricLabel}>Shortlists</div>
              </div>
            </div>

            {/* Details */}
            <div style={{ borderTop: "0.5px solid #2e3240", paddingTop: "1rem" }}>
              <Row label="Status">
                <span className={`badge ${statusClasses[d.status] ?? "badge-draft"}`}>
                  {statusLabels[d.status] ?? d.status}
                </span>
              </Row>
              <Row label="Category">{d.categoryName ?? d.fraCategoryId}</Row>
              <Row label="Description">{d.description || "—"}</Row>
              <Row label="Target Amount">SGD {Number(d.amtRequested ?? 0).toLocaleString()}</Row>
              <Row label="Deadline">
                {d.deadline ? new Date(d.deadline).toLocaleDateString("en-GB") : "—"}
              </Row>
            </div>

            <div style={{ display: "flex", gap: "10px", marginTop: "1.5rem" }}>
              <button
                className="submit-btn"
                onClick={() => onEdit(d)}
                style={{ flex: 1 }}
              >
                Edit
              </button>
              <button
                className="admin-btn"
                onClick={onBack}
                style={{ flex: 1, height: "40px", fontSize: "14px" }}
              >
                Back
              </button>
            </div>
          </>
        )}
      </div>
    </>
  );
}

function Row({ label, children }) {
  return (
    <div style={{ display: "flex", justifyContent: "space-between", padding: "7px 0", borderBottom: "0.5px solid #2e3240", fontSize: "13px" }}>
      <span style={{ color: "#9a9daa" }}>{label}</span>
      <span style={{ color: "#e8e6e1", fontWeight: 500, textAlign: "right", maxWidth: "60%" }}>{children}</span>
    </div>
  );
}

const metricBox = {
  background: "#13151c",
  border: "0.5px solid #2e3240",
  borderRadius: "8px",
  padding: "14px 16px",
};
const metricVal = { fontSize: "22px", fontWeight: 500, color: "#e8e6e1", marginBottom: "4px" };
const metricLabel = { fontSize: "12px", color: "#9a9daa" };
