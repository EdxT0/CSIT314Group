import { useState, useEffect } from "react";
import { fraApi } from "../fraApi";
import "../../styles/adminpage.css";

// Receives: fra (the FRA object to edit), onSuccess(), onCancel()
export default function EditFRAForm({ fra, onSuccess, onCancel }) {
  const [categories, setCategories] = useState([]);
  const [error, setError] = useState("");
  const [submitting, setSubmitting] = useState(false);

  // Pre-fill from existing FRA; all fields optional on update
  const [form, setForm] = useState({
    fraName: fra.name ?? "",
    description: fra.description ?? "",
    deadline: isoToInputDate(fra.deadline),
    fraCategoryId: fra.fraCategoryId?.toString() ?? "",
    amtRequested: fra.amtRequested?.toString() ?? "",
    status: fra.status?.toString() ?? "",
  });

  useEffect(() => {
    fraApi.getCategories()
      .then((res) => (res.ok ? res.json() : []))
      .then(setCategories)
      .catch(() => {});
  }, []);

  // Backend stores ISO; HTML date input needs yyyy-MM-dd
  function isoToInputDate(iso) {
    if (!iso) return "";
    return iso.slice(0, 10); // "2025-09-30T00:00:00" → "2025-09-30"
  }

  // HTML date input yyyy-MM-dd → backend dd-MM-yyyy
  function formatDeadline(dateStr) {
    if (!dateStr) return undefined;
    const [y, m, d] = dateStr.split("-");
    return `${d}-${m}-${y}`;
  }

  function handleChange(e) {
    setForm((prev) => ({ ...prev, [e.target.name]: e.target.value }));
  }

  async function handleSubmit(e) {
    e.preventDefault();
    setError("");
    setSubmitting(true);

    // Only send fields that have changed — backend rejects same-value updates
    const payload = { fraId: fra.id };
    if (form.fraName.trim() && form.fraName.trim() !== fra.name)
      payload.fraName = form.fraName.trim();
    if (form.description.trim() && form.description.trim() !== fra.description)
      payload.description = form.description.trim();
    if (form.deadline && formatDeadline(form.deadline) !== formatDeadline(isoToInputDate(fra.deadline)))
      payload.deadline = formatDeadline(form.deadline);
    if (form.fraCategoryId && parseInt(form.fraCategoryId) !== fra.fraCategoryId)
      payload.fraCategoryId = parseInt(form.fraCategoryId);
    if (form.amtRequested && parseFloat(form.amtRequested) !== fra.amtRequested)
      payload.amtRequested = parseFloat(form.amtRequested);
    if (form.status !== "" && parseInt(form.status) !== fra.status)
      payload.status = parseInt(form.status);

    if (Object.keys(payload).length === 1) {
      setError("No changes detected.");
      setSubmitting(false);
      return;
    }

    try {
      const res = await fraApi.update(payload);
      const text = await res.text();
      if (!res.ok) { setError(text || "Failed to update fundraiser."); return; }
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
        <h1>Edit Fundraising Activity</h1>
      </div>

      <div className="admin-form-card">
        <h2>Edit FRA — {fra.name}</h2>

        {error && <div className="form-error">{error}</div>}

        <div className="form-field">
          <label>Activity Name</label>
          <input
            name="fraName"
            value={form.fraName}
            onChange={handleChange}
            placeholder="Leave unchanged to keep current name"
          />
        </div>

        <div className="form-field">
          <label>Description</label>
          <input
            name="description"
            value={form.description}
            onChange={handleChange}
            placeholder="Leave unchanged to keep current description"
          />
        </div>

        <div className="form-field">
          <label>Category</label>
          <select name="fraCategoryId" value={form.fraCategoryId} onChange={handleChange}>
            <option value="">— keep current —</option>
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

        <div className="form-field">
          <label>Status</label>
          <select name="status" value={form.status} onChange={handleChange}>
            <option value="">— keep current —</option>
            <option value="0">Active</option>
            <option value="1">Completed</option>
            <option value="2">Paused</option>
          </select>
        </div>

        <div style={{ display: "flex", gap: "10px", marginTop: "0.5rem" }}>
          <button className="submit-btn" onClick={handleSubmit} disabled={submitting} style={{ flex: 1 }}>
            {submitting ? "Saving…" : "Save Changes"}
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
