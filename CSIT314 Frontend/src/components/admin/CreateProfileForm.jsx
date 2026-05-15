import { useState } from "react";

export default function CreateProfileForm({ onSuccess, onCancel }) {
  const [form, setForm] = useState({ ProfileName: "", Description: "" });
  const [error, displayError] = useState("");
  const [success, displaySuccess] = useState("");

  const handleSubmit = async () => {
    displayError("");
    displaySuccess("");
    const res = await fetch("/api/CreateUserProfile", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify(form),
    });
    const text = await res.text();
    if (!res.ok) { displayError(text); return; }
    displaySuccess("Profile created successfully!");
    onSuccess();
  };

  return (
    <div className="admin-form-card">
      <h2>Create profile</h2>
      {error && <div className="form-error">{error}</div>}
      {success && <div className="form-success">{success}</div>}

      <div className="form-field">
        <label>Profile name</label>
        <input
          value={form.ProfileName}              
          placeholder="e.g. fundraiser, donee"
          onChange={e => setForm({ ...form, ProfileName: e.target.value })}  
        />
      </div>

      <div className="form-field">
        <label>Description</label>
        <input
          value={form.Description}             
          placeholder="Brief description of this role"
          onChange={e => setForm({ ...form, Description: e.target.value })}  
        />
      </div>

      <div style={{ display: "flex", gap: "8px", marginTop: "0.5rem" }}>
        <button className="submit-btn" onClick={handleSubmit}>Create profile</button>
        <button className="admin-btn" onClick={onCancel}>Cancel</button>
      </div>
    </div>
  );
}