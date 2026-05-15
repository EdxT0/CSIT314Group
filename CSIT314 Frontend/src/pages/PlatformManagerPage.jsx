import { useState, useEffect } from "react";
import { useAuth } from "../AuthContext";
import { useNavigate } from "react-router-dom";
import CategoryTable from "../components/platformmanager/CategoryTable";
import CreateCategoryForm from "../components/platformmanager/CreateCategoryForm";
import "../styles/adminpage.css";

export default function PlatformManagerPage() {
  const { logout } = useAuth();
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState("categories");
  const [categories, setCategories] = useState([]);
  const [fras, setFras] = useState([]);
  const [categorySearch, setCategorySearch] = useState("");
  const [error, displayError] = useState("");

  useEffect(() => {
    fetchCategories();
    fetchFRAs();
  }, []);

  const fetchCategories = async () => {
    displayError("");
    const res = await fetch("/api/ViewAllCategory", { credentials: "include" });
    if (res.status === 404) { setCategories([]); return; }
    if (!res.ok) { displayError("Failed to load categories"); return; }
    setCategories(await res.json());
  };

  const fetchFRAs = async () => {
    const res = await fetch("/api/ViewAllFundraiser", { credentials: "include" });
    if (!res.ok) return;
    setFras(await res.json());
  };

  const handleDelete = async (id) => {
    if (!window.confirm("Are you sure you want to delete this category?")) return;
    displayError("");
    const res = await fetch(`/api/DeleteCategory/${id}`, {
      method: "DELETE",
      credentials: "include",
    });
    if (!res.ok) { displayError(await res.text()); return; }
    fetchCategories();
  };

  const handleSearchCategories = async () => {
    if (!categorySearch.trim()) { fetchCategories(); return; }
    displayError("");
    setCategories([]);
    const res = await fetch(`/api/SearchCategory?keyword=${encodeURIComponent(categorySearch)}`, {
      credentials: "include",
    });
    if (res.status === 404) { setCategories([]); return; }
    if (!res.ok) { displayError(await res.text()); return; }
    const data = await res.json();
    setCategories(Array.isArray(data) ? data : [data]);
  };

  const handleResetCategories = () => {
    setCategorySearch("");
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
          onClick={() => setActiveTab("categories")}>
          View categories
        </div>
        <div className={`nav-item ${activeTab === "createCategory" ? "active" : ""}`}
          onClick={() => setActiveTab("createCategory")}>
          Create category
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
            search={categorySearch}
            setSearch={setCategorySearch}
            onDelete={handleDelete}
            onSuccess={fetchCategories}
            onSearch={handleSearchCategories}
            onReset={handleResetCategories}
          />
        )}

        {activeTab === "createCategory" && (
          <CreateCategoryForm
            onSuccess={fetchCategories}
            onCancel={() => setActiveTab("categories")}
          />
        )}
      </main>
    </div>
  );
}