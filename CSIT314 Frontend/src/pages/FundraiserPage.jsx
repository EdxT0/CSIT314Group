import { useState, useEffect } from "react";
import { useAuth } from "../AuthContext";
import { useNavigate } from "react-router-dom";
import FRATable from "../components/fundraiser/FRATable";
import MyFRATable from "../components/fundraiser/MyFRATable";
import CreateFRAForm from "../components/fundraiser/CreateFRAForm";
import "../styles/adminpage.css";

export default function FundraiserPage() {
  const { logout } = useAuth();
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState("myFRAs");
  const [favouriteCounts, setFavouriteCounts] = useState({});
  const [fras, setFras] = useState([]);
  const [myFras, setMyFras] = useState([]);
  const [completedFras, setCompletedFras] = useState([]);
  const [search, setSearch] = useState("");
  const [myFraSearch, setMyFraSearch] = useState("");
  const [completedSearch, setCompletedSearch] = useState("");
  const [error, displayError] = useState("");

  useEffect(() => {
    fetchFRAs();
    fetchMyOwnFRAs();
  }, []);

  useEffect(() => {
    fetchCompleted();
  }, [myFras]);

  const fetchFRAs = async () => { // ← #16 fetch all FRAs for the browse tab
    displayError("");
    const res = await fetch("/api/ViewAllFundraiser", { credentials: "include" });
    if (res.status === 404) { setFras([]); displayError("No activities found"); return; }// ← #16 display error if fetch returns 404
    if (!res.ok) { displayError("Failed to load activities"); return; } // ← #16 display error if fetch fails
    const data = await res.json();
    setFras(data); // ← #16 set the full list of FRAs for the browse tab
    fetchFavouriteCounts(data);  
  };

  const fetchMyOwnFRAs = async () => {
    displayError("");
    const res = await fetch("/api/ViewMyFundraisers", { credentials: "include" });
    if (res.status === 404) { setMyFras([]); displayError("No activities found"); return; }
    if (!res.ok) { displayError("Failed to load my activities"); return; }
    const data = await res.json();
    setMyFras(data);
    fetchFavouriteCounts(data);  
  };

  const fetchCompleted = () => { // ← #29 fetch all FRAs then filter for the completed tab
    const completed = myFras.filter(f => f.status === true || f.status === 1); // ← #29 filtering
    if (completed.length === 0) { displayError("No completed activities found"); return; } // ← #29 if no completed activities, display error
    setCompletedFras(completed); // ← #29 set the list of completed FRAs
  };

  const fetchFavouriteCounts = async (fraList) => { // ← #28 fetch favourite counts for a given list of FRAs (used for both the browse and my activities tabs)
    displayError("");
    const counts = {};
    let hasError = false;
    await Promise.all(fraList.map(async (f) => {
      const res = await fetch(`/api/ViewAmtOfFavourites?fraId=${f.id}`, {
        credentials: "include",
      });
      if (res.ok) {
        const count = await res.json();
        counts[f.id] = count;
      } else {
        counts[f.id] = 0;
        hasError = true;
      }
    }));
    if (hasError) displayError("Failed to load some favourite counts"); // ← #28 display error if any of the favourite count fetches fail (but still display the counts that did load successfully)
    setFavouriteCounts(counts); // ← #28 set the favourite counts state after fetching counts for all FRAs in the list
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

  const handleSearchFRA = async () => { // ← #19 search function for the browse tab
    if (!search.trim()) { fetchFRAs(); return; }
    displayError("");
    setFras([]);
    const res = await fetch(`/api/SearchFundraiser?name=${encodeURIComponent(search)}`, {
      credentials: "include",
    });
    if (!res.ok) { displayError(await res.text()); return; }// ← #19 display error if search fails
    const data = await res.json();
    setFras(Array.isArray(data) ? data : [data]);// ← #19 set search results for the browse tab (handle both array and single object responses)
  };

  const handleMyFRASearch = async () => {  
    if (!myFraSearch.trim()) { fetchMyOwnFRAs(); return; }
    displayError("");
    setMyFras([]);
    const res = await fetch(`/api/SearchFundraiser?name=${encodeURIComponent(myFraSearch)}`, {
      credentials: "include",
    });
    if (!res.ok) { displayError(await res.text()); return; }
    const data = await res.json();
    setMyFras(Array.isArray(data) ? data : [data]);
  };

  const handleCompletedSearch = async () => { // ← #30 search function for the completed tab
    if (!completedSearch.trim()) { fetchCompleted(); return; }
    displayError("");
    setCompletedFras([]);
    const res = await fetch(`/api/SearchFundraiser?name=${encodeURIComponent(completedSearch)}`, {
      credentials: "include",
    });
    if (res.status === 404) { displayError("No activities found"); return; }  // ←  #30 display error if no activities found
    if (!res.ok) { displayError(await res.text()); return; }
    const data = await res.json();
    const all = Array.isArray(data) ? data : [data];
    const completed = all.filter(f => f.status === true || f.status === 1); // ← #30 filtering
    if (completed.length === 0) { displayError("No completed activities found"); return; }  // ←  #30 handle case where search returns results but none are completed
    setCompletedFras(completed); // ← #30 set search results for the completed tab
  };

  const handleReset = () => { // ← for the reset button to clear the search input and reset the browse tab to show all FRAs
    setSearch("");
    fetchFRAs();
  };

  const handleMyFRAReset = () => {  // ← for the reset button to clear the search input and reset the "My activities" tab to show all FRAs
    setMyFraSearch("");
    fetchMyOwnFRAs();
  };

  const handleCompletedReset = () => { // ← for the reset button to clear the search input and reset the "Completed" tab to show all completed FRAs
    setCompletedSearch("");
    fetchCompleted();
  };


  const handleLogout = async () => {  // ← for the logout button calls AuthContext's logout function and redirects to login page
    await logout();
    navigate("/login");
  };

{/*-----------------------------------------Render--------------------------------------------------------*/}

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
          onClick={() => { setActiveTab("completed"); setCompletedSearch(""); }}>
          Completed
        </div>

        <div className="sidebar-bottom">
          <div className="logout-btn" onClick={handleLogout}>Log out</div>
        </div>
      </aside>

{/* -----------------------------------------Main content area with conditional rendering based on activeTab--------------------------------------------------------*/}

      <main className="admin-main">
        {error && <div className="form-error">{error}</div>} 

        {activeTab === "allFRAs" && (
          <FRATable
            fras={fras}
            search={search}
            setSearch={setSearch}
            onSuccess={fetchFRAs}
            onSearch={handleSearchFRA}
            onReset={handleReset}
            favouriteCounts={favouriteCounts}           
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
            favouriteCounts={favouriteCounts}           
          />
        )}

        {activeTab === "completed" && (
          <MyFRATable
            fras={completedFras}
            search={completedSearch}
            setSearch={setCompletedSearch}
            onDelete={handleDelete}
            onSuccess={fetchMyOwnFRAs}
            onSearch={handleCompletedSearch}
            onReset={handleCompletedReset}
            favouriteCounts={favouriteCounts}           
          />
        )}

        {activeTab === "createFRA" && (
          <CreateFRAForm
            onSuccess={() => { fetchMyOwnFRAs(); fetchFRAs(); }}
            onCancel={() => setActiveTab("myFRAs")}
          />
        )}
      </main>
    </div>
  );
}