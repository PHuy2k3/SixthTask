import { useState } from "react";
import { InputText } from "primereact/inputtext";
import { Password } from "primereact/password";
import { Button } from "primereact/button";
import { login } from "../api/auth.api";
import { useNavigate } from "react-router-dom";
import { decodeJwt } from "../utils/jwt";
import "../css/auth-frame.css";

export default function LoginPage({ onLogin }) {
  const [user, setUser] = useState("");
  const [pass, setPass] = useState("");
  const nav = useNavigate();

  async function submit() {
    const res = await login({ userNameOrEmail: user.trim(), password: pass });

    localStorage.setItem("access_token", res.data.accessToken);
    localStorage.setItem("refresh_token", res.data.refreshToken);

    const p = decodeJwt(res.data.accessToken);
    // sub thường là userId
    if (p?.sub) localStorage.setItem("me_userId", p.sub);
    // name / unique_name tùy bạn set claim ở BE
    const uname = p?.name || p?.unique_name || p?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"];
    if (uname) localStorage.setItem("me_userName", uname);

    onLogin();
  }

  return (
    <div className="fb-auth-frame">
      <div className="fb-auth-card">
        <h2 className="m-0 mb-3 text-center">Login</h2>

        <div className="p-float-label mb-4">
          <InputText
            id="user"
            value={user}
            onChange={(e) => setUser(e.target.value)}
            className="w-full"
          />
          <label htmlFor="user">Username or Email</label>
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

        <Button label="Login" className="w-full fb-login-btn" onClick={submit} />

        <Button
          label="Create new account"
          className="w-full p-button-outlined mt-3 fb-register-btn"
          onClick={() => nav("/register")}
        />
      </div>
    </div>
  );
}
