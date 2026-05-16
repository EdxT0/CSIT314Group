import { useState, useEffect } from "react";
import { useAuth } from "../AuthContext";
import { useNavigate } from "react-router-dom";
import DoneeFRATable from "../components/donee/DoneeFRATable";
import FavouritesTable from "../components/donee/FavouritesTable";
import DonationHistoryTable from "../components/donee/DonationHistoryTable";
import "../styles/adminpage.css";

export default function DoneePage() {
  const { logout } = useAuth();
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState("browse");
  const [fras, setFras] = useState([]);
  const [favourites, displayFavourites] = useState([]);
  const [donations, displayDonations] = useState([]);
  const [browseSearch, setBrowseSearch] = useState("");
  const [favSearch, setFavSearch] = useState("");
  const [donationSearch, setDonationSearch] = useState("");
  const [errorMessage, setErrorMessage] = useState("");
  const [favouriteIds, setFavouriteIds] = useState([]);

  // Proper named functions for BCE diagram
  const displayError = (msg) => setErrorMessage(msg); // ← #23, #24, #31, #32 displayError()
  const clearErrorMessage = () => setErrorMessage("");
  const updateFRAs = (data) => setFras(data);
  const clearFRAs = () => setFras([]);
  const setFavourites = (data) => { displayFavourites(data); setFavouriteIds(data.map(f => f.id)); }; // ← #23, #24 setFavourites()
  const clearFavourites = () => { displayFavourites([]); setFavouriteIds([]); };
  const setDonations = (data) => displayDonations(data); // ← #31, #32 setDonations()
  const clearDonations = () => displayDonations([]);

  useEffect(() => {
    fetchAllFRAs();
    fetchFavourites();
  }, []);

  useEffect(() => {
    if (activeTab === "favourites") fetchFavourites(); // ← #24 call fetchFavourites() when switching to favourites tab
    if (activeTab === "history") fetchDonationHistory(); // ← #31 call fetchDonationHistory() when switching to history tab
  }, [activeTab]);

  const fetchAllFRAs = async () => {
    clearErrorMessage();
    const res = await fetch("/api/ViewAllFundraiser", { credentials: "include" });
    if (res.status === 404) { clearFRAs(); return; }
    if (!res.ok) { displayError("Failed to load activities"); return; }
    updateFRAs(await res.json());
  };

  const fetchFavourites = async () => { // ← #24 fetchFavourites()
    clearErrorMessage();
    const res = await fetch("/api/ViewFundraiserFavourites", { credentials: "include" });
    if (res.status === 404) { clearFavourites(); return; }
    if (!res.ok) { displayError("Failed to load favourites"); return; }
    setFavourites(await res.json());
  };

  const fetchDonationHistory = async () => { // ← #31 fetchDonationHistory()
    clearErrorMessage();
    const res = await fetch("/api/ViewDonationHistory", { credentials: "include" });
    if (!res.ok) { displayError("Failed to load donation history"); return; }
    setDonations(await res.json());
  };

  const handleBrowseSearch = async () => {
    if (!browseSearch.trim()) { fetchAllFRAs(); return; }
    clearErrorMessage();
    clearFRAs();
    const res = await fetch(`/api/SearchFundraiser?name=${encodeURIComponent(browseSearch)}`, {
      credentials: "include",
    });
    if (res.status === 404) { clearFRAs(); return; }
    if (!res.ok) { displayError(await res.text()); return; }
    const data = await res.json();
    updateFRAs(Array.isArray(data) ? data : [data]);
  };

  const handleBrowseReset = () => {
    setBrowseSearch("");
    fetchAllFRAs();
  };

  const handleFavSearch = async () => { // ← #23 handleFavSearch()
    if (!favSearch.trim()) { fetchFavourites(); return; }
    clearErrorMessage();
    clearFavourites();
    const res = await fetch(`/api/SearchFavourite?fraName=${encodeURIComponent(favSearch)}`, {
      credentials: "include",
    });
    if (!res.ok) { clearFavourites(); return; }
    const text = await res.text();
    try {
      const data = JSON.parse(text);
      setFavourites(Array.isArray(data) ? data : [data]);
    } catch {
      clearFavourites();
    }
  };

  const handleFavReset = () => {
    setFavSearch("");
    fetchFavourites();
  };

  const handleDonationSearch = async () => { // ← #32 handleDonationSearch()
    if (!donationSearch.trim()) { fetchDonationHistory(); return; }
    clearErrorMessage();
    clearDonations();
    const res = await fetch(`/api/SearchDonationHistory?fraName=${encodeURIComponent(donationSearch)}`, {
      credentials: "include",
    });
    if (!res.ok) { clearDonations(); return; }
    const data = await res.json();
    setDonations(Array.isArray(data) ? data : [data]);
  };

  const handleDonationReset = () => {
    setDonationSearch("");
    fetchDonationHistory();
  };

  const handleUnfavourite = async (fraId) => {
    clearErrorMessage();
    const res = await fetch("/api/UnfavouriteFundraiser", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify({ FraId: fraId }),
    });
    if (!res.ok) { displayError(await res.text()); return; }
    fetchFavourites();
  };

  const handleLogout = async () => {
    await logout();
    navigate("/login");
  };

  return (
    <div className="admin-wrap">
      <aside className="admin-sidebar">
        <div className="sidebar-logo">Donee</div>

        <div className="sidebar-section">Discover</div>
        <div className={`nav-item ${activeTab === "browse" ? "active" : ""}`}
          onClick={() => { setActiveTab("browse"); setBrowseSearch(""); }}>
          Browse activities
        </div>

        <div className="sidebar-section">My list</div>
        <div className={`nav-item ${activeTab === "favourites" ? "active" : ""}`}
          onClick={() => setActiveTab("favourites")}>
          Favourites
        </div>

        <div className="sidebar-section">History</div>
        <div className={`nav-item ${activeTab === "history" ? "active" : ""}`}
          onClick={() => setActiveTab("history")}>
          Donation history
        </div>

        <div className="sidebar-bottom">
          <div className="logout-btn" onClick={handleLogout}>Log out</div>
        </div>
      </aside>

      <main className="admin-main">
        {errorMessage && <div className="form-error">{errorMessage}</div>}

        {activeTab === "browse" && (
          <DoneeFRATable
            fras={fras}
            search={browseSearch}
            setSearch={setBrowseSearch}
            favouriteIds={favouriteIds}
            onSearch={handleBrowseSearch}
            onReset={handleBrowseReset}
            onSuccess={() => {
              fetchAllFRAs();
              fetchFavourites();
            }}
          />
        )}

        {activeTab === "favourites" && (
          <FavouritesTable
            favourites={favourites}
            search={favSearch}
            setSearch={setFavSearch}
            onUnfavourite={handleUnfavourite}
            favouriteIds={favouriteIds}
            onSearch={handleFavSearch} // ← #23 call handleFavSearch()
            onReset={handleFavReset}
            onSuccess={() => {
              fetchAllFRAs();
              fetchFavourites();
            }}
          />
        )}

        {activeTab === "history" && (
          <DonationHistoryTable
            donations={donations}
            search={donationSearch}
            setSearch={setDonationSearch}
            favouriteIds={favouriteIds}
            onSearch={handleDonationSearch} // ← #32 call handleDonationSearch()
            onReset={handleDonationReset}
            onSuccess={() => {
              fetchDonationHistory();  
              fetchAllFRAs();
              fetchFavourites();
            }}
          />
        )}
      </main>
    </div>
  );
}