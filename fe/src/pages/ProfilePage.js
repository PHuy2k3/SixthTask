import { useEffect, useMemo, useState } from "react";
import { useParams, useLocation } from "react-router-dom";
import HomeLayout from "./HomeLayout";
import PostCard from "../components/PostCard";
import PostComposer from "../components/PostComposer";
import { getMyPosts, getPostsByUser } from "../api/posts.api";
import { getFriendStatus, sendFriendRequest, acceptFriendRequest } from "../api/friends.api";
import { Button } from "primereact/button";
import "../css/profile-page.css";

export default function ProfilePage({ onLogout }) {
  const { userId } = useParams();
  const loc = useLocation();

  const isMe = useMemo(() => loc.pathname === "/profile", [loc.pathname]);

  const [posts, setPosts] = useState([]);
  const [friendStatus, setFriendStatus] = useState("none");
  const [loadingFriend, setLoadingFriend] = useState(false);

  // Bạn chưa có API lấy profile user -> tạm dùng text
  const displayName = isMe ? "You" : "User";
  const handleName = isMe ? "@me" : `@${(userId || "").slice(0, 6)}`;

  async function load() {
    const res = isMe ? await getMyPosts(30) : await getPostsByUser(userId, 30);
    setPosts(res.data);

    if (!isMe && userId) {
      const st = await getFriendStatus(userId);
      setFriendStatus(st.data.status);
    }
  }

  useEffect(() => {
    load();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isMe, userId]);

  async function addFriend() {
    if (!userId) return;
    setLoadingFriend(true);
    try {
      await sendFriendRequest(userId);
      await load();
    } finally {
      setLoadingFriend(false);
    }
  }

  async function acceptFriend() {
    if (!userId) return;
    setLoadingFriend(true);
    try {
      await acceptFriendRequest(userId);
      await load();
    } finally {
      setLoadingFriend(false);
    }
  }

  const friendBtn = !isMe && (
    <div className="profile-actions">
      {friendStatus === "none" && (
        <Button
          label="Add friend"
          icon="pi pi-user-plus"
          onClick={addFriend}
          loading={loadingFriend}
          className="p-button-sm profile-btn-primary"
        />
      )}

      {friendStatus === "pending_out" && (
        <Button
          label="Request sent"
          icon="pi pi-clock"
          disabled
          className="p-button-sm"
        />
      )}

      {friendStatus === "pending_in" && (
        <Button
          label="Accept"
          icon="pi pi-check"
          onClick={acceptFriend}
          loading={loadingFriend}
          className="p-button-sm profile-btn-primary"
        />
      )}

      {friendStatus === "friends" && (
        <Button
          label="Friends"
          icon="pi pi-users"
          severity="success"
          outlined
          className="p-button-sm"
        />
      )}
    </div>
  );

  return (
    <HomeLayout onLogout={onLogout} activeMenu="profile">
      {/* ===== Top Profile Card ===== */}
      <div className="pcard profile-top">
        <div className="profile-header">
          <div className="profile-avatar">
            <span>{displayName.charAt(0).toUpperCase()}</span>
          </div>

          <div className="profile-info">
            <div className="profile-nameRow">
              <div className="profile-name">{displayName}</div>
              <div className="profile-badges">
                <span className="profile-pill">Active</span>
                <span className="profile-pill soft">Fb-lite</span>
              </div>
            </div>

            <div className="profile-handle">{handleName}</div>

            <div className="profile-stats">
              <div className="profile-stat">
                <div className="profile-statNum">{posts.length}</div>
                <div className="profile-statLbl">Posts</div>
              </div>
              <div className="profile-stat">
                <div className="profile-statNum">—</div>
                <div className="profile-statLbl">Friends</div>
              </div>
              <div className="profile-stat">
                <div className="profile-statNum">—</div>
                <div className="profile-statLbl">Photos</div>
              </div>
            </div>
          </div>

          {friendBtn}
        </div>

        <div className="profile-tabs">
          <button className="profile-tab active" type="button">
            <i className="pi pi-clone" /> Posts
          </button>
          <button className="profile-tab" type="button" disabled>
            <i className="pi pi-users" /> Friends
          </button>
          <button className="profile-tab" type="button" disabled>
            <i className="pi pi-images" /> Photos
          </button>
        </div>
      </div>

      {/* ===== Composer (chỉ mình) ===== */}
      {isMe && <PostComposer onCreated={load} />}

      {/* ===== Feed ===== */}
      {posts.length === 0 ? (
        <div className="pcard pcard-muted">
          No posts yet.
        </div>
      ) : (
        posts.map(p => <PostCard key={p.id} post={p} onUpdated={load} />)
      )}
    </HomeLayout>
  );
}
