import { useState } from "react";
import { useAuth } from "../AuthContext";
import { useNavigate } from "react-router-dom";
import "../styles/loginpage.css";

export default function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await login(email, password);
      navigate("/"); // RoleRouter will redirect to the right dashboard
    } catch (err) {
      setError(err.message);
    }
  };

  return (
    <div className="login-page">
      <div className="login-card">
        <h2>Welcome back</h2>
        <p>Sign in to your account</p>

        {error && <div className="login-error">{error}</div>}

        <div className="login-field">
          <label>Email address</label>
          <input type="email" placeholder="you@example.com"
            value={email} onChange={e => setEmail(e.target.value)} />
        </div>

        <div className="login-field">
          <label>Password</label>
          <input type="password" placeholder="••••••••"
            value={password} onChange={e => setPassword(e.target.value)} />
          {/* <a className="login-forgot" href="#">Forgot password?</a> */}
        </div>

        <button className="login-btn" onClick={handleSubmit}>Sign in</button>

      </div>
    </div>
  );

}