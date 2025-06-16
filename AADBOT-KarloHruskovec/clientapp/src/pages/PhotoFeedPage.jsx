import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import api from "../config/api";
import "./PhotoFeedPage.css";

function PhotoFeedPage() {
  const [photos, setPhotos] = useState([]);

  useEffect(() => {
    fetch(api.photo.latest)
      .then((res) => res.json())
      .then(setPhotos)
      .catch((err) => console.error("Failed to load photos:", err));
  }, []);

  return (
    <div className="feed-container">
      <h1>Latest Photos</h1>
      {photos.length === 0 && <p>No photos found.</p>}

      <div className="photo-grid">
        {photos.map((photo) => (
          <div key={photo.id} className="photo-card">
            <Link to={`/photo/${photo.id}`}>
              <img
                src={`/images/${photo.fileName}`}
                alt={photo.description}
                className="photo-thumbnail"
              />
            </Link>
            <div className="photo-meta">
              <p><strong>{photo.description}</strong></p>
              <p><span className="meta-label">By:</span> {photo.author}</p>
              <p><span className="meta-label">Tags:</span> {photo.hashtags}</p>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}

export default PhotoFeedPage;
