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

const styles = {
  page: {
    height: "100vh",
    display: "flex",
    justifyContent: "center",
    alignItems: "center",
    backgroundColor: "#f9fafb",
  },
  heading: {
    fontSize: "32px",
    fontWeight: "bold",
    color: "black",
  },
};