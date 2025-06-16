import { useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import api from "../config/api";
import "./PhotoDetailPage.css";

function PhotoDetailPage() {
  const { id } = useParams();
  const [photo, setPhoto] = useState(null);
  const [filters, setFilters] = useState({
    resizeWidth: "",
    sepia: false,
    blur: false,
    format: "jpg"
  });
  const [message, setMessage] = useState("");

  useEffect(() => {
    fetch(api.photo.latest)
      .then((res) => res.json())
      .then((data) => {
        const found = data.find(p => p.id.toString() === id);
        if (found) setPhoto(found);
        else setMessage("Photo not found.");
      })
      .catch(() => setMessage("Error loading photo."));
  }, [id]);

  const handleDownload = async () => {
    try {
      const response = await fetch(api.photo.download(id), {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify({
          resizeWidth: filters.resizeWidth ? parseInt(filters.resizeWidth) : null,
          sepia: filters.sepia,
          blur: filters.blur,
          format: filters.format
        })
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

  if (!photo) return <p className="status-msg">{message || "Loading photo..."}</p>;

  return (
    <div className="photo-detail-container">
      <h1>Photo Detail</h1>

      <img
        className="photo-preview"
        src={`/images/${photo.fileName}`}
        alt={photo.description}
      />

      <div className="photo-info">
        <p><strong>Description:</strong> {photo.description}</p>
        <p><strong>Author:</strong> {photo.author}</p>
        <p><strong>Hashtags:</strong> {photo.hashtags}</p>
        <p><strong>Uploaded:</strong> {new Date(photo.uploadDate).toLocaleString()}</p>
      </div>

      <h3>Download with Filters</h3>
      <div className="filter-controls">
        <label>Resize Width (px)</label>
        <input
          type="number"
          value={filters.resizeWidth}
          onChange={(e) => setFilters({ ...filters, resizeWidth: e.target.value })}
        />

        <label>
          <input
            type="checkbox"
            checked={filters.sepia}
            onChange={(e) => setFilters({ ...filters, sepia: e.target.checked })}
          />
          Sepia
        </label>

        <label>
          <input
            type="checkbox"
            checked={filters.blur}
            onChange={(e) => setFilters({ ...filters, blur: e.target.checked })}
          />
          Blur
        </label>

        <label>Format</label>
        <select
          value={filters.format}
          onChange={(e) => setFilters({ ...filters, format: e.target.value })}
        >
          <option value="jpg">JPG</option>
          <option value="png">PNG</option>
          <option value="bmp">BMP</option>
        </select>

        <button onClick={handleDownload}>Download</button>
      </div>

      {message && <p className="status-msg">{message}</p>}
    </div>
  );
}

export default PhotoDetailPage;
