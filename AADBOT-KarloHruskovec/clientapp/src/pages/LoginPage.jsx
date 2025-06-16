import { useState } from "react";
import { useNavigate } from "react-router-dom";
import api from "../config/api";
import "./LoginPage.css"; // optional if you split styles

function LoginPage() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [message, setMessage] = useState("");
  const [errors, setErrors] = useState({});
  const navigate = useNavigate();

  const validate = () => {
    const errs = {};
    if (!email) errs.email = "Email is required.";
    else if (!/\S+@\S+\.\S+/.test(email)) errs.email = "Invalid email format.";

    if (!password) errs.password = "Password is required.";
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
      const res = await fetch(api.auth.login, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify({ email, password })
      });

      if (res.ok) {
        setMessage("Login successful.");
        setTimeout(() => navigate("/"), 1000);
      } else {
        const err = await res.text();
        setMessage("Login failed: " + err);
      }
    } catch (err) {
      setMessage("Login error: " + err.message);
    }
  };

  return (
    <div className="login-container">
      <h1>Login</h1>

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
          placeholder="Enter your password"
          onChange={(e) => setPassword(e.target.value)}
        />
        {errors.password && <span className="error">{errors.password}</span>}

        <button type="submit" className="primary-btn">Login</button>
      </form>

      {message && <p className="status-msg">{message}</p>}

      <div className="oauth-section">
        <p>Or sign in with</p>
        <div className="oauth-buttons">
          <a href={api.auth.loginGoogle}>
            <img
              src="https://upload.wikimedia.org/wikipedia/commons/4/4a/Logo_2013_Google.png"
              alt="Google login"
              className="oauth-icon"
              title="Login with Google"
            />
          </a>
          <a href={api.auth.loginGitHub}>
            <img
              src="https://github.githubassets.com/images/modules/logos_page/GitHub-Mark.png"
              alt="GitHub login"
              className="oauth-icon"
              title="Login with GitHub"
            />
          </a>
        </div>
      </div>
    </div>
  );
}

export default LoginPage;
