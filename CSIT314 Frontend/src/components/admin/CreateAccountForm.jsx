import { useState } from "react";

export default function CreateAccountForm({ profiles, onSuccess, onCancel }) {
  const [form, setForm] = useState({
    name: "", email: "", phoneNumber: "",
    password: "", profileName: "", isSuspended: false
  });
  const [error, setError] = useState("");

  const handleSubmit = async () => {
    setError("");
    const res = await fetch("/api/CreateUserAccount", {
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
      <h2>Create account</h2>
      {error && <div className="form-error">{error}</div>}

      {["name", "email", "phoneNumber", "password"].map(field => (
        <div className="form-field" key={field}>
          <label>{field.charAt(0).toUpperCase() + field.slice(1)}</label>
          <input
            type={field === "password" ? "password" : "text"}
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

      <div style={{ display: "flex", gap: "8px", marginTop: "0.5rem" }}>
        <button className="submit-btn" onClick={handleSubmit}>Create account</button>
        <button className="admin-btn" onClick={onCancel}>Cancel</button>
      </div>
    </div>
  );
}