import { useState } from "react";

export default function AccountPopup({ acc, profiles, onClose, onSuccess }) {
  const [isEditing, setIsEditing] = useState(acc.startEditing ?? false);
  const [error, displayError] = useState("");// ← for displaying errors within the popup
  const [success, displaySuccess] = useState("");// ← for displaying success messages within the popup
  const [localAccount, setLocalAccount] = useState(acc); // ← tracks live status inside the popup
  const [form, setForm] = useState({
    id: acc.id,
    name: acc.name ?? "",
    email: acc.email ?? "",
    phoneNumber: acc.phoneNumber ?? "",
    profileName: acc.profileName ?? "",
    password: "",
  });

  const handleUpdate = async () => {
    displayError("");
    displaySuccess("");
    const res = await fetch("/api/UpdateUserAccount", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify(form),
    });
    const text = await res.text();
    if (!res.ok) { displayError(text); return; }
    setIsEditing(false);
    setLocalAccount({ 
      ...localAccount, 
      name: form.name, 
      email: form.email, 
      phoneNumber: form.phoneNumber, 
      profileName: form.profileName 
    });
    displaySuccess("Account updated successfully!");
    onSuccess?.("Account updated successfully!");  // ← refetches table in background
  };

  const handleSuspend = async () => {
    displayError("");
    displaySuccess("");
    const res = await fetch(`/api/SuspendUserAccount?userId=${encodeURIComponent(localAccount.id)}&suspendUser=${encodeURIComponent(!localAccount.isSuspended)}`, {
      method: "PUT",
      credentials: "include",
    });
    const text = await res.text();
    if (!res.ok) { displayError(text); return; }
    const newSuspended = !localAccount.isSuspended;
    setLocalAccount({ ...localAccount, isSuspended: newSuspended });
    displaySuccess(newSuspended ? "Account suspended" : "Account unsuspended");
    await onSuccess?.();
  };  
  return (
    <div className="popup-overlay" onClick={onClose}>
      <div className="popup-card" style={{ maxWidth: "440px" }} onClick={e => e.stopPropagation()}>

        <div className="popup-header">
          <h2>{isEditing ? "Edit account" : localAccount.name}</h2>
          <button className="popup-close" onClick={onClose}>✕</button>
        </div>

        {success && <div className="form-success">{success}</div>}  {/* ← Display success message within the popup */}
        {error && <div className="form-error">{error}</div>}  {/* ← Display error within the popup */}

        {!isEditing && (
          <>
            <div className="popup-row">
              <span className="popup-label">Name</span>
              <span className="popup-val">{localAccount.name}</span>      
            </div>
            <div className="popup-row">
              <span className="popup-label">Email</span>
              <span className="popup-val">{localAccount.email}</span>      
            </div>
            <div className="popup-row">
              <span className="popup-label">Phone</span>
              <span className="popup-val">{localAccount.phoneNumber}</span> 
            </div>
            <div className="popup-row">
              <span className="popup-label">Profile</span>
              <span className="popup-val">{localAccount.profileName}</span> 
            </div>
            <div className="popup-row">
              <span className="popup-label">Status</span>
              <span className="popup-val">
                <span className={`badge ${localAccount.isSuspended ? "badge-suspended" : "badge-active"}`}>
                  {localAccount.isSuspended ? "Suspended" : "Active"}
                </span>
              </span>
            </div>

            <div className="popup-actions">
              <button className="popup-edit-btn" onClick={() => { setIsEditing(true); displaySuccess(""); displayError(""); }}>Edit</button>
              <button
                className={`popup-delete-btn ${localAccount.isSuspended ? "popup-success-btn" : ""}`}
                onClick={handleSuspend}>
                {localAccount.isSuspended ? "Unsuspend" : "Suspend"}
              </button>
            </div>
          </>
        )}

        {isEditing && (
          <>
            <p style={{ fontSize: "13px", color: "#7a7d8a", marginBottom: "1.25rem" }}>
              Only fill in fields you want to change.
            </p>

            {["name", "email", "phoneNumber"].map(field => (
              <div className="form-field" key={field}>
                <label>{field.charAt(0).toUpperCase() + field.slice(1)}</label>
                <input
                  value={form[field]}
                  onChange={e => setForm({ ...form, [field]: e.target.value })}
                />
              </div>
            ))}

            <div className="form-field">
                <label>Profile</label>
                <select
                    value={form.profileName}
                    onChange={e => setForm({ ...form, profileName: e.target.value })}
                    >
                    <option value="">Select a profile...</option>
                    {profiles.map(p => (
                        <option key={p.id} value={p.profileName}>{p.profileName}</option>
                    ))}
                </select>
            </div>


            <div className="form-field">
              <label>New password</label>
              <input
                type="password"
                placeholder="Leave blank to keep current"
                value={form.password}
                onChange={e => setForm({ ...form, password: e.target.value })}
              />
            </div>

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