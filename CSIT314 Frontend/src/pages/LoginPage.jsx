import { useState } from "react";
import { useNavigate } from "react-router-dom";

export default function LoginPage() {
  const [username, setUsername] = useState("");
  const navigate = useNavigate();

  function handleLogin() {
    const role = username.trim();

    if (role === "Admin") {
      navigate("/admin");
    } else if (role === "Donee") {
      navigate("/donee");
    } else if (role === "Fundraiser") {
      navigate("/fundraiser");
    } else if (role === "PlatformManager") {
      navigate("/platform-manager");
    } else {
      alert("Invalid demo username");
    }
  }

  return (
    <div style={styles.page}>
      <div style={styles.card}>
        <h2 style={styles.title}>Login</h2>

        <input
          type="text"
          placeholder="Enter role name"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
          style={styles.input}
        />

        <input
          type="password"
          placeholder="Password"
          style={styles.input}
        />

        <button onClick={handleLogin} style={styles.button}>
          Login
        </button>

        <p style={styles.help}>
          Demo usernames: Admin, Donee, Fundraiser, PlatformManager
        </p>
      </div>
    </div>
  );
}

const styles = {
  page: {
    height: "100vh",
    display: "flex",
    justifyContent: "center",
    alignItems: "center",
    backgroundColor: "#f3f4f6",
  },
  card: {
    width: "320px",
    padding: "25px",
    backgroundColor: "#fff",
    borderRadius: "10px",
    display: "flex",
    flexDirection: "column",
    gap: "12px",
    boxShadow: "0 4px 10px rgba(0,0,0,0.1)",
  },
  title: {
    textAlign: "center",
    margin: 0,
    color: "black",
  },
  input: {
    padding: "10px",
    borderRadius: "6px",
    border: "1px solid #ccc",
  },
  button: {
    padding: "10px",
    border: "none",
    borderRadius: "6px",
    backgroundColor: "#2563eb",
    color: "white",
    cursor: "pointer",
  },
  help: {
    fontSize: "13px",
    color: "#555",
    marginTop: "8px",
  },
};