import { createContext, useEffect, useState } from "react";
import api from "../config/api";

export const AuthContext = createContext();

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [checked, setChecked] = useState(false);

  useEffect(() => {
    const saved = localStorage.getItem("user");
    if (saved) {
      setUser(JSON.parse(saved));
      setChecked(true);
    } else {
      fetch(api.auth.me, { credentials: "include" })
        .then((res) => res.ok ? res.json() : null)
        .then((data) => {
          if (data) {
            setUser(data);
            localStorage.setItem("user", JSON.stringify(data));
          }
          setChecked(true);
        });
    }
  }, []);

  const loginUser = (userData) => {
    setUser(userData);
    localStorage.setItem("user", JSON.stringify(userData));
  };

  const logoutUser = () => {
    setUser(null);
    localStorage.removeItem("user");
  };

  return (
    <AuthContext.Provider value={{ user, setUser: loginUser, logoutUser, checked }}>
      {children}
    </AuthContext.Provider>
  );
}
