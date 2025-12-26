import { useState } from "react";
import { InputText } from "primereact/inputtext";
import { Password } from "primereact/password";
import { Button } from "primereact/button";
import { useNavigate } from "react-router-dom";
import { register } from "../api/auth.api";
import "../css/auth-frame.css";

export default function RegisterPage({ onRegistered }) {
  const [userName, setUserName] = useState("");
  const [email, setEmail] = useState("");
  const [pass, setPass] = useState("");
  const [confirm, setConfirm] = useState("");
  const [loading, setLoading] = useState(false);

  const nav = useNavigate();

  async function submit() {
    if (pass !== confirm) {
      alert("Mật khẩu nhập lại không khớp");
      return;
    }

    try {
      setLoading(true);
      const res = await register({
        userName: userName.trim(),
        email: email.trim(),
        password: pass,
      });

      // nếu BE trả token giống login thì lưu luôn
      if (res?.data?.accessToken) {
        localStorage.setItem("access_token", res.data.accessToken);
      }
      if (res?.data?.refreshToken) {
        localStorage.setItem("refresh_token", res.data.refreshToken);
      }

      if (onRegistered) onRegistered();
      else nav("/"); // hoặc nav("/feed") tùy app bạn
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="fb-auth-frame">
      <div className="fb-auth-card">
        <h2 className="m-0 mb-3 text-center">Create account</h2>

        <div className="p-float-label mb-4">
          <InputText
            id="username"
            value={userName}
            onChange={(e) => setUserName(e.target.value)}
            className="w-full"
          />
          <label htmlFor="username">Username</label>
        </div>

        <div className="p-float-label mb-4">
          <InputText
            id="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            className="w-full"
          />
          <label htmlFor="email">Email</label>
        </div>

        <div className="p-float-label mb-4">
          <Password
            id="pass"
            value={pass}
            onChange={(e) => setPass(e.target.value)}
            feedback={false}
            toggleMask
            className="w-full"
            inputClassName="w-full"
          />
          <label htmlFor="pass">Password</label>
        </div>

        <div className="p-float-label mb-4">
          <Password
            id="confirm"
            value={confirm}
            onChange={(e) => setConfirm(e.target.value)}
            feedback={false}
            toggleMask
            className="w-full"
            inputClassName="w-full"
          />
          <label htmlFor="confirm">Confirm password</label>
        </div>

        <Button
          label={loading ? "Creating..." : "Create account"}
          className="w-full fb-login-btn"
          onClick={submit}
          disabled={
            loading ||
            !userName.trim() ||
            !email.trim() ||
            !pass ||
            !confirm
          }
        />

        <Button
          label="Back to login"
          className="w-full p-button-outlined mt-3 fb-register-btn"
          onClick={() => nav("/login")}
          type="button"
        />
      </div>
    </div>
  );
}
