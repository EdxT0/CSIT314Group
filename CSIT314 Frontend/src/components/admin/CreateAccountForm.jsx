// components/admin/CreateAccountForm.jsx
import { useState } from "react";
import { authFetch } from "../../utils/api";

export function CreateAccountForm({ onSuccess }) {
  const [form, setForm] = useState({
    name: "", email: "", phoneNumber: "", password: "", profileName: "", isSuspended: false
  });
  const [error, setError] = useState("");

  const handleSubmit = async () => {
    const res = await authFetch("/api/CreateUserAccount", {
      method: "POST",
      body: JSON.stringify(form),
    });
    if (!res.ok) {
      const text = await res.text();
      setError(text || "Failed to create account");
      return;
    }
    onSuccess();
  };

  return (
    <div className="admin-form-card">
      <h2>Create account</h2>
      {error && <div className="form-error">{error}</div>}
      <div className="form-field"><label>Name</label>
        <input value={form.name} onChange={e => setForm({ ...form, name: e.target.value })} /></div>
      <div className="form-field"><label>Email</label>
        <input type="email" value={form.email} onChange={e => setForm({ ...form, email: e.target.value })} /></div>
      <div className="form-field"><label>Phone</label>
        <input value={form.phoneNumber} onChange={e => setForm({ ...form, phoneNumber: e.target.value })} /></div>
      <div className="form-field"><label>Password</label>
        <input type="password" value={form.password} onChange={e => setForm({ ...form, password: e.target.value })} /></div>
      <div className="form-field"><label>Profile</label>
        <input placeholder="e.g. donee, fundraiser, admin" value={form.profileName}
          onChange={e => setForm({ ...form, profileName: e.target.value })} /></div>
      <button className="submit-btn" onClick={handleSubmit}>Create account</button>
    </div>
  );
}