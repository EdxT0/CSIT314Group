import { createContext, useContext, useState, useEffect } from "react";

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  const fetchMe = async () => {
    const res = await fetch("/api/Auth/Me", { credentials: "include" });
    if (!res.ok) return null;
    const text = await res.text();
    if (!text) return null;
    return JSON.parse(text);
  };

  useEffect(() => {   // ← single useEffect for session check on mount
    fetchMe()
      .then(data => setUser(data))
      .catch(() => setUser(null))
      .finally(() => setLoading(false));
  }, []);

  const login = async (email, password) => {
    const res = await fetch(`/api/Auth/Login?email=${encodeURIComponent(email)}`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify(password),
    });

    if (!res.ok) throw new Error("Invalid email or password");

    const userData = await fetchMe();
    if (!userData) throw new Error("Failed to get user details");
    setUser(userData);
  };

  const logout = async () => {
    await fetch("/api/Auth/Logout", { credentials: "include" });
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, login, logout, loading }}>
      {children}
    </AuthContext.Provider>
  );
}

export const useAuth = () => useContext(AuthContext);