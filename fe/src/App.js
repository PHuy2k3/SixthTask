import { useEffect, useState } from "react";
import AppRoutes from "./routes/AppRoutes";
import { refresh } from "./api/auth.api";

export default function App() {
  const [auth, setAuth] = useState(null); // null = đang kiểm tra

  useEffect(() => {
    const access = localStorage.getItem("access_token");
    const ref = localStorage.getItem("refresh_token");

    if (access) {
      setAuth(true);
      return;
    }

    if (ref) {
      refresh({ refreshToken: ref })
        .then(res => {
          localStorage.setItem("access_token", res.data.accessToken);
          localStorage.setItem("refresh_token", res.data.refreshToken);
          setAuth(true);
        })
        .catch(() => setAuth(false));
      return;
    }

    setAuth(false);
  }, []);

  const logout = () => {
    localStorage.removeItem("access_token");
    localStorage.removeItem("refresh_token");
    localStorage.removeItem("username");
    setAuth(false);
  };

  const onAuthed = () => setAuth(true);

  return <AppRoutes auth={auth} onAuthed={onAuthed} onLogout={logout} />;
}
