import { useState } from "react";

export default function AccountPopup({ acc, profiles, onClose, onSuspend, onSuccess }) {
  const [isEditing, setIsEditing] = useState(acc.startEditing ?? false);
  const [error, displayError] = useState("");
  const [message, displayMessage] = useState("");

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
    const res = await fetch("/api/UpdateUserAccount", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify(form),
    });
    const text = await res.text();
    console.log("Update response:", { status: res.status, text });
    if (!res.ok) { displayError(text); return; }
    displayMessage("Account updated successfully!");
    setIsEditing(false);
    onSuccess?.("Account updated successfully!");  
  };

  const handleSuspend = async () => {
    await onSuspend(acc.id, !acc.isSuspended);
    onClose();
  };

  return (
    <div className="popup-overlay" onClick={onClose}>
      <div className="popup-card" style={{ maxWidth: "440px" }} onClick={e => e.stopPropagation()}>

        <div className="popup-header">
          <h2>{isEditing ? "Edit account" : acc.name}</h2>
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
              <span className="popup-val">{acc.name}</span>
            </div>
            <div className="popup-row">
              <span className="popup-label">Email</span>
              <span className="popup-val">{acc.email}</span>
            </div>
            <div className="popup-row">
              <span className="popup-label">Phone</span>
              <span className="popup-val">{acc.phoneNumber}</span>
            </div>
            <div className="popup-row">
              <span className="popup-label">Profile</span>
              <span className="popup-val">{acc.profileName}</span>
            </div>
            <div className="popup-row">
              <span className="popup-label">Status</span>
              <span className="popup-val">
                <span className={`badge ${acc.isSuspended ? "badge-suspended" : "badge-active"}`}>
                  {acc.isSuspended ? "Suspended" : "Active"}
                </span>
              </span>
            </div>

            <div className="popup-actions">
              <button className="popup-edit-btn" onClick={() => setIsEditing(true)}>Edit</button>
              <button className="popup-delete-btn" onClick={handleSuspend}>
                {acc.isSuspended ? "Unsuspend" : "Suspend"}
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