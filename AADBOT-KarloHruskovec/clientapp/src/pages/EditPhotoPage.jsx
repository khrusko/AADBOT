import { useParams, useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import api from "../config/api";
import "./EditPhotoPage.css";

function EditPhotoPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [description, setDescription] = useState("");
  const [hashtags, setHashtags] = useState("");
  const [message, setMessage] = useState("");

  useEffect(() => {
    fetch(api.photo.latest)
      .then(res => res.json())
      .then(data => {
        const photo = data.find(p => p.id.toString() === id);
        if (!photo) {
          setMessage("Photo not found or not owned by you.");
          return;
        }
        setDescription(photo.description);
        setHashtags(photo.hashtags);
      })
      .catch(() => setMessage("Failed to load photo."));
  }, [id]);

  const handleSubmit = async (e) => {
    e.preventDefault();

    const response = await fetch(api.photo.edit(id), {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify({ description, hashtags })
    });

    if (response.ok) {
      setMessage("Photo updated.");
      setTimeout(() => navigate(`/photo/${id}`), 1000);
    } else {
      const error = await response.text();
      setMessage("Failed to update: " + error);
    }
  };

  return (
    <div className="edit-photo-container">
      <h1>Edit Photo</h1>

      {message && <p className="status-msg">{message}</p>}

      <form className="edit-photo-form" onSubmit={handleSubmit}>
        <label>Description</label>
        <input
          type="text"
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          placeholder="Describe the photo"
        />

        <label>Hashtags</label>
        <input
          type="text"
          value={hashtags}
          onChange={(e) => setHashtags(e.target.value)}
          placeholder="#sunset,#vacation"
        />

        <button type="submit" className="primary-btn">Save Changes</button>
      </form>
    </div>
  );
}

export default EditPhotoPage;
