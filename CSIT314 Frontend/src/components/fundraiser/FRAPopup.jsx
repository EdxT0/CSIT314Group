import { useState } from "react";

export default function FRAPopup({ fra, onClose, onDelete, onSuccess, readOnly }) {
  const [isEditing, setIsEditing] = useState(fra.startEditing ?? false);  
  const [error, displayError] = useState("");
  const [form, setForm] = useState({
    Id: fra.id,
    Name: fra.name ?? "",
    Description: fra.description ?? "",
    DeadlineInString: "",
    Status: fra.status,
    AmtRequested: fra.amtRequested ?? "",
    FraCategoryId: fra.fraCategoryId ?? "",
  });

  const handleUpdate = async () => {
    displayError("");
    const payload = {Id: form.Id,};
    
    if (form.Name.trim()) payload.Name = form.Name;
    if (form.Description.trim()) payload.Description = form.Description;
    if (form.DeadlineInString.trim()) payload.DeadlineInString = form.DeadlineInString;
    if (form.AmtRequested) payload.AmtRequested = parseFloat(form.AmtRequested);
    //if (form.FraCategoryId) payload.FraCategoryId = parseInt(form.FraCategoryId);
    if (form.Status !== null && form.Status !== undefined) payload.Status = form.Status;

    const res = await fetch("/api/UpdateFundraiser", {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify(payload),
    });
    const text = await res.text();
    if (!res.ok) { displayError(text); return; }
    await onSuccess?.("Activity updated successfully!");
  };

  return (
    <div className="popup-overlay" onClick={onClose}>
      <div className="popup-card" onClick={e => e.stopPropagation()}>
        <div className="popup-header">
          <h2>{isEditing ? "Edit activity" : fra.name}</h2>
          <button className="popup-close" onClick={onClose}>✕</button>
        </div>

        {error && <div className="form-error">{error}</div>}

        {!isEditing && (
          <>
            <div className="popup-row">
              <span className="popup-label">Category</span>
              <span className="popup-val">{fra.fraCategoryName}</span>
            </div>
            <div className="popup-row">
              <span className="popup-label">Description</span>
              <span className="popup-val">{fra.description}</span>
            </div>
            <div className="popup-row">
              <span className="popup-label">Goal</span>
              <span className="popup-val">${fra.amtRequested?.toLocaleString()}</span>
            </div>
            <div className="popup-row">
              <span className="popup-label">Donated</span>
              <span className="popup-val">${fra.amtDonated?.toLocaleString()}</span>
            </div>
            <div className="popup-row">
              <span className="popup-label">Deadline</span>
              <span className="popup-val">{fra.deadline}</span>
            </div>
            <div className="popup-row">
              <span className="popup-label">Views</span>
              <span className="popup-val">{fra.amtOfViews}</span>
            </div>
            <div className="popup-row">
              <span className="popup-label">Status</span>
              <span className="popup-val">
                <span className={`badge ${!fra.status ? "badge-active" : "badge-completed"}`}>
                  {fra.status ? "Completed" : "Active"}
                </span>
              </span>
            </div>

            <div className="popup-actions">
              {!readOnly && (                              
                <button className="popup-edit-btn" onClick={() => setIsEditing(true)}>
                  Edit
                </button>
              )}
              {!readOnly && onDelete && (                 
                <button className="popup-delete-btn" onClick={onDelete}>
                  Delete
                </button>
              )}
              {readOnly && (                              
                <button className="popup-edit-btn" onClick={onClose}>
                  Close
                </button>
              )}
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
              <input value={form.DeadlineInString}
                placeholder="Leave blank to keep current"
                onChange={e => setForm({ ...form, DeadlineInString: e.target.value })} />
            </div>
            <div className="form-field">
              <label>Goal amount ($)</label>
              <input type="number" value={form.AmtRequested}
                onChange={e => setForm({ ...form, AmtRequested: e.target.value })} />
            </div>
            <div className="form-field">
              <label>Status</label>
              <select value={form.Status ? "true" : "false"}
                onChange={e => setForm({ ...form, Status: e.target.value === "true" })}>
                <option value="false">Active</option>
                <option value="true">Completed</option>
              </select>
            </div>
            <div className="form-field">
              <label>Category ID</label>
              <input type="number" value={form.FraCategoryId}
                onChange={e => setForm({ ...form, FraCategoryId: e.target.value })} />
            </div>

            <div className="popup-actions">
              <button className="popup-edit-btn" onClick={handleUpdate}>Save changes</button>
              <button className="admin-btn" onClick={() => { setIsEditing(false); displayError(""); }}>
                Cancel
              </button>
            </div>
          </>
        )}      
        </div>
    </div>
  );
}