import { useState, useEffect } from "react";
import { fraApi } from "../fraApi";
import "../../styles/adminpage.css";

// Receives: onSuccess(), onCancel()
export default function CreateFRAForm({ onSuccess, onCancel }) {
  const [categories, setCategories] = useState([]);
  const [error, setError] = useState("");
  const [submitting, setSubmitting] = useState(false);
  const [form, setForm] = useState({
    name: "",
    description: "",
    deadline: "",       // date input → formatted to dd-MM-yyyy on submit
    fraCategoryId: "",
    amtRequested: "",
  });

  useEffect(() => {
    fraApi.getCategories()
      .then((res) => (res.ok ? res.json() : []))
      .then(setCategories)
      .catch(() => {});
  }, []);

  function handleChange(e) {
    setForm((prev) => ({ ...prev, [e.target.name]: e.target.value }));
  }

  // Convert yyyy-MM-dd (HTML date input) → dd-MM-yyyy (backend expects)
  function formatDeadline(dateStr) {
    if (!dateStr) return "";
    const [y, m, d] = dateStr.split("-");
    return `${d}-${m}-${y}`;
  }

  async function handleSubmit(e) {
    e.preventDefault();
    setError("");

    if (!form.name.trim() || !form.deadline || !form.fraCategoryId || !form.amtRequested) {
      setError("All fields are required.");
      return;
    }

    setSubmitting(true);
    try {
      const res = await fraApi.create({
        name: form.name.trim(),
        description: form.description.trim(),
        deadline: formatDeadline(form.deadline),
        fraCategoryId: parseInt(form.fraCategoryId),
        amtRequested: parseFloat(form.amtRequested),
      });
      const text = await res.text();
      if (!res.ok) { setError(text || "Failed to create fundraiser."); return; }
      onSuccess();
    } catch {
      setError("Network error. Please try again.");
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <>
      <div className="admin-topbar">
        <h1>Create Fundraising Activity</h1>
      </div>

      <div className="admin-form-card">
        <h2>New FRA</h2>

        {error && <div className="form-error">{error}</div>}

        <div className="form-field">
          <label>Activity Name</label>
          <input
            name="name"
            value={form.name}
            onChange={handleChange}
            placeholder="e.g. Annual Charity Run 2025"
          />
        </div>

        <div className="form-field">
          <label>Description</label>
          <input
            name="description"
            value={form.description}
            onChange={handleChange}
            placeholder="Describe the activity"
          />
        </div>

        <div className="form-field">
          <label>Category</label>
          <select name="fraCategoryId" value={form.fraCategoryId} onChange={handleChange}>
            <option value="">Select a category</option>
            {categories.map((cat) => (
              <option key={cat.id} value={cat.id}>{cat.name}</option>
            ))}
          </select>
        </div>

        <div className="form-field">
          <label>Target Amount (SGD)</label>
          <input
            type="number"
            name="amtRequested"
            value={form.amtRequested}
            onChange={handleChange}
            placeholder="e.g. 10000"
            min="0"
          />
        </div>

        <div className="form-field">
          <label>Deadline</label>
          <input
            type="date"
            name="deadline"
            value={form.deadline}
            onChange={handleChange}
          />
        </div>

        <div style={{ display: "flex", gap: "10px", marginTop: "0.5rem" }}>
          <button className="submit-btn" onClick={handleSubmit} disabled={submitting} style={{ flex: 1 }}>
            {submitting ? "Creating…" : "Create Activity"}
          </button>
          <button
            className="admin-btn"
            onClick={onCancel}
            style={{ flex: 1, height: "40px", fontSize: "14px" }}
          >
            Cancel
          </button>
        </div>
      </div>
    </>
  );
}
