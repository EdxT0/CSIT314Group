import { useState, useEffect } from "react";
import { useAuth } from "../AuthContext";
import { useNavigate } from "react-router-dom";
import DoneeFRATable from "../components/donee/DoneeFRATable";
import FRADetailPopup from "../components/donee/FRADetailPopup";
import FavouritesTable from "../components/donee/FavouritesTable";
import DonationHistoryTable from "../components/donee/DonationHistoryTable";
import "../styles/adminpage.css";
import "../styles/fundraiserpage.css";

export default function DoneePage() {
  const { logout } = useAuth();
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState("browse");
  const [fras, setFras] = useState([]);
  const [favourites, setFavourites] = useState([]);
  const [donations, setDonations] = useState([]);
  const [selectedFRA, setSelectedFRA] = useState(null);
  const [browseSearch, setBrowseSearch] = useState("");
  const [favSearch, setFavSearch] = useState("");
  const [donationSearch, setDonationSearch] = useState("");
  const [error, displayError] = useState("");
  const [successMessage, setSuccessMessage] = useState("");
  const [favouriteIds, setFavouriteIds] = useState([]);

  useEffect(() => {
    fetchAllFRAs();
    fetchFavourites();
  }, []);

  useEffect(() => {
    if (activeTab === "favourites") fetchFavourites();
    if (activeTab === "history") fetchDonationHistory();
  }, [activeTab]);

  const fetchAllFRAs = async () => {
    displayError("");
    const res = await fetch("/api/ViewAllFundraiser", { credentials: "include" });
    if (res.status === 404) { setFras([]); return; }
    if (!res.ok) { displayError("Failed to load activities"); return; }
    setFras(await res.json());
  };

  const fetchFavourites = async () => {
    const res = await fetch("/api/ViewFundraiserFavourites", { credentials: "include" });
    if (res.status === 404) { setFavourites([]); setFavouriteIds([]); return; }
    if (!res.ok) return;
    const data = await res.json();
    setFavourites(data);
    setFavouriteIds(data.map(f => f.id));  // ← extract just the IDs
  };

  const fetchDonationHistory = async () => {
    displayError("");
    const res = await fetch("/api/ViewDonationHistory", { credentials: "include" });
    if (!res.ok) { displayError("Failed to load donation history"); return; }
    setDonations(await res.json());
  };

  const handleBrowseSearch = async () => {
    if (!browseSearch.trim()) { fetchAllFRAs(); return; }
    displayError("");
    setFras([]);
    const res = await fetch(`/api/SearchFundraiser?name=${encodeURIComponent(browseSearch)}`, {
      credentials: "include",                           // ← param is "name" not "fraName"
    });
    if (res.status === 404) { setFras([]); return; }
    if (!res.ok) { displayError(await res.text()); return; }
    const data = await res.json();
    setFras(Array.isArray(data) ? data : [data]);
  };

  const handleBrowseReset = () => {
    setBrowseSearch("");
    fetchAllFRAs();
  };

  const handleFavSearch = async () => {
    if (!favSearch.trim()) { fetchFavourites(); return; }
    displayError("");
    setFavourites([]);
    const res = await fetch(`/api/SearchFavourite?fraName=${encodeURIComponent(favSearch)}`, {
      credentials: "include",
    });
    if (!res.ok) { setFavourites([]); return; }
    const text = await res.text();
    try {
      const data = JSON.parse(text);
      setFavourites(Array.isArray(data) ? data : [data]);
    } catch {
      setFavourites([]);  // ← handles "No favourites found" plain string
    }
  };

  const handleFavReset = () => {
    setFavSearch("");
    fetchFavourites();
  };

  const handleDonationSearch = async () => {
    if (!donationSearch.trim()) { fetchDonationHistory(); return; }
    displayError("");
    setDonations([]);
    const res = await fetch(`/api/SearchDonationHistory?fraName=${encodeURIComponent(donationSearch)}`, {
      credentials: "include",                           // ← param is "fraName"
    });
    if (!res.ok) { setDonations([]); return; }
    const data = await res.json();
    setDonations(Array.isArray(data) ? data : [data]);
  };

  const handleDonationReset = () => {
    setDonationSearch("");
    fetchDonationHistory();
  };

  const handleSelectFRA = async (fra) => {
    const res = await fetch(`/api/ViewOneFundraiser?fraId=${fra.id}`, {
      credentials: "include"
    });
    if (res.ok) {
      const data = await res.json();
      setSelectedFRA(data);
    } else {
      setSelectedFRA(fra);
    }
  };

  const handleUnfavourite = async (fraId) => {
    displayError("");
    const res = await fetch("/api/UnfavouriteFundraiser", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify({ FraId: fraId }),  // ← capital F
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
        {error && <div className="form-error">{error}</div>}

        {successMessage && (
        <div style={{
          background: "#0f2e1a",
          border: "0.5px solid #1d9e75",
          borderRadius: "8px",
          padding: "10px 12px",
          fontSize: "13px",
          color: "#5dcaa5",
          marginBottom: "1rem"
        }}>
          {successMessage}
        </div>
        )}
      
        {activeTab === "browse" && (
          <DoneeFRATable
            fras={fras}
            search={browseSearch}
            setSearch={setBrowseSearch}
            favouriteIds={favouriteIds}   // ← add this
            onSearch={handleBrowseSearch}    // ← add
            onReset={handleBrowseReset}
            onSuccess={() => {           // ← add this
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
            favouriteIds={favouriteIds}   // ← add this
            onSearch={handleFavSearch}    // ← add
            onReset={handleFavReset}
            onSuccess={() => {           // ← add this
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
            favouriteIds={favouriteIds}   // ← add this
            onSearch={handleDonationSearch}    // ← add
            onReset={handleDonationReset}
            onSuccess={() => {       
              fetchDonationHistory();    // ← add this
              fetchAllFRAs();
              fetchFavourites();
            }}

          />
        )}

        {selectedFRA && (
          <FRADetailPopup
            fra={selectedFRA}
            onClose={() => setSelectedFRA(null)}
            isFavourited={favouriteIds.includes(selectedFRA.id)}
            onSuccess={() => {
              fetchAllFRAs();           // ← refetch after donate/favourite
              if (activeTab === "favourites") fetchFavourites();
              if (activeTab === "history") fetchDonationHistory();
            }}
          />
        )}
      </main>
    </div>
  );
}