import { useState } from "react";
import CategoryPopup from "./CategoryPopup";

export default function CategoryTable({ categories, search, setSearch, onEdit, onSearch, onReset, onDelete, onSuccess }) {
  const [selectedCategory, setSelectedCategory] = useState(null);

  return (
    <>
      <div className="admin-topbar">
        <h1>Fundraising categories</h1>
        <div style={{ display: "flex", gap: "8px" }}>
          <input
            className="admin-search"
            placeholder="Search categories..."
            value={search}
            onChange={e => setSearch(e.target.value)}
            onKeyDown={e => e.key === "Enter" && onSearch()}
          />
          <button className="admin-btn" onClick={onSearch}>Search</button>
          <button className="admin-btn" onClick={onReset}>Reset</button>
        </div>
      </div>

      <div className="admin-metrics">
        <div className="metric">
          <div className="metric-label">Total categories</div>
          <div className="metric-val">{categories.length}</div>
        </div>
      </div>

      <div className="admin-table-card">
        <table>
          <thead>
            <tr>
              <th>Name</th>
              <th>Description</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {categories.length === 0 && (
              <tr>
                <td colSpan={3} style={{ textAlign: "center", color: "#7a7d8a", padding: "2rem" }}>
                  No categories found
                </td>
              </tr>
            )}
            {categories.map(c => (
              <tr
                key={c.id}
                onClick={() => setSelectedCategory(c)}
                style={{ cursor: "pointer" }}
              >
                <td>{c.name}</td>
                <td>{c.description}</td>
                <td>
                  <button
                    className="action-btn"
                    onClick={e => {
                      e.stopPropagation();
                      setSelectedCategory({ ...c, startEditing: true });
                    }}>
                    Edit
                  </button>
                  <button
                    className="action-btn danger"
                    onClick={e => {
                      e.stopPropagation();
                      onDelete(c.id);
                    }}>
                    Delete
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {selectedCategory && (
        <CategoryPopup
          category={selectedCategory}
          onClose={async () => {
            await onSuccess?.();
            setSelectedCategory(null);
          }}
          onSuccess={async () => {
            await onSuccess?.();
          }}
        />
      )}
    </>
  );
}