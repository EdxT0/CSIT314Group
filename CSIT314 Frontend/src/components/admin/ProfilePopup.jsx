import { useState } from "react";

export default function ProfilePopup({ profile, onClose, onSuspend, onSuccess }) {
  const [isEditing, setIsEditing] = useState(profile.startEditing ?? false);
  const [form, setForm] = useState({
    id: profile.id,
    profileName: profile.profileName,
    description: profile.description,
  });
  const [error, displayError] = useState(""); // ← for displaying errors within the popup
  const [success, displaySuccess] = useState(""); // ← for displaying success messages within the popup

  const handleUpdate = async () => {        // ← update profile details
    displayError("");
    const res = await fetch("/api/UpdateUserProfile", {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify(form),
    });
    const text = await res.text();
    if (!res.ok) { displayError(text); return; }
    setIsEditing(false);
    displaySuccess("Profile updated successfully!"); // ← show success message within the popup
  };

  const handleSuspend = async () => {
    displayError("");
    await onSuspend(profile.id, profile.status == 0);
    displaySuccess(profile.status == 0 ? "Profile suspended" : "Profile unsuspended"); // ← show status change message
  };

  return (
    <div className="popup-overlay" onClick={onClose}>
      <div className="popup-card" style={{ maxWidth: "440px" }} onClick={e => e.stopPropagation()}>

        <div className="popup-header">
          <h2>{isEditing ? "Edit profile" : profile.profileName}</h2>
          <button className="popup-close" onClick={onClose}>✕</button>
        </div>

        {error && <div className="form-error">{error}</div>} {/* ← Display error within the popup */}
        {success && <div className="form-success">{success}</div>}  {/* ← Display success message within the popup */}

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
              <button className="popup-edit-btn" onClick={handleUpdate}>Save changes</button> {/* Finish editing but stay on popup */}
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