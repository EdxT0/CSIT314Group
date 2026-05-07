import { useState } from "react";

export default function CreateAccountForm({ profiles, onSuccess, onCancel }) {
  const [form, setForm] = useState({
    Name: "", 
    Email: "", 
    PhoneNumber: "",
    HashedPassword: "", 
    ProfileId: "", 
    IsSuspended: false
  });
  const [error, displayError] = useState("");

  const handleSubmit = async () => {
    displayError("");
    const res = await fetch("/api/CreateUserAccount", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify(form),
    });
    const text = await res.text();
    if (!res.ok) { displayError(text); return; }
    onSuccess();
  };

  return (
    <div className="admin-form-card">
      <h2>Create account</h2>
      {error && <div className="form-error">{error}</div>}

      {["Name", "Email", "PhoneNumber", "HashedPassword"].map(field => (
        <div className="form-field" key={field}>
          <label>{field.charAt(0).toUpperCase() + field.slice(1)}</label>
          <input
            type={field === "HashedPassword" ? "password" : "text"}
            value={form[field]}
            onChange={e => setForm({ ...form, [field]: e.target.value })}
          />
        </div>
      ))}

      <div className="form-field">
        <label>Profile</label>
        <select
          value={form.ProfileId}
          onChange={e => setForm({ ...form, ProfileId: e.target.value })}
        >
          <option value="">Select a profile...</option>
          {profiles.map(p => (
            <option key={p.id} value={p.id}>{p.profileName}</option>
          ))}
        </select>
      </div>

      <div style={{ display: "flex", gap: "8px", marginTop: "0.5rem" }}>
        <button className="submit-btn" onClick={handleSubmit}>Create account</button>
        <button className="admin-btn" onClick={onCancel}>Cancel</button>
      </div>
    </div>
  );
}