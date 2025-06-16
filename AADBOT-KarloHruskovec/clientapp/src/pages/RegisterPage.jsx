import { useState } from "react";
import { useNavigate } from "react-router-dom";
import api from "../config/api";
import "./LoginPage.css"; // reuse same styling

function RegisterPage() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [selectedPackage, setSelectedPackage] = useState("FREE");
  const [message, setMessage] = useState("");
  const [errors, setErrors] = useState({});
  const navigate = useNavigate();

  const validate = () => {
    const errs = {};
    if (!email) errs.email = "Email is required.";
    else if (!/\S+@\S+\.\S+/.test(email)) errs.email = "Invalid email format.";
    if (!password || password.length < 6)
      errs.password = "Password must be at least 6 characters.";
    return errs;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    const validationErrors = validate();
    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return;
    }

    setErrors({});
    try {
      const res = await fetch(api.auth.register, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify({ email, password, package: selectedPackage })
      });

      if (res.ok) {
        setMessage("Registration successful.");
        setTimeout(() => navigate("/login"), 1000);
      } else {
        const err = await res.text();
        setMessage("Registration failed: " + err);
      }
    } catch (err) {
      setMessage("Error: " + err.message);
    }
  };

  return (
    <div className="login-container">
      <h1>Register</h1>

      <form className="login-form" onSubmit={handleSubmit}>
        <label>Email</label>
        <input
          type="email"
          value={email}
          placeholder="you@example.com"
          onChange={(e) => setEmail(e.target.value)}
        />
        {errors.email && <span className="error">{errors.email}</span>}

        <label>Password</label>
        <input
          type="password"
          value={password}
          placeholder="Minimum 6 characters"
          onChange={(e) => setPassword(e.target.value)}
        />
        {errors.password && <span className="error">{errors.password}</span>}

        <label>Select Package</label>
        <select
          value={selectedPackage}
          onChange={(e) => setSelectedPackage(e.target.value)}
        >
          <option value="FREE">FREE</option>
          <option value="PRO">PRO</option>
          <option value="GOLD">GOLD</option>
        </select>

        <br /><br />
        <button type="submit" className="primary-btn">Register</button>
      </form>

      {message && <p className="status-msg">{message}</p>}
    </div>
  );
}

export default RegisterPage;
