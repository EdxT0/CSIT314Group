import { useState, useEffect } from "react";
import { useAuth } from "../AuthContext";
import { useNavigate } from "react-router-dom";
import FRATable from "../components/fundraiser/FRATable";
import FRAPopup from "../components/fundraiser/FRAPopup";
import CreateFRAForm from "../components/fundraiser/CreateFRAForm";
import EditFRAForm from "../components/fundraiser/EditFRAForm";
import "../styles/adminpage.css"; 
import "../styles/fundraiserpage.css";

export default function FundraiserPage() {
  const { logout } = useAuth();
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState("myFRAs");
  const [fras, setFras] = useState([]);
  const [search, setSearch] = useState("");
  const [selectedFRA, setSelectedFRA] = useState(null);
  const [editingFRA, setEditingFRA] = useState(null);
  const [error, setError] = useState("");

  useEffect(() => {
    fetchMyFRAs();
  }, []);

  const fetchMyFRAs = async () => {
    setError("");
    const res = await fetch("/api/ViewMyFundraisers", { credentials: "include" });
    if (res.status === 404) { setFras([]); return; }
    if (!res.ok) { setError("Failed to load activities"); return; }
    setFras(await res.json());
  };

  const handleDelete = async (fraId) => {
    if (!window.confirm("Are you sure you want to delete this activity?")) return;
    setError("");
    const res = await fetch(`/api/DeleteFundraiser?fundraiserId=${fraId}`, {
      method: "DELETE",
      credentials: "include",
    });
    if (!res.ok) { setError(await res.text()); return; }
    setSelectedFRA(null);
    fetchMyFRAs();
  };

  const handleLogout = async () => {
    await logout();
    navigate("/login");
  };

  // Completed FRAs filtered from the same list
  const completedFRAs = fras.filter(f => f.status === true || f.status === 1);
  const activeFRAs = fras.filter(f => f.status === false || f.status === 0);
  const displayFRAs = activeTab === "completed" ? completedFRAs : activeFRAs;

  return (
    <div className="admin-wrap">
      <aside className="admin-sidebar">
        <div className="sidebar-logo">Fundraiser</div>

        <div className="sidebar-section">Activities</div>
        <div className={`nav-item ${activeTab === "myFRAs" ? "active" : ""}`}
          onClick={() => { setActiveTab("myFRAs"); setSearch(""); }}>
          My activities
        </div>
        <div className={`nav-item ${activeTab === "createFRA" ? "active" : ""}`}
          onClick={() => setActiveTab("createFRA")}>
          Create activity
        </div>

        <div className="sidebar-section">History</div>
        <div className={`nav-item ${activeTab === "completed" ? "active" : ""}`}
          onClick={() => { setActiveTab("completed"); setSearch(""); }}>
          Completed
        </div>

        <div className="sidebar-bottom">
          <div className="logout-btn" onClick={handleLogout}>Log out</div>
        </div>
      </aside>

      <main className="admin-main">
        {error && <div className="form-error">{error}</div>}

        {(activeTab === "myFRAs" || activeTab === "completed") && (
          <FRATable
            fras={displayFRAs}
            search={search}
            setSearch={setSearch}
            onSelect={setSelectedFRA}
          />
        )}

        {activeTab === "createFRA" && (
          <CreateFRAForm
            onSuccess={() => { setActiveTab("myFRAs"); fetchMyFRAs(); }}
            onCancel={() => setActiveTab("myFRAs")}
          />
        )}

        {activeTab === "editFRA" && editingFRA && (
          <EditFRAForm
            fra={editingFRA}
            onSuccess={() => { setActiveTab("myFRAs"); fetchMyFRAs(); setSelectedFRA(null); }}
            onCancel={() => { setActiveTab("myFRAs"); setSelectedFRA(null); }}
          />
        )}

        {/* Popup — renders on top of any tab */}
        {selectedFRA && (
          <FRAPopup
            fra={selectedFRA}
            onClose={() => setSelectedFRA(null)}
            onEdit={() => {
              setEditingFRA(selectedFRA);
              setActiveTab("editFRA");
              setSelectedFRA(null);
            }}
            onDelete={() => handleDelete(selectedFRA.id)}
          />
        )}
      </main>
    </div>
  );
}