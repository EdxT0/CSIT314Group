import { createContext, useContext, useState, useEffect } from "react";

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true); // ← expose loading too

  useEffect(() => {
  fetch("/api/auth/Me", { credentials: "include" })
    .then(res => res.ok ? res.json() : null)
    .then(data => setUser(data))
    .catch(() => setUser(null))
    .finally(() => setLoading(false));
}, []);

  const fetchMe = async () => {
    const meRes = await fetch("/api/auth/Me", { credentials: "include" });
    if (!meRes.ok) return null;         // ← catches 401 Unauthorized
    
    const text = await meRes.text();
    if (!text) return null;             // ← catches empty body
    
    return JSON.parse(text);
  };

  // Use it in both login and useEffect:
  const login = async (email, password) => {
    const res = await fetch(`/api/auth/Login?email=${encodeURIComponent(email)}`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify(password),
    });

    if (!res.ok) throw new Error("Login failed");

    const userData = await fetchMe();
    setUser(userData);
    return userData;
  };

  useEffect(() => {
    fetchMe()
      .then(data => setUser(data))
      .catch(() => setUser(null))
      .finally(() => setLoading(false));
  }, []);  
  
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