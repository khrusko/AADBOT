import { useEffect, useState } from "react";
import api from "../config/api";
import "./AdminPage.css";

function AdminPage() {
  const [users, setUsers] = useState([]);
  const [logs, setLogs] = useState([]);
  const [photos, setPhotos] = useState([]);
  const [message, setMessage] = useState("");

  useEffect(() => {
    fetchAll();
  }, []);

  const fetchAll = async () => {
    try {
      const [userRes, logRes, photoRes] = await Promise.all([
        fetch(api.admin.users, { credentials: "include" }),
        fetch(api.admin.logs, { credentials: "include" }),
        fetch(api.admin.photos, { credentials: "include" })
      ]);

      setUsers(await userRes.json());
      setLogs(await logRes.json());
      setPhotos(await photoRes.json());
    } catch {
      setMessage("Failed to load admin data.");
    }
  };

  const handlePackageChange = async (userId, newPackage) => {
    const res = await fetch(api.admin.changeUserPackage(userId), {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify(newPackage)
    });
    if (res.ok) {
      setMessage("Package updated.");
      fetchAll();
    } else {
      setMessage("Failed to update package.");
    }
  };

  const handleDeletePhoto = async (photoId) => {
    const res = await fetch(api.admin.deletePhoto(photoId), {
      method: "DELETE",
      credentials: "include"
    });
    if (res.ok) {
      setMessage("Photo deleted.");
      fetchAll();
    } else {
      setMessage("Failed to delete photo.");
    }
  };

  return (
    <div className="admin-container">
      <h1>Admin Dashboard</h1>
      {message && <p className="status-msg">{message}</p>}

      <div className="admin-section">
        <h2>Users</h2>
        {users.map(user => (
          <div key={user.id} className="admin-card">
            <strong>{user.email}</strong> — Package: {user.package}<br />
            <select
              defaultValue={user.package}
              onChange={(e) => handlePackageChange(user.id, e.target.value)}
            >
              <option value="FREE">FREE</option>
              <option value="PRO">PRO</option>
              <option value="GOLD">GOLD</option>
            </select>
          </div>
        ))}
      </div>

      <div className="admin-section">
        <h2>Photos</h2>
        {photos.map(photo => (
          <div key={photo.id} className="admin-card">
            <p><strong>{photo.author}</strong>: {photo.description}</p>
            <button onClick={() => handleDeletePhoto(photo.id)}>Delete</button>
          </div>
        ))}
      </div>

      <div className="admin-section">
        <h2>Recent Logs</h2>
        <div className="log-list">
          {logs.map(log => (
            <div key={log.id} className="log-entry">
              <p><strong>{log.timestamp}</strong> — {log.userId || "Anonymous"} — {log.action}</p>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}

export default AdminPage;
