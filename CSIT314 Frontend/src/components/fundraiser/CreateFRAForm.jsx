import { useState } from "react";

export default function CreateFRAForm({ onSuccess, onCancel }) {
  const [form, setForm] = useState({
    name: "",
    description: "",
    deadline: "",      // must be dd-MM-yyyy
    fraCategoryId: "",
    amtRequested: "",
  });
  const [error, setError] = useState("");

  const handleSubmit = async () => {
    setError("");
    const res = await fetch("/api/CreateFundraiser", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify({
        ...form,
        fraCategoryId: parseInt(form.fraCategoryId),
        amtRequested: parseFloat(form.amtRequested),
      }),
    });
    const text = await res.text();
    if (!res.ok) { setError(text); return; }
    onSuccess();
  };

  return (
    <div className="admin-form-card">
      <h2>Create activity</h2>
      {error && <div className="form-error">{error}</div>}

      <div className="form-field">
        <label>Name</label>
        <input value={form.name} onChange={e => setForm({ ...form, name: e.target.value })} />
      </div>
      <div className="form-field">
        <label>Description</label>
        <input value={form.description} onChange={e => setForm({ ...form, description: e.target.value })} />
      </div>
      <div className="form-field">
        <label>Deadline (dd-MM-yyyy)</label>
        <input
          value={form.deadline}
          placeholder="e.g. 31-12-2025"
          onChange={e => setForm({ ...form, deadline: e.target.value })}
        />
      </div>
      <div className="form-field">
        <label>Category ID</label>
        <input
          type="number"
          value={form.fraCategoryId}
          onChange={e => setForm({ ...form, fraCategoryId: e.target.value })}
        />
      </div>
      <div className="form-field">
        <label>Goal amount ($)</label>
        <input
          type="number"
          value={form.amtRequested}
          onChange={e => setForm({ ...form, amtRequested: e.target.value })}
        />
      </div>

      <div style={{ display: "flex", gap: "8px", marginTop: "0.5rem" }}>
        <button className="submit-btn" onClick={handleSubmit}>Create activity</button>
        <button className="admin-btn" onClick={onCancel}>Cancel</button>
      </div>
    </div>
  );
}