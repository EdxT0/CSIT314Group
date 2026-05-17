import { useEffect, useState } from "react";
import { useAuth } from "../AuthContext";
import { useNavigate } from "react-router-dom";
import "../styles/adminpage.css";

export default function LoginPage() {
  const { login, user } = useAuth(); 
  const navigate = useNavigate();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [errorMessage, setErrorMessage] = useState("");

  const displayError = (msg) => setErrorMessage(msg);

  useEffect(() => {// ← useEffect handles navigation when user is set
    if (user) {
      switch (user.role) {
        case "admin":
          navigate("/admin", { replace: true });
          break;
        case "donee":
          navigate("/donee", { replace: true });
          break;
        case "fundraiser manager":
          navigate("/fundraiser", { replace: true });
          break;
        case "platform manager":
          navigate("/platform", { replace: true });
          break;
        default:
          navigate("/", { replace: true });
      }
    }
  }, [user]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await login(email, password);
    } catch (err) {
      displayError(err.message);
    }
  };
  
  return (
    <div className="login-page">
      <div className="login-card">
        <h2>Welcome back</h2>
        <p>Sign in to your account</p>

        {errorMessage && <div className="login-error">{errorMessage}</div>}

        <form onSubmit={handleSubmit}>
          <div className="login-field">
            <label>Email address</label>
            <input type="email" placeholder="you@example.com"
              value={email} onChange={e => setEmail(e.target.value)} />
          </div>

          <div className="login-field">
            <label>Password</label>
            <input type="password" placeholder="••••••••"
              value={password} onChange={e => setPassword(e.target.value)} />
          </div>

          <button type="submit" className="login-btn">Sign in</button>
        </form>
      </div>
    </div>
  );
}