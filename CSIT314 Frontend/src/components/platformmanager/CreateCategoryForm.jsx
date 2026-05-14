import { useState } from "react";

export default function CreateCategoryForm({ onSuccess, onCancel }) {
  const [form, setForm] = useState({ name: "", description: "" });
  const [error, displayError] = useState("");
  const [success, displaySuccess] = useState("");

  const handleSubmit = async () => {
    displayError("");
    displaySuccess("");
    const res = await fetch("/api/CreateCategory", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify(form),
    });
    const text = await res.text();
    if (!res.ok) { displayError(text); return; }
    displaySuccess("Category created successfully!");
    setForm({ name: "", description: "" });  // ← reset form but stay on page
    onSuccess?.();                            // ← refetch categories in background
  };

  return (
    <div className="admin-form-card">
      <h2>Create category</h2>
      {success && <div className="form-success">{success}</div>}
      {error && <div className="form-error">{error}</div>}

      <div className="form-field">
        <label>Name</label>
        <input
          value={form.name}
          placeholder="e.g. medical, education"
          onChange={e => setForm({ ...form, name: e.target.value })}
        />
      </div>
      <div className="form-field">
        <label>Description</label>
        <input
          value={form.description}
          placeholder="Brief description of this category"
          onChange={e => setForm({ ...form, description: e.target.value })}
        />
      </div>

      <div style={{ display: "flex", gap: "8px", marginTop: "0.5rem" }}>
        <button className="submit-btn" onClick={handleSubmit}>Create category</button>
        <button className="admin-btn" onClick={onCancel}>Cancel</button>
      </div>
    </div>
  );
}