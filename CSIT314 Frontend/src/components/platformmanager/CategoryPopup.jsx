import { useState } from "react";

export default function CategoryPopup({ category, onClose, onSuccess }) {
  const [isEditing, setIsEditing] = useState(category.startEditing ?? false);
  const [error, displayError] = useState("");
  const [success, displaySuccess] = useState("");
  const [localCategory, setLocalCategory] = useState(category);
  const [form, setForm] = useState({
    id: category.id,
    name: category.name ?? "",
    description: category.description ?? "",
  });

  const handleUpdate = async () => {
    displayError("");
    const res = await fetch("/api/UpdateCategory", {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify(form),
    });
    const text = await res.text();
    if (!res.ok) { displayError(text); return; }
    setLocalCategory({ ...localCategory, name: form.name, description: form.description });
    setIsEditing(false);
    displaySuccess("Category updated successfully!");
    await onSuccess?.();
  };

  return (
    <div className="popup-overlay" onClick={onClose}>
      <div className="popup-card" style={{ maxWidth: "440px" }} onClick={e => e.stopPropagation()}>

        <div className="popup-header">
          <h2>{isEditing ? "Edit category" : localCategory.name}</h2>
          <button className="popup-close" onClick={onClose}>✕</button>
        </div>

        {success && <div className="form-success">{success}</div>}
        {error && <div className="form-error">{error}</div>}

        {!isEditing && (
          <>
            <div className="popup-row">
              <span className="popup-label">Name</span>
              <span className="popup-val">{localCategory.name}</span>
            </div>
            <div className="popup-row" style={{ borderBottom: "none" }}>
              <span className="popup-label">Description</span>
              <span className="popup-val">{localCategory.description}</span>
            </div>

            <div className="popup-actions">
              <button
                className="popup-edit-btn"
                onClick={() => { setIsEditing(true); displaySuccess(""); displayError(""); }}>
                Edit
              </button>
            </div>
          </>
        )}

        {isEditing && (
          <>
            <p style={{ fontSize: "13px", color: "#7a7d8a", marginBottom: "1.25rem" }}>
              Only fill in fields you want to change.
            </p>

            <div className="form-field">
              <label>Name</label>
              <input
                value={form.name}
                onChange={e => setForm({ ...form, name: e.target.value })}
              />
            </div>
            <div className="form-field">
              <label>Description</label>
              <input
                value={form.description}
                onChange={e => setForm({ ...form, description: e.target.value })}
              />
            </div>

            <div className="popup-actions">
              <button className="popup-edit-btn" onClick={handleUpdate}>Save changes</button>
              <button
                className="admin-btn"
                onClick={() => { setIsEditing(false); displayError(""); }}>
                Cancel
              </button>
            </div>
          </>
        )}
      </div>
    </div>
  );
}