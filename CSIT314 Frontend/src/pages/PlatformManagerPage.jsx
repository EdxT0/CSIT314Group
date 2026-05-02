import { useState, useEffect } from "react";
import { useAuth } from "../AuthContext";
import { useNavigate } from "react-router-dom";
import CategoryTable from "../components/platformmanager/CategoryTable";
import CreateCategoryForm from "../components/platformmanager/CreateCategoryForm";
import EditCategoryForm from "../components/platformmanager/EditCategoryForm";
import ReportsTab from "../components/platformmanager/ReportsTab";
import "../styles/adminpage.css";
import "../styles/fundraiserpage.css";

export default function PlatformManagerPage() {
  const { logout } = useAuth();
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState("categories");
  const [categories, setCategories] = useState([]);
  const [fras, setFras] = useState([]);
  const [search, setSearch] = useState("");
  const [editingCategory, setEditingCategory] = useState(null);
  const [error, setError] = useState("");

  useEffect(() => {
    fetchCategories();
    fetchFRAs();
  }, []);

  const fetchCategories = async () => {
    setError("");
    const res = await fetch("/api/ViewAllCategory", { credentials: "include" });
    if (res.status === 404) { setCategories([]); return; }
    if (!res.ok) { setError("Failed to load categories"); return; }
    setCategories(await res.json());
  };

  const fetchFRAs = async () => {
    const res = await fetch("/api/ViewAllFundraiser", { credentials: "include" });
    if (!res.ok) return;
    setFras(await res.json());
  };

  const handleDelete = async (id) => {
    if (!window.confirm("Are you sure you want to delete this category?")) return;
    setError("");
    const res = await fetch(`/api/DeleteCategory/${id}`, {
      method: "DELETE",
      credentials: "include",
    });
    if (!res.ok) { setError(await res.text()); return; }
    fetchCategories();
  };

  const handleLogout = async () => {
    await logout();
    navigate("/login");
  };

  return (
    <div className="admin-wrap">
      <aside className="admin-sidebar">
        <div className="sidebar-logo">Platform manager</div>

        <div className="sidebar-section">Categories</div>
        <div className={`nav-item ${activeTab === "categories" ? "active" : ""}`}
          onClick={() => { setActiveTab("categories"); setSearch(""); }}>
          View categories
        </div>
        <div className={`nav-item ${activeTab === "createCategory" ? "active" : ""}`}
          onClick={() => setActiveTab("createCategory")}>
          Create category
        </div>

        <div className="sidebar-section">Reports</div>
        <div className={`nav-item ${activeTab === "reports" ? "active" : ""}`}
          onClick={() => setActiveTab("reports")}>
          Activity reports
        </div>

        <div className="sidebar-bottom">
          <div className="logout-btn" onClick={handleLogout}>Log out</div>
        </div>
      </aside>

      <main className="admin-main">
        {error && <div className="form-error">{error}</div>}

        {activeTab === "categories" && (
          <CategoryTable
            categories={categories}
            search={search}
            setSearch={setSearch}
            onEdit={(c) => { setEditingCategory(c); setActiveTab("editCategory"); }}
            onDelete={handleDelete}
          />
        )}

        {activeTab === "createCategory" && (
          <CreateCategoryForm
            onSuccess={() => { setActiveTab("categories"); fetchCategories(); }}
            onCancel={() => setActiveTab("categories")}
          />
        )}

        {activeTab === "editCategory" && editingCategory && (
          <EditCategoryForm
            category={editingCategory}
            onSuccess={() => { setActiveTab("categories"); fetchCategories(); }}
            onCancel={() => setActiveTab("categories")}
          />
        )}

        {activeTab === "reports" && (
          <ReportsTab fras={fras} />
        )}
      </main>
    </div>
  );
}