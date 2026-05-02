import { useState } from "react";

export default function FRADetailPopup({ fra, onClose, onFavourited }) {
  const [donationAmt, setDonationAmt] = useState("");
  const [message, setMessage] = useState("");
  const [error, setError] = useState("");

  const handleFavourite = async () => {
    setError(""); setMessage("");
    const res = await fetch("/api/FavouriteFundraiser", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify({ fraId: fra.id }),
    });
    const text = await res.text();
    if (!res.ok) { setError(text); return; }
    setMessage("Added to favourites!");
    onFavourited?.();
  };

  const handleDonate = async () => {
    setError(""); setMessage("");
    if (!donationAmt || parseFloat(donationAmt) <= 0) {
      setError("Please enter a valid donation amount");
      return;
    }
    const res = await fetch("/api/AddDonation", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify({ fraId: fra.id, amtDonatedByUser: parseFloat(donationAmt) }),
    });
    const text = await res.text();
    if (!res.ok) { setError(text); return; }
    setMessage("Donation successful!");
    setDonationAmt("");
  };

  const progress = fra.amtRequested > 0
    ? Math.min((fra.amtDonated / fra.amtRequested) * 100, 100).toFixed(1)
    : 0;

  return (
    <div className="popup-overlay" onClick={onClose}>
      <div className="popup-card" style={{ maxWidth: "440px" }} onClick={e => e.stopPropagation()}>
        <div className="popup-header">
          <h2>{fra.name}</h2>
          <button className="popup-close" onClick={onClose}>✕</button>
        </div>

        {message && <div style={{ background: "#0f2e1a", border: "0.5px solid #1d9e75", borderRadius: "8px", padding: "8px 12px", fontSize: "13px", color: "#5dcaa5", marginBottom: "1rem" }}>{message}</div>}
        {error && <div className="form-error">{error}</div>}

        <div className="popup-row">
          <span className="popup-label">Category</span>
          <span className="popup-val">{fra.fraCategoryName}</span>
        </div>
        <div className="popup-row">
          <span className="popup-label">Description</span>
          <span className="popup-val">{fra.description}</span>
        </div>
        <div className="popup-row">
          <span className="popup-label">Goal</span>
          <span className="popup-val">${fra.amtRequested?.toLocaleString()}</span>
        </div>
        <div className="popup-row">
          <span className="popup-label">Donated</span>
          <span className="popup-val">${fra.amtDonated?.toLocaleString()} ({progress}%)</span>
        </div>
        <div className="popup-row">
          <span className="popup-label">Deadline</span>
          <span className="popup-val">{fra.deadline}</span>
        </div>
        <div className="popup-row">
          <span className="popup-label">Views</span>
          <span className="popup-val">{fra.amtOfViews}</span>
        </div>
        <div className="popup-row">
          <span className="popup-label">Status</span>
          <span className="popup-val">
            <span className={`badge ${!fra.status ? "badge-active" : "badge-completed"}`}>
              {fra.status ? "Completed" : "Active"}
            </span>
          </span>
        </div>

        {/* Donate section */}
        {!fra.status && (
          <div style={{ marginTop: "1.25rem", borderTop: "0.5px solid #2e3240", paddingTop: "1.25rem" }}>
            <div style={{ fontSize: "12px", color: "#9a9daa", marginBottom: "8px", fontWeight: "500" }}>Make a donation</div>
            <div style={{ display: "flex", gap: "8px" }}>
              <input
                type="number"
                className="admin-search"
                style={{ flex: 1 }}
                placeholder="Enter amount ($)"
                value={donationAmt}
                onChange={e => setDonationAmt(e.target.value)}
              />
              <button className="popup-edit-btn" style={{ flex: "none", width: "100px" }} onClick={handleDonate}>
                Donate
              </button>
            </div>
          </div>
        )}

        <div className="popup-actions">
          <button className="popup-edit-btn" onClick={handleFavourite}>Save to favourites</button>
        </div>
      </div>
    </div>
  );
}