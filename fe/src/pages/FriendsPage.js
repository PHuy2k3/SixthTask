import { useEffect, useState } from "react";
import HomeLayout from "./HomeLayout";
import { getFriends } from "../api/friends.api";
import { useNavigate } from "react-router-dom";
import "../css/friend-page.css"; // Import CSS file

export default function FriendsPage({ onLogout }) {
  const [friends, setFriends] = useState([]);
  const nav = useNavigate();

  async function load() {
    const res = await getFriends();
    setFriends(res.data);
  }

  useEffect(() => { load(); }, []);

  return (
    <HomeLayout onLogout={onLogout} activeMenu="friends">
      <div className="friends-list">
        <div className="friends-header">
          <h1>Friends</h1>
          <div className="friends-count">
            {friends.length} friends
          </div>
        </div>

        {friends.length === 0 && (
          <div className="pcard pcard-muted">
            You don't have any friends yet.
          </div>
        )}

       {friends.map(f => (
          <div key={f.id} className="pcard">
            <div className="pcard-head">
              <div className="pcard-avatar">
                {f.userName?.charAt(0).toUpperCase()}
              </div>

              <div className="pcard-meta">
                <div
                  className="pcard-name clickable"
                  onClick={() => nav(`/u/${f.id}`)}
                >
                  {f.userName}
                </div>
                <div className="pcard-sub">Friends</div>
              </div>

              {/* ✅ NÚT CHAT */}
              <button
                className="pcard-action"
                onClick={() => nav(`/chat/u/${f.id}`)}
              >
                💬 Chat
              </button>
            </div>
          </div>
        ))}
      </div>
    </HomeLayout>
  );
}
