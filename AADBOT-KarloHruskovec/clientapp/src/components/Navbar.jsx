import { useContext } from "react";
import { Link, useNavigate } from "react-router-dom";
import { AuthContext } from "../context/AuthContext";
import api from "../config/api";

function Navbar() {
  const { user, logoutUser } = useContext(AuthContext);
  const navigate = useNavigate();

  const handleLogout = async () => {
    await fetch(api.auth.logout, {
      method: "POST",
      credentials: "include"
    });
    logoutUser();
    navigate("/");
  };

  const getRoleLabel = () => {
    if (!user) return "Anonymous";
    if (user.isAdmin) return "Admin";
    return "Registered";
  };

  return (
    <nav style={{
      padding: "1rem",
      borderBottom: "1px solid #ddd",
      display: "flex",
      justifyContent: "space-between",
      alignItems: "center"
    }}>
      <div>
        <Link to="/" style={{ marginRight: "1rem" }}>Photos</Link>
        {user && <Link to="/upload" style={{ marginRight: "1rem" }}>Upload</Link>}
        {user && <Link to="/package" style={{ marginRight: "1rem" }}>My Package</Link>}
        {user?.isAdmin && <Link to="/admin" style={{ marginRight: "1rem" }}>Admin</Link>}
      </div>

      <div style={{ fontWeight: "bold" }}>
        {getRoleLabel()}
      </div>

      <div>
        {user
          ? <>
              <span style={{ marginRight: "1rem" }}>👤 {user.email}</span>
              <button onClick={handleLogout}>Logout</button>
            </>
          : <>
          <Link to="/login">Login</Link> |
          <Link to="/register"> Register</Link>
          </>}
      </div>
    </nav>
  );
}

export default Navbar;
