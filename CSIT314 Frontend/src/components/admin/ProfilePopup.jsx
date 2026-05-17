import { useState } from "react";
export default function ProfilePopup({ profile, onClose, onSuspend, onSuccess }) {
  const [isEditing, setIsEditing] = useState(profile.startEditing ?? false);
  const [error, displayError] = useState(""); // ← for displaying errors within the popup
  const [success, displaySuccess] = useState(""); // ← for displaying success messages within the popup
  const [localProfile, setLocalProfile] = useState(profile); // ← tracks live status inside the popup
  const [form, setForm] = useState({
    id: profile.id,
    profileName: profile.profileName,
    description: profile.description,
  });

  // Function to handle profile update
  const handleUpdate = async () => {
    displayError("");
    displaySuccess("");
    const res = await fetch("/api/UpdateUserProfile", {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify(form),
    });
    const text = await res.text();
    if (!res.ok) { displayError(text); return; }
    setIsEditing(false);
    setLocalProfile({ ...localProfile, profileName: form.profileName, description: form.description }); // ← update localProfile to reflect changes
    displaySuccess("Profile updated successfully!"); // ← show success message within the popup
    await onSuccess?.();
  };

  // Function to handle profile suspension/unsuspension 
  const handleSuspend = async () => {
    displayError("");
    displaySuccess("");
    await onSuspend(localProfile.id, localProfile.status == 0);  

    const newStatus = localProfile.status == 0 ? 1 : 0;
    setLocalProfile({ ...localProfile, status: newStatus });
    displaySuccess(newStatus == 1 ? "Profile suspended" : "Profile unsuspended");
    await onSuccess?.();
  };
  return (
    <div className="popup-overlay" onClick={onClose}>
      <div className="popup-card" style={{ maxWidth: "440px" }} onClick={e => e.stopPropagation()}>
        <div className="popup-header">
          <h2>{isEditing ? "Edit profile" : localProfile.profileName}</h2> {/* Use localProfile for display */}
          <button className="popup-close" onClick={onClose}>✕</button>
        </div>

        {error && <div className="form-error">{error}</div>} {/* ← Display error within the popup */}
        {success && <div className="form-success">{success}</div>}  {/* ← Display success message within the popup */}

        {!isEditing && (
          <>
            <div className="popup-row">
              <span className="popup-label">Name</span>
              <span className="popup-val">{localProfile.profileName}</span> {/* Use localProfile for display */}
            </div>
            <div className="popup-row">
              <span className="popup-label">Description</span>
              <span className="popup-val">{localProfile.description}</span> {/* Use localProfile for display */}
            </div>
            <div className="popup-row">
              <span className="popup-label">Status</span>
              <span className="popup-val">
                <span className={`badge ${localProfile.status == 0 ? "badge-active" : "badge-suspended"}`}>
                  {localProfile.status == 0 ? "Active" : "Suspended"}
                </span>
              </span>
            </div>

            <div className="popup-actions">
              <button className="popup-edit-btn" onClick={() => { setIsEditing(true); displaySuccess(""); displayError(""); }}>Edit</button>
              <button 
                className={`popup-delete-btn ${localProfile.status == 1 ? "popup-success-btn" : ""}`}
                onClick={handleSuspend}>
                {localProfile.status == 0 ? "Suspend" : "Unsuspend"}
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
              <button className="admin-btn" onClick={() => { setIsEditing(false); displayError(""); displaySuccess(""); }}> 
                Cancel
              </button>
            </div>
          </>
        )}
      </div>
    </div>
  );
}