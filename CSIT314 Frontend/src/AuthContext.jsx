import { createContext, useContext, useState, useEffect } from "react";

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);

  useEffect(() => {
    const token = localStorage.getItem("token");
    if (token) {
      const payload = JSON.parse(atob(token.split(".")[1]));
      setUser({
        id: payload.nameid,
        name: payload.unique_name,
        role: payload.role,
      });
    }
  }, []);

const login = async (email, password) => {
  const res = await fetch("https://localhost:7039/api/auth/Login", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    credentials: "include",
    body: JSON.stringify({ email, password }),
  });

  const text = await res.text();

  if (!res.ok) {
    throw new Error(text || "Login failed");
  }

  setUser({ email });
};

  const logout = () => {
    localStorage.removeItem("token");
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export const useAuth = () => useContext(AuthContext);