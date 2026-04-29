import { useState } from "react";

export default function CreateFRAForm({ onSuccess, onCancel }) {
  const [form, setForm] = useState({ name: "", description: "", deadline: "", });
  const [error, setError] = useState("");
/*        public int Id { get; set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Deadline { get; private set; }
        public bool Status { get; private set; }
        public double AmtRequested { get; private set; }
        public double AmtDonated { get; private set; }
        public int AmtOfViews { get; private set; }

        public string FraCategoryName { get; private set; } */

  const handleSubmit = async () => {
    setError("");
    const res = await fetch("/api/CreateUserProfile", {
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
      <h2>Create profile</h2>
      {error && <div className="form-error">{error}</div>}

      <div className="form-field">
        <label>Profile name</label>
        <input
          value={form.profileName}
          placeholder="e.g. fundraiser, donee"
          onChange={e => setForm({ ...form, profileName: e.target.value })}
        />
      </div>

      <div className="form-field">
        <label>Description</label>
        <input
          value={form.description}
          placeholder="Brief description of this role"
          onChange={e => setForm({ ...form, description: e.target.value })}
        />
      </div>

      <div style={{ display: "flex", gap: "8px", marginTop: "0.5rem" }}>
        <button className="submit-btn" onClick={handleSubmit}>Create profile</button>
        <button className="admin-btn" onClick={onCancel}>Cancel</button>
      </div>
    </div>
  );
}