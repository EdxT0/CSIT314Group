import { useState } from "react";

export default function ProfilePopup({ profile, onClose, onSuspend, onSuccess }) {
  const [isEditing, setIsEditing] = useState(profile.startEditing ?? false);
  const [form, setForm] = useState({
    id: profile.id,
    profileName: profile.profileName,
    description: profile.description,
  });
  const [error, displayError] = useState("");
  const [message, setMessage] = useState("");

  const handleUpdate = async () => {
    displayError("");
    const res = await fetch("/api/UpdateUserAccount", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify(form),
    });
    const text = await res.text();
    console.log("Update response:", { status: res.status, text });
    if (!res.ok) { displayError(text); return; }
    setMessage("Account updated successfully!");
    setIsEditing(false);
    onSuccess?.();
  };

  const handleSuspend = async () => {
    await onSuspend(profile.id, profile.status == 0);
    onClose();
  };

  return (
    <div className="popup-overlay" onClick={onClose}>
      <div className="popup-card" style={{ maxWidth: "440px" }} onClick={e => e.stopPropagation()}>

        <div className="popup-header">
          <h2>{isEditing ? "Edit profile" : profile.profileName}</h2>
          <button className="popup-close" onClick={onClose}>✕</button>
        </div>

        {message && (
          <div style={{ background: "#0f2e1a", border: "0.5px solid #1d9e75", borderRadius: "8px", padding: "8px 12px", fontSize: "13px", color: "#5dcaa5", marginBottom: "1rem" }}>
            {message}
          </div>
        )}
        {error && <div className="form-error">{error}</div>}

        {!isEditing && (
          <>
            <div className="popup-row">
              <span className="popup-label">Name</span>
              <span className="popup-val">{profile.profileName}</span>
            </div>
            <div className="popup-row">
              <span className="popup-label">Description</span>
              <span className="popup-val">{profile.description}</span>
            </div>
            <div className="popup-row">
              <span className="popup-label">Phone</span>
              <span className="popup-val">{profile.phoneNumber}</span>
            </div>
            <div className="popup-row">
              <span className="popup-label">Profile</span>
              <span className="popup-val">{profile.profileName}</span>
            </div>
            <div className="popup-row">
              <span className="popup-label">Status</span>
              <span className="popup-val">
                <span className={`badge ${profile.status == 0 ? "badge-active" : "badge-suspended"}`}>
                  {profile.status == 0 ? "Active" : "Suspended"}
                </span>
              </span>
            </div>

            <div className="popup-actions">
              <button className="popup-edit-btn" onClick={() => setIsEditing(true)}>Edit</button>
              <button className="popup-delete-btn" onClick={handleSuspend}>
                {profile.status == 0 ? "Suspend" : "Unsuspend"}
              </button>
            </div>
          </>
        )}

        {isEditing && (
          <>
            <p style={{ fontSize: "13px", color: "#7a7d8a", marginBottom: "1.25rem" }}>
              Only fill in fields you want to change.
            </p>

            {["profileName", "description"].map(field => (
              <div className="form-field" key={field}>
                <label>{field.charAt(0).toUpperCase() + field.slice(1)}</label>
                <input
                  value={form[field]}
                  onChange={e => setForm({ ...form, [field]: e.target.value })}
                />
              </div>
            ))}

            <div className="popup-actions">
              <button className="popup-edit-btn" onClick={handleUpdate}>Save changes</button>
              <button className="admin-btn" onClick={() => { setIsEditing(false); displayError(""); }}>
                Cancel
              </button>
            </div>
          </>
        )}
      </div>
    </div>
  );
}