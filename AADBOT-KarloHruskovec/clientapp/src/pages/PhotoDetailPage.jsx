import { useContext, useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { AuthContext } from "../context/AuthContext";
import api from "../config/api";
import "./PhotoDetailPage.css";

function PhotoDetailPage() {
  const { id } = useParams();
  const { user } = useContext(AuthContext);
  const [photo, setPhoto] = useState(null);
  const [filters, setFilters] = useState({
    resizeWidth: "",
    sepia: false,
    blur: false,
    format: "jpg",
  });
  const [message, setMessage] = useState("");
  const [editFields, setEditFields] = useState({
    description: "",
    hashtags: "",
  });
  const [editing, setEditing] = useState(false);

  useEffect(() => {
    fetch(api.photo.latest)
      .then((res) => res.json())
      .then((data) => {
        const found = data.find((p) => p.id.toString() === id);
        if (found) {
          setPhoto(found);
          setEditFields({
            description: found.description || "",
            hashtags: found.hashtags || "",
          });
        } else setMessage("Photo not found.");
      })
      .catch(() => setMessage("Error loading photo."));
  }, [id]);

  const isOwnerOrAdmin = user?.email === photo?.author || user?.isAdmin;

  const handleSave = async () => {
    const response = await fetch(api.photo.edit(id), {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify({
        description: editFields.description,
        hashtags: editFields.hashtags,
      }),
    });

    if (response.ok) {
      setMessage("Photo updated successfully.");
      setPhoto((prev) => ({
        ...prev,
        description: editFields.description,
        hashtags: editFields.hashtags,
      }));
      setEditing(false);
    } else {
      const err = await response.text();
      setMessage("Update failed: " + err);
    }
  };

  const handleDownload = async () => {
    try {
      const response = await fetch(api.photo.download(id), {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify({
          resizeWidth: filters.resizeWidth
            ? parseInt(filters.resizeWidth)
            : null,
          sepia: filters.sepia,
          blur: filters.blur,
          format: filters.format,
        }),
      });

      if (!response.ok) {
        setMessage("Failed to download.");
        return;
      }

      const blob = await response.blob();
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement("a");
      a.href = url;
      a.download = `photo.${filters.format}`;
      a.click();
    } catch (err) {
      setMessage("Download error.");
    }
  };

  if (!photo)
    return <p className="status-msg">{message || "Loading photo..."}</p>;

  return (
    <div className="photo-detail-container">
      <h1>Photo Details</h1>

      <img
        className="photo-preview"
        src={`https://localhost:44300/images/${photo.fileName}`}
        alt={photo.description}
      />

      <div className="detail-two-columns">
        <div className="column edit-column">
          <h3>General information</h3>
          <label>Description:</label>
          <textarea
            value={editFields.description}
            onChange={(e) =>
              setEditFields((p) => ({ ...p, description: e.target.value }))
            }
            disabled={!isOwnerOrAdmin}
          />

          <label>Hashtags:</label>
          <textarea
            value={editFields.hashtags}
            onChange={(e) =>
              setEditFields((p) => ({ ...p, hashtags: e.target.value }))
            }
            disabled={!isOwnerOrAdmin}
          />

          <p>
            <strong>Author:</strong> {photo.author}
          </p>
          <p>
            <strong>Uploaded:</strong>{" "}
            {new Date(photo.uploadDate).toLocaleDateString()}
          </p>

          {isOwnerOrAdmin && (
            <button onClick={handleSave} className="primary-btn">
              Save Changes
            </button>
          )}
        </div>

        <div className="column download-column">
          <h3>Download with Filters</h3>

          <label>Resize Width (px)</label>
          <input
            type="number"
            value={filters.resizeWidth}
            onChange={(e) =>
              setFilters({ ...filters, resizeWidth: e.target.value })
            }
          />

          <div className="checkbox-group">
            <label className="checkbox-label">
              <input
                type="checkbox"
                checked={filters.sepia}
                onChange={(e) =>
                  setFilters({ ...filters, sepia: e.target.checked })
                }
              />
              Sepia
            </label>

            <label className="checkbox-label">
              <input
                type="checkbox"
                checked={filters.blur}
                onChange={(e) =>
                  setFilters({ ...filters, blur: e.target.checked })
                }
              />
              Blur
            </label>
          </div>

          <label>Format</label>
          <select
            value={filters.format}
            onChange={(e) => setFilters({ ...filters, format: e.target.value })}
          >
            <option value="jpg">JPG</option>
            <option value="png">PNG</option>
            <option value="bmp">BMP</option>
          </select>

          <button onClick={handleDownload} className="primary-btn">
            Download
          </button>
        </div>
      </div>

      {message && <p className="status-msg">{message}</p>}
    </div>
  );
}

export default PhotoDetailPage;
