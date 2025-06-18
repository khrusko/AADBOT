import { useState } from "react";
import api from "../config/api";
import "./PhotoUploadPage.css";

function PhotoUploadPage() {
  const [file, setFile] = useState(null);
  const [description, setDescription] = useState("");
  const [hashtags, setHashtags] = useState("");
  const [format, setFormat] = useState("jpg");
  const [resize, setResize] = useState("");
  const [message, setMessage] = useState("");

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!file) {
      setMessage("Please select a file.");
      return;
    }

    const formData = new FormData();
    formData.append("file", file);
    formData.append("description", description);
    formData.append("hashtags", hashtags);
    formData.append("format", format);
    if (resize) formData.append("resize", resize);

    try {
      const response = await fetch(api.photo.upload, {
        method: "POST",
        body: formData,
        credentials: "include"
      });

      if (response.ok) {
        setMessage("Photo uploaded successfully.");
        setFile(null);
        setDescription("");
        setHashtags("");
        setResize("");
        setFormat("jpg");
      } else {
        const err = await response.text();
        setMessage("Upload failed: " + err);
      }
    } catch (err) {
      setMessage("Upload error: " + err.message);
    }
  };

  return (
    <div className="upload-container">
      <h1>Upload a Photo</h1>

      <form className="upload-form" onSubmit={handleSubmit}>
        <label>Image File</label>
        <input
          type="file"
          accept="image/*"
          onChange={(e) => setFile(e.target.files[0])}
        />

        <label>Description</label>
        <input
          type="text"
          value={description}
          placeholder="Short description"
          onChange={(e) => setDescription(e.target.value)}
        />

        <label>Hashtags (comma-separated)</label>
        <input
          type="text"
          value={hashtags}
          placeholder="#nature,#sunset"
          onChange={(e) => setHashtags(e.target.value)}
        />

        <label>Format</label>
        <select value={format} onChange={(e) => setFormat(e.target.value)}>
          <option value="jpg">JPG</option>
          <option value="png">PNG</option>
          <option value="bmp">BMP</option>
        </select>

        <label>Resize Width (optional)</label>
        <input
          type="number"
          value={resize}
          placeholder="e.g. 800"
          onChange={(e) => setResize(e.target.value)}
        />

        <button type="submit">Upload</button>
      </form>

      {message && <p className="status-msg">{message}</p>}
    </div>
  );
}

export default PhotoUploadPage;
