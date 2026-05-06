export default function FRAPopup({ fra, onClose, onEdit, onDelete }) {
  return (
    <div className="popup-overlay" onClick={onClose}>
      <div className="popup-card" onClick={e => e.stopPropagation()}>
        <div className="popup-header">
          <h2>{fra.name}</h2>
          <button className="popup-close" onClick={onClose}>✕</button>
        </div>

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
          <button className="popup-edit-btn" onClick={onEdit}>Edit activity</button>
          <button className="popup-delete-btn" onClick={onDelete}>Delete</button>
        </div>
      </div>
    </div>
  );
}