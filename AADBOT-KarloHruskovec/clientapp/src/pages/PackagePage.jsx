import { useEffect, useState } from "react";
import api from "../config/api";
import "./PackagePage.css";

function PackagePage() {
  const [status, setStatus] = useState(null);
  const [selectedPackage, setSelectedPackage] = useState("FREE");
  const [message, setMessage] = useState("");

  useEffect(() => {
    fetch(api.package.status, { credentials: "include" })
      .then((res) => res.json())
      .then((data) => {
        setStatus(data);
        setSelectedPackage(data.package);
      })
      .catch(() => setMessage("Failed to load package info."));
  }, []);

  const handleChangePackage = async () => {
    const response = await fetch(api.package.change, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify({ newPackage: selectedPackage }),
    });

    if (response.ok) {
      setMessage("Package changed successfully.");
    } else {
      const error = await response.text();
      setMessage("Failed to change package: " + error);
    }
  };

  if (!status) return <p>{message || "Loading..."}</p>;

  return (
    <div className="package-container">
      <h1>Your Package</h1>

      <div className="package-status">
        <p><strong>Current Package:</strong> {status.package}</p>
        <p><strong>Used Today:</strong> {(status.dailyUsed / (1024 * 1024)).toFixed(2)} MB</p>
        <p><strong>Daily Limit:</strong> {status.dailyLimit ? (status.dailyLimit / (1024 * 1024)).toFixed(0) + " MB" : "Unlimited"}</p>
        <p><strong>Can Change Today:</strong> {status.canChangePackage ? "Yes" : "No"}</p>
      </div>

      <div className="package-change">
        <label>Change to:</label>
        <select
          value={selectedPackage}
          onChange={(e) => setSelectedPackage(e.target.value)}
          disabled={!status.canChangePackage}
        >
          <option value="FREE">FREE</option>
          <option value="PRO">PRO</option>
          <option value="GOLD">GOLD</option>
        </select>

        <button
          onClick={handleChangePackage}
          disabled={!status.canChangePackage}
        >
          Change Package
        </button>

        {!status.canChangePackage && (
          <p className="pending-msg" style={{ marginTop: "1rem", color: "#777" }}>
            You've already changed your package today. You’ll be able to change it again tomorrow.
          </p>
        )}
      </div>

      {message && <p className="status-msg">{message}</p>}
    </div>
  );
}

export default PackagePage;
