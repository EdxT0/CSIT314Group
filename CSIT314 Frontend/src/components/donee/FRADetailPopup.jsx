import { useState, useEffect } from "react";
import { formatDeadline } from "../../utils/formatDeadline";

export default function FRADetailPopup({ fra, onClose, onSuccess, isFavourited: initialFavourited = false }) {
  const [donationAmt, setDonationAmt] = useState("");
  const [errorMessage, setErrorMessage] = useState("");
  const [successMessage, setSuccessMessage] = useState("");
  const [isFavourited, setIsFavourited] = useState(initialFavourited);
  const [donations, setDonations] = useState([]);
  const [loadingDonations, setLoadingDonations] = useState(true);
  const [localAmtDonated, setLocalAmtDonated] = useState(fra.amtDonated ?? 0);

  // ← proper named functions for BCE diagram
  const displayErrorMessage = (msg) => setErrorMessage(msg);
  const clearErrorMessage = () => setErrorMessage("");
  const displaySuccessMessage = (msg) => setSuccessMessage(msg);
  const clearSuccessMessage = () => setSuccessMessage("");
  const updateDonations = (data) => setDonations(data);
  const clearDonations = () => setDonations([]);

  useEffect(() => {
    fetchDonationHistory();
  }, [fra.id]);

  const fetchDonationHistory = async () => {
    setLoadingDonations(true);
    const res = await fetch(`/api/SearchDonationHistory?fraName=${encodeURIComponent(fra.name)}`, {
      credentials: "include",
    });
    if (res.ok) {
      const data = await res.json();
      updateDonations(Array.isArray(data) ? data : [data]);
    } else {
      clearDonations();
    }
    setLoadingDonations(false);
  };

  const handleFavourite = async () => {
    clearErrorMessage();
    clearSuccessMessage();
    if (isFavourited) {
      const res = await fetch("/api/UnfavouriteFundraiser", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify({ FraId: fra.id }),
      });
      const text = await res.text();
      if (!res.ok) { displayErrorMessage(text); return; }
      setIsFavourited(false);
      displaySuccessMessage("Removed from favourites!");
    } else {
      const res = await fetch("/api/FavouriteFundraiser", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify({ FraId: fra.id }),
      });
      const text = await res.text();
      if (!res.ok) { displayErrorMessage(text); return; }
      setIsFavourited(true);
      displaySuccessMessage("Added to favourites!");
    }
    onSuccess?.();
  };

  const handleDonate = async () => {
    clearErrorMessage();
    clearSuccessMessage();
    if (!donationAmt || parseFloat(donationAmt) <= 0) {
      displayErrorMessage("Please enter a valid donation amount");
      return;
    }
    const res = await fetch("/api/AddDonation", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify({ id: fra.id, amtDonated: parseFloat(donationAmt) }),
    });
    const text = await res.text();
    if (!res.ok) { displayErrorMessage(text); return; }
    setLocalAmtDonated(prev => prev + parseFloat(donationAmt));
    displaySuccessMessage("Donation successful!");
    setDonationAmt("");
    fetchDonationHistory();
    onSuccess?.();
  };

  const progress = fra.amtRequested > 0
    ? Math.min((localAmtDonated / fra.amtRequested) * 100, 100).toFixed(1)
    : 0;

  const isCompleted = fra.amtRequested > 0 && localAmtDonated >= fra.amtRequested;

  return (
    <div className="popup-overlay" onClick={onClose}>
      <div className="popup-card" style={{ maxWidth: "480px" }} onClick={e => e.stopPropagation()}>

        <div className="popup-header">
          <h2>{fra.name}</h2>
          <div style={{ display: "flex", gap: "8px", alignItems: "center" }}>
            <button
              onClick={handleFavourite}
              title={isFavourited ? "Unfavourite" : "Add to favourites"}
              style={{
                width: "28px",
                height: "28px",
                background: isFavourited ? "#2a1a1a" : "#22262f",
                border: `0.5px solid ${isFavourited ? "#7a2020" : "#2e3240"}`,
                borderRadius: "6px",
                color: isFavourited ? "#f09595" : "#9a9daa",
                fontSize: "14px",
                cursor: "pointer",
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
              }}>
              {isFavourited ? "♥" : "♡"}
            </button>
            <button className="popup-close" onClick={onClose}>✕</button>
          </div>
        </div>

        {successMessage && <div className="form-success">{successMessage}</div>}
        {errorMessage && <div className="form-error">{errorMessage}</div>}

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
          <span className="popup-val">${localAmtDonated?.toLocaleString()} ({progress}%)</span>
        </div>
        <div className="popup-row">
          <span className="popup-label">Deadline</span>
          <span className="popup-val">{fra.deadlineInString || fra.deadline || "—"}</span>
        </div>
        <div className="popup-row">
          <span className="popup-label">Views</span>
          <span className="popup-val">{fra.amtOfViews}</span>
        </div>
        <div className="popup-row" style={{ borderBottom: "none" }}>
          <span className="popup-label">Status</span>
          <span className="popup-val">
            <span className={`badge ${!isCompleted && !fra.status ? "badge-active" : "badge-completed"}`}>
              {isCompleted || fra.status ? "Completed" : "Active"}
            </span>
          </span>
        </div>

        <div style={{ marginTop: "1rem", borderTop: "0.5px solid #2e3240", paddingTop: "0.75rem" }}>
          <div style={{ fontSize: "13px", color: "#9a9daa", fontWeight: "500", marginBottom: "6px" }}>
            Donation history
          </div>
          <div style={{
            height: "100px",
            overflowY: "auto",
            border: "0.5px solid #2e3240",
            borderRadius: "8px",
            padding: "4px 8px",
          }}>
            {loadingDonations ? (
              <div style={{ fontSize: "13px", color: "#7a7d8a", padding: "8px 0" }}>Loading...</div>
            ) : donations.length === 0 ? (
              <div style={{ fontSize: "13px", color: "#7a7d8a", padding: "8px 0" }}>No donations yet</div>
            ) : (
              donations.map((d, i) => (
                <div key={i} style={{
                  display: "flex",
                  justifyContent: "space-between",
                  fontSize: "13px",
                  padding: "4px 0",
                  borderBottom: i < donations.length - 1 ? "0.5px solid #2e3240" : "none"
                }}>
                  <span style={{ color: "#9a9daa" }}>{formatDeadline(d.dateDonated)}</span>
                  <span style={{ color: "#e8e6e1" }}>${d.userDonatedAmt?.toLocaleString()}</span>
                </div>
              ))
            )}
          </div>
        </div>

        {!isCompleted && !fra.status && (
          <div style={{ marginTop: "1rem", borderTop: "0.5px solid #2e3240", paddingTop: "0.75rem" }}>
            <div style={{ fontSize: "13px", color: "#9a9daa", fontWeight: "500", marginBottom: "6px" }}>
              Make a donation
            </div>
            <div style={{ display: "flex", gap: "8px" }}>
              <input
                type="number"
                className="admin-search"
                style={{ flex: 1 }}
                placeholder="Enter amount ($)"
                value={donationAmt}
                onChange={e => setDonationAmt(e.target.value)}
              />
              <button
                className="popup-edit-btn"
                style={{ flex: "none", width: "100px" }}
                onClick={handleDonate}>
                Donate
              </button>
            </div>
          </div>
        )}

      </div>
    </div>
  );
}