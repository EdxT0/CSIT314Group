import { useState, useEffect } from "react";

export default function CreateFRAForm({ onSuccess, onCancel }) {
  const [form, setForm] = useState({
    Name: "",
    Description: "",
    DeadlineInString: "",
    FraCategoryId: "",
    AmtRequested: "",
  });
  const [error, displayError] = useState("");
  const [success, displaySuccess] = useState("");
  const [categories, setCategories] = useState([]);  

  useEffect(() => {
    const fetchCategories = async () => {
      const res = await fetch("/api/ViewAllCategory", { credentials: "include" });
      if (res.ok) {
        const data = await res.json();
        setCategories(data);
      }
    };
    fetchCategories();
  }, []);

  const handleSubmit = async () => {
    displayError("");
    displaySuccess("");
    const res = await fetch("/api/CreateFundraiser", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify({
        ...form,
        FraCategoryId: parseInt(form.FraCategoryId),
        AmtRequested: parseFloat(form.AmtRequested),
      }),
    });
    const text = await res.text();
    if (!res.ok) { displayError(text); return; }
    displaySuccess("Activity created successfully!");
    setForm({ Name: "", Description: "", DeadlineInString: "", FraCategoryId: "", AmtRequested: "" });
    onSuccess?.();
  };

  return (
    <div className="admin-form-card">
      <h2>Create activity</h2>
      {success && <div className="form-success">{success}</div>}
      {error && <div className="form-error">{error}</div>}

      <div className="form-field">
        <label>Name</label>
        <input value={form.Name}
          onChange={e => setForm({ ...form, Name: e.target.value })} />
      </div>
      <div className="form-field">
        <label>Description</label>
        <input value={form.Description}
          onChange={e => setForm({ ...form, Description: e.target.value })} />
      </div>
      <div className="form-field">
        <label>Deadline (dd-MM-yyyy)</label>
        <input value={form.DeadlineInString} placeholder="e.g. 31-12-2025"
          onChange={e => setForm({ ...form, DeadlineInString: e.target.value })} />
      </div>


      <div className="form-field">
        <label>Category</label>
        <select
          value={form.FraCategoryId}
          onChange={e => setForm({ ...form, FraCategoryId: e.target.value })}>
          <option value="">Select a category...</option>
          {categories.map(c => (
            <option key={c.id} value={c.id}>{c.name}</option>
          ))}
        </select>
      </div>

      <div className="form-field">
        <label>Goal amount ($)</label>
        <input type="number" value={form.AmtRequested}
          onChange={e => setForm({ ...form, AmtRequested: e.target.value })} />
      </div>

      <div style={{ display: "flex", gap: "8px", marginTop: "0.5rem" }}>
        <button className="submit-btn" onClick={handleSubmit}>Create activity</button>
        <button className="admin-btn" onClick={onCancel}>Cancel</button>
      </div>
    </div>
  );
}