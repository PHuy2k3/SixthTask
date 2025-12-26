import { useNavigate, useLocation } from "react-router-dom";
import AppHeader from "../components/AppHeader";
import "../css/home.css";

const menus = [
  { icon: "pi pi-home", label: "Home", key: "home", path: "/" },
  { icon: "pi pi-user", label: "Profile", key: "profile", path: "/profile" },
  { icon: "pi pi-users", label: "Friends", key: "friends", path: "/friends" },
  { icon: "pi pi-bell", label: "Notifications", key: "notifications", path: "/notifications" }
];

function resolveActive(pathname) {
  if (pathname.startsWith("/profile")) return "profile";
  if (pathname.startsWith("/friends")) return "friends";
  if (pathname.startsWith("/notifications")) return "notifications";
  return "home";
}

export default function HomeLayout({
  children,
  onLogout,
  friendSuggestions = [],
  activeMenu
}) {
  const nav = useNavigate();
  const loc = useLocation();
  const active = activeMenu || resolveActive(loc.pathname);

  return (
    <div className="home">
      <AppHeader onLogout={onLogout} />

      <div className="home-wrap">
        <div className="home-grid">
          {/* Left */}
          <aside className="home-left">
            <div className="card home-card sticky">
              <div className="home-card-title gradient-text">Menu</div>

              <div className="home-menu">
                {menus.map((item) => (
                  <button
                    key={item.key}
                    type="button"
                    className={`home-menu-item ${active === item.key ? "active" : ""}`}
                    onClick={() => nav(item.path)}
                  >
                    <i className={item.icon} />
                    {item.label}
                  </button>
                ))}
              </div>
            </div>
          </aside>

          {/* Center */}
          <main className="home-center">{children}</main>

          {/* Right */}
          <aside className="home-right">
            <div className="card home-card sticky">
              <div className="home-card-title gradient-text">Friend suggestions</div>

              <div className="home-suggest">
                {friendSuggestions.length === 0 ? (
                  <div className="text-600">No suggestions.</div>
                ) : (
                  friendSuggestions.map((friend, index) => (
                    <div key={index} className="home-suggest-row">
                      <div className="home-avatar gradient-avatar">
                        {friend.initial}
                      </div>
                      <div className="home-suggest-name">{friend.name}</div>
                      <button
                        type="button"
                        className="p-button p-component p-button-sm p-button-outlined p-button-rounded add-btn"
                      >
                        Add
                      </button>
                    </div>
                  ))
                )}
              </div>

              <div className="home-divider" />
              <div className="home-card-title gradient-text">Shortcuts</div>
              <div className="text-600 shortcut-text">Nhóm / Trang / Lưu</div>
            </div>
          </aside>
        </div>
      </div>
    </div>
  );
}
