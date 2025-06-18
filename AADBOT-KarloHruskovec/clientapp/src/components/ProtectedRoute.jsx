import { useContext } from "react";
import { Navigate } from "react-router-dom";
import { AuthContext } from "../context/AuthContext";

function ProtectedRoute({ children, allowAdmin = false, allowUser = false }) {
  const { user, checked } = useContext(AuthContext);

  if (!checked) return <p>Loading...</p>;

  if (!user) return <Navigate to="/" replace />;

  const isAdmin = user?.isAdmin;

  if (allowAdmin && isAdmin) return children;
  if (allowUser && !isAdmin) return children;

  return <p style={{ padding: "2rem", fontWeight: "bold" }}>403 – Unauthorized</p>;
}

export default ProtectedRoute;
