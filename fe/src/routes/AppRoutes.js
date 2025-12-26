import { Routes, Route, Navigate } from "react-router-dom";
import FeedPage from "../pages/FeedPage";
import ProfilePage from "../pages/ProfilePage";
import UserProfilePage from "../pages/UserProfilePage";
import FriendsPage from "../pages/FriendsPage";
import ChatPage from "../pages/ChatPage";
import LoginPage from "../pages/LoginPage";
import RegisterPage from "../pages/RegisterPage";
import PrivateRoute from "./PrivateRoute";

export default function AppRoutes({ auth, onAuthed, onLogout }) {
  if (auth === null) return null;

  // Chưa login
  if (!auth) {
    return (
      <Routes>
        <Route path="/login" element={<LoginPage onLogin={onAuthed} />} />
        <Route path="/register" element={<RegisterPage onRegistered={onAuthed} />} />
        <Route path="*" element={<Navigate to="/login" replace />} />
      </Routes>
    );
  }

  // Đã login
  return (
    <Routes>
      <Route
        path="/"
        element={
          <PrivateRoute authed={auth}>
            <FeedPage onLogout={onLogout} />
          </PrivateRoute>
        }
      />

      <Route
        path="/profile"
        element={
          <PrivateRoute authed={auth}>
            <ProfilePage onLogout={onLogout} />
          </PrivateRoute>
        }
      />

      {/* ✅ xem trang cá nhân người khác */}
      <Route
        path="/u/:userId"
        element={
          <PrivateRoute authed={auth}>
            <UserProfilePage onLogout={onLogout} />
          </PrivateRoute>
        }
      />

      {/* ✅ friends */}
      <Route
        path="/friends"
        element={
          <PrivateRoute authed={auth}>
            <FriendsPage onLogout={onLogout} />
          </PrivateRoute>
        }
      />

      {/* ✅ chat */}
      <Route
        path="/chat"
        element={
          <PrivateRoute authed={auth}>
            <ChatPage onLogout={onLogout} />
          </PrivateRoute>
        }
      />
      <Route
        path="/chat/u/:userId"
        element={
          <PrivateRoute authed={auth}>
            <ChatPage onLogout={onLogout} />
          </PrivateRoute>
        }
      />

      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}
