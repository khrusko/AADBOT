import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import api from "../config/api";
import "./PhotoFeedPage.css";

function PhotoFeedPage() {
  const [photos, setPhotos] = useState([]);
  const [filters, setFilters] = useState({
    hashtags: "",
    author: "",
    minSize: "",
    maxSize: "",
    from: "",
    to: "",
  });

  const fetchPhotos = async (activeFilters) => {
    const hasFilters = Object.values(activeFilters).some((v) => v);
    const params = new URLSearchParams();

    if (activeFilters.hashtags)
      params.append("hashtag", activeFilters.hashtags);
    if (activeFilters.author) params.append("author", activeFilters.author);
    if (activeFilters.minSize)
      params.append("minSize", activeFilters.minSize * 1024); // KB → B
    if (activeFilters.maxSize)
      params.append("maxSize", activeFilters.maxSize * 1024); // KB → B
    if (activeFilters.from) params.append("from", activeFilters.from);
    if (activeFilters.to) params.append("to", activeFilters.to);

    const url = hasFilters
      ? `${api.photo.search}?${params.toString()}`
      : api.photo.latest;

    const res = await fetch(url);
    setPhotos(res.ok ? await res.json() : []);
  };

  useEffect(() => {
    const delay = setTimeout(() => {
      fetchPhotos(filters);
    }, 400);

    return () => clearTimeout(delay);
  }, [filters]);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFilters((prev) => ({ ...prev, [name]: value }));
  };

  const handleClearFilters = () => {
    const cleared = {
      hashtags: "",
      author: "",
      minSize: "",
      maxSize: "",
      from: "",
      to: "",
    };
    setFilters(cleared);
    fetchPhotos(cleared);
  };

  return (
    <div className="feed-container">
      <h1>Latest Photos</h1>

      <div className="photo-search-form">
        <input
          type="text"
          name="author"
          placeholder="Author"
          value={filters.author}
          onChange={handleInputChange}
        />
        <input
          type="number"
          name="minSize"
          placeholder="Min size (Kb)"
          value={filters.minSize}
          onChange={handleInputChange}
        />
        <input
          type="number"
          name="maxSize"
          placeholder="Max size (Kb)"
          value={filters.maxSize}
          onChange={handleInputChange}
        />
        <input
          type="date"
          name="from"
          value={filters.from}
          onChange={handleInputChange}
        />
        <input
          type="date"
          name="to"
          value={filters.to}
          onChange={handleInputChange}
        />
        <input
          type="text"
          name="hashtags"
          placeholder="Hashtags (e.g. nature, city, pet)"
          value={filters.hashtags}
          onChange={handleInputChange}
        />
        <button type="button" onClick={handleClearFilters}>
          Clear Filters
        </button>
      </div>

      {photos.length === 0 && <p>No photos found.</p>}
      <div className="photo-grid">
        {photos.map((photo, index) => (
          <div
            key={photo.id}
            className={`photo-card ${index >= 10 ? "compact-card" : ""}`}
          >
            <Link to={`/photo/${photo.id}`}>
              <img
                src={`https://localhost:44300/images/${photo.fileName}`}
                alt={photo.description}
                className="photo-thumbnail"
              />
            </Link>

            {index < 10 && (
              <div className="photo-meta">
                <p>
                  <strong>{photo.description}</strong>
                </p>
                <p>
                  <span className="meta-label">By:</span> {photo.author}
                </p>
                <p>
                  <span className="meta-label">Uploaded:</span>{" "}
                  {new Date(photo.uploadDate).toLocaleDateString()}
                </p>
                <p>
                  {photo.hashtags}
                </p>
              </div>
            )}
          </div>
        ))}
      </div>
    </div>
  );
}

export default PhotoFeedPage;
