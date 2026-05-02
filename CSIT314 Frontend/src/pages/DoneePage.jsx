import { useState, useEffect } from "react";
import { useAuth } from "../AuthContext";
import { useNavigate } from "react-router-dom";
import FRAListTable from "../components/donee/FRAListTable";
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
  const [error, setError] = useState("");

  useEffect(() => {
    fetchAllFRAs();
  }, []);

  useEffect(() => {
    if (activeTab === "favourites") fetchFavourites();
    if (activeTab === "history") fetchDonationHistory();
  }, [activeTab]);

  const fetchAllFRAs = async () => {
    setError("");
    const res = await fetch("/api/ViewAllFundraiser", { credentials: "include" });
    if (res.status === 404) { setFras([]); return; }
    if (!res.ok) { setError("Failed to load activities"); return; }
    setFras(await res.json());
  };

  const fetchFavourites = async () => {
    setError("");
    const res = await fetch("/api/ViewFundraiserFavourites", { credentials: "include" });
    if (res.status === 404) { setFavourites([]); return; }
    if (!res.ok) { setError("Failed to load favourites"); return; }
    setFavourites(await res.json());
  };

  const fetchDonationHistory = async () => {
    setError("");
    const res = await fetch("/api/ViewDonationHistory", { credentials: "include" });
    if (!res.ok) { setDonations([]); return; }
    setDonations(await res.json());
  };

  const handleSelectFRA = async (fra) => {
    // fetch full details to increment view count
    const res = await fetch(`/api/ViewOneFundraiser?fraId=${fra.id}`, {
      credentials: "include"
    });
    if (res.ok) {
      const data = await res.json();
      setSelectedFRA(data);
    } else {
      setSelectedFRA(fra); // fallback to existing data
    }
  };

  const handleUnfavourite = async (fraId) => {
    setError("");
    const res = await fetch("/api/UnfavouriteFundraiser", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify({ fraId }),
    });
    if (!res.ok) { setError(await res.text()); return; }
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

        {activeTab === "browse" && (
          <FRAListTable
            fras={fras}
            search={browseSearch}
            setSearch={setBrowseSearch}
            onSelect={handleSelectFRA}
          />
        )}

        {activeTab === "favourites" && (
          <FavouritesTable
            favourites={favourites}
            search={favSearch}
            setSearch={setFavSearch}
            onSelect={handleSelectFRA}
            onUnfavourite={handleUnfavourite}
          />
        )}

        {activeTab === "history" && (
          <DonationHistoryTable
            donations={donations}
            search={donationSearch}
            setSearch={setDonationSearch}
          />
        )}

        {/* Popup renders on top of any tab */}
        {selectedFRA && (
          <FRADetailPopup
            fra={selectedFRA}
            onClose={() => setSelectedFRA(null)}
            onFavourited={() => {
              if (activeTab === "favourites") fetchFavourites();
            }}
          />
        )}
      </main>
    </div>
  );
}