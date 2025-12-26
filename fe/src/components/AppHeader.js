import { useMemo, useRef } from "react";
import { Menu } from "primereact/menu";
import { Button } from "primereact/button";
import "../css/header.css";

export default function AppHeader({ onLogout }) {
  const menuRef = useRef(null);

  const userName = localStorage.getItem("username") || "User";
  const initial = useMemo(() => (userName.trim()[0] || "U").toUpperCase(), [userName]);

  const items = [
    {
      label: "Profile",
      icon: "pi pi-user",
      command: () => (window.location.href = "/profile"),
    },
    { separator: true },
    {
      label: "Logout",
      icon: "pi pi-sign-out",
      command: () => onLogout?.(),
    },
  ];

  return (
    <header className="app-header">
      <div className="app-header-inner">
        <div className="app-logo" onClick={() => (window.location.href = "/")}>
          <span className="app-logo-badge">GG</span>
          <span className="app-logo-text">Social</span>
        </div>

        <div className="app-header-right">
          <Menu model={items} popup ref={menuRef} />

          <Button
            type="button"
            className="user-chip"
            onClick={(e) => menuRef.current.toggle(e)}
          >
            <span className="user-avatar">{initial}</span>
            <span className="user-name">{userName}</span>
            <i className="pi pi-chevron-down" />
          </Button>
        </div>
      </div>
    </header>
  );
}
