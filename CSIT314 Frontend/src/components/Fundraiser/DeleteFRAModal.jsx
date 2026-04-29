import "../../styles/adminpage.css";

// Receives: fra (the FRA to delete), onConfirm(), onCancel()
export default function DeleteFRAModal({ fra, onConfirm, onCancel }) {
  return (
    <div style={overlay}>
      <div style={modal}>
        <h2 style={{ fontSize: "15px", fontWeight: 500, color: "#f09595", marginBottom: "0.75rem" }}>
          Delete Activity?
        </h2>
        <p style={{ fontSize: "13px", color: "#9a9daa", marginBottom: "1.5rem", lineHeight: 1.6 }}>
          You are about to permanently delete{" "}
          <strong style={{ color: "#e8e6e1" }}>{fra.name}</strong>.
          This action cannot be undone.
        </p>
        <div style={{ display: "flex", gap: "10px", justifyContent: "flex-end" }}>
          <button className="admin-btn" onClick={onCancel}>Cancel</button>
          <button
            className="admin-btn"
            onClick={onConfirm}
            style={{ background: "#7a2020", borderColor: "#7a2020", color: "#f09595" }}
          >
            Delete
          </button>
        </div>
      </div>
    </div>
  );
}

const overlay = {
  position: "fixed",
  inset: 0,
  background: "rgba(0,0,0,0.6)",
  display: "flex",
  alignItems: "center",
  justifyContent: "center",
  zIndex: 100,
};

const modal = {
  background: "#1c1f26",
  border: "0.5px solid #7a2020",
  borderRadius: "12px",
  padding: "1.5rem",
  width: "360px",
  maxWidth: "95vw",
};
