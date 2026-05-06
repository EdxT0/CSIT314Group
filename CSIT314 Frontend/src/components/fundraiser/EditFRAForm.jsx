import { useState } from "react";

export default function EditFRAForm({ fra, onSuccess, onCancel }) {
  const [form, setForm] = useState({
    fraId: fra.id,
    fraName: fra.name ?? "",
    description: fra.description ?? "",
    deadline: "",       // leave blank to keep current
    status: fra.status,
    amtRequested: fra.amtRequested ?? "",
    fraCategoryId: fra.fraCategoryId ?? "",
  });
  const [error, setError] = useState("");

  const handleSubmit = async () => {
    setError("");
    const res = await fetch("/api/UpdateFundraiser", {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify({
        ...form,
        amtRequested: form.amtRequested ? parseFloat(form.amtRequested) : null,
        fraCategoryId: form.fraCategoryId ? parseInt(form.fraCategoryId) : null,
      }),
    });
    const text = await res.text();
    if (!res.ok) { setError(text); return; }
    onSuccess();
  };

  return (
    <div className="admin-form-card">
      <h2>Edit activity</h2>
      <p style={{ fontSize: "13px", color: "#7a7d8a", marginBottom: "1.25rem" }}>
        Only fill in fields you want to change.
      </p>
      {error && <div className="form-error">{error}</div>}

      <div className="form-field">
        <label>Name</label>
        <input value={form.fraName} onChange={e => setForm({ ...form, fraName: e.target.value })} />
      </div>
      <div className="form-field">
        <label>Description</label>
        <input value={form.description} onChange={e => setForm({ ...form, description: e.target.value })} />
      </div>
      <div className="form-field">
        <label>Deadline (dd-MM-yyyy)</label>
        <input
          value={form.deadline}
          placeholder="Leave blank to keep current"
          onChange={e => setForm({ ...form, deadline: e.target.value })}
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
      <div className="form-field">
        <label>Status</label>
        <select
          value={form.status ? "true" : "false"}
          onChange={e => setForm({ ...form, status: e.target.value === "true" })}
        >
          <option value="false">Active</option>
          <option value="true">Completed</option>
        </select>
      </div>
      <div className="form-field">
        <label>Category ID</label>
        <input
          type="number"
          value={form.fraCategoryId}
          onChange={e => setForm({ ...form, fraCategoryId: e.target.value })}
        />
      </div>

      <div style={{ display: "flex", gap: "8px", marginTop: "0.5rem" }}>
        <button className="submit-btn" onClick={handleSubmit}>Save changes</button>
        <button className="admin-btn" onClick={onCancel}>Cancel</button>
      </div>
    </div>
  );
}