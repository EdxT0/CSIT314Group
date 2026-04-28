import { useState } from "react";

export default function EditAccountForm({ account, profiles, onSuccess, onCancel }) {
  const [form, setForm] = useState({
    id: account.id,
    name: account.name,
    email: account.email,
    phoneNumber: account.phoneNumber,
    profileName: account.profileName,
    password: ""
  });
  const [error, setError] = useState("");

  const handleSubmit = async () => {
    setError("");
    const res = await fetch("/api/UpdateUserAccount", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify(form),
    });
    const text = await res.text();
    if (!res.ok) { setError(text); return; }
    onSuccess();
  };

  return (
    <div className="admin-form-card">
      <h2>Edit account</h2>
      <p style={{ fontSize: "13px", color: "#7a7d8a", marginBottom: "1.25rem" }}>
        Only fill in fields you want to change.
      </p>
      {error && <div className="form-error">{error}</div>}

      {["name", "email", "phoneNumber", "password"].map(field => (
        <div className="form-field" key={field}>
          <label>{field.charAt(0).toUpperCase() + field.slice(1)}</label>
          <input
            type={field === "password" ? "password" : "text"}
            value={form[field]}
            placeholder={field === "password" ? "Leave blank to keep current" : ""}
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

      <div style={{ display: "flex", gap: "8px", marginTop: "0.5rem" }}>
        <button className="submit-btn" onClick={handleSubmit}>Save changes</button>
        <button className="admin-btn" onClick={onCancel}>Cancel</button>
      </div>
    </div>
  );
}