import { useState, useEffect } from "react";
import { useAuth } from "../AuthContext";
import { useNavigate } from "react-router-dom";
import FRATable from "../components/fundraiser/FRATable";
import MyFRATable from "../components/fundraiser/MyFRATable";
import CreateFRAForm from "../components/fundraiser/CreateFRAForm";
import "../styles/adminpage.css";
import "../styles/fundraiserpage.css";

export default function FundraiserPage() {
  const { logout } = useAuth();
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState("myFRAs");

  const [fras, setFras] = useState([]);        
  const [myFras, setMyFras] = useState([]);       
  const [search, setSearch] = useState("");
  const [myFraSearch, setMyFraSearch] = useState("");  
  const [error, displayError] = useState("");

  useEffect(() => {
    fetchAllFRAs();
    fetchMyOwnFRAs();  
  }, []);

  const fetchAllFRAs = async () => {
    displayError("");
    const res = await fetch("/api/ViewAllFundraiser", { credentials: "include" });
    if (res.status === 404) { setFras([]); return; }
    if (!res.ok) { displayError("Failed to load activities"); return; }
    setFras(await res.json());
  };

  const fetchMyOwnFRAs = async () => {   
    const res = await fetch("/api/ViewMyFundraisers", { credentials: "include" });
    if (res.status === 404) { setMyFras([]); return; }
    if (!res.ok) { displayError("Failed to load my activities"); return; }
    setMyFras(await res.json());
  };

  const handleDelete = async (fraId) => {
    if (!window.confirm("Are you sure you want to delete this activity?")) return;
    displayError("");
    const res = await fetch(`/api/DeleteFundraiser?fundraiserId=${fraId}`, {
      method: "DELETE",
      credentials: "include",
    });
    if (!res.ok) { displayError(await res.text()); return; }
    fetchMyOwnFRAs();
  };

  const handleSearch = async () => {
    if (!search.trim()) { fetchAllFRAs(); return; }
    displayError("");
    setFras([]);
    const res = await fetch(`/api/SearchFundraiser?name=${encodeURIComponent(search)}`, {
      credentials: "include",
    });
    if (res.status === 404) { setFras([]); return; }
    if (!res.ok) { displayError(await res.text()); return; }
    const data = await res.json();
    setFras(Array.isArray(data) ? data : [data]);
  };

  const handleMyFRASearch = async () => {  
    if (!myFraSearch.trim()) { fetchMyOwnFRAs(); return; }
    displayError("");
    setMyFras([]);
    const res = await fetch(`/api/SearchFundraiser?name=${encodeURIComponent(myFraSearch)}`, {
      credentials: "include",
    });
    if (res.status === 404) { setMyFras([]); return; }
    if (!res.ok) { displayError(await res.text()); return; }
    const data = await res.json();
    setMyFras(Array.isArray(data) ? data : [data]);
  };

  const handleReset = () => {
    setSearch("");
    fetchAllFRAs();
  };

  const handleMyFRAReset = () => {       
    setMyFraSearch("");
    fetchMyOwnFRAs();
  };

  const handleLogout = async () => {
    await logout();
    navigate("/login");
  };

  return (
    <div className="admin-wrap">
      <aside className="admin-sidebar">
        <div className="sidebar-logo">Fundraiser</div>

        <div className="sidebar-section">Activities</div>
        <div className={`nav-item ${activeTab === "allFRAs" ? "active" : ""}`}
          onClick={() => { setActiveTab("allFRAs"); setSearch(""); }}>
          Browse all
        </div>
        <div className={`nav-item ${activeTab === "myFRAs" ? "active" : ""}`}
          onClick={() => { setActiveTab("myFRAs"); setMyFraSearch(""); }}>
          My activities
        </div>
        <div className={`nav-item ${activeTab === "createFRA" ? "active" : ""}`}
          onClick={() => setActiveTab("createFRA")}>
          Create activity
        </div>

        <div className="sidebar-section">History</div>
        <div className={`nav-item ${activeTab === "completed" ? "active" : ""}`}
          onClick={() => { setActiveTab("completed"); setMyFraSearch(""); }}>
          Completed
        </div>

        <div className="sidebar-bottom">
          <div className="logout-btn" onClick={handleLogout}>Log out</div>
        </div>
      </aside>

      <main className="admin-main">
        {error && <div className="form-error">{error}</div>}

        {activeTab === "allFRAs" && (
          <FRATable
            fras={fras}
            search={search}
            setSearch={setSearch}
            onSuccess={fetchAllFRAs}
            onSearch={handleSearch}
            onReset={handleReset}
          />
        )}

        {activeTab === "myFRAs" && (
          <MyFRATable
            fras={myFras}
            search={myFraSearch}
            setSearch={setMyFraSearch}
            onDelete={handleDelete}
            onSuccess={fetchMyOwnFRAs}
            onSearch={handleMyFRASearch}
            onReset={handleMyFRAReset}
          />
        )}

        {activeTab === "completed" && (
          <MyFRATable
            fras={myFras.filter(f => f.status === true || f.status === 1)}
            search={myFraSearch}
            setSearch={setMyFraSearch}
            onDelete={handleDelete}
            onSuccess={fetchMyOwnFRAs}
            onSearch={handleMyFRASearch}
            onReset={handleMyFRAReset}
          />
        )}

        {activeTab === "createFRA" && (
          <CreateFRAForm
            onSuccess={() => { setActiveTab("myFRAs"); fetchMyOwnFRAs(); }}
            onCancel={() => setActiveTab("myFRAs")}
          />
        )}
      </main>
    </div>
  );
}