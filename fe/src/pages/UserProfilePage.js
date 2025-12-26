import { useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import HomeLayout from "./HomeLayout";
import {
  getFriendStatus,
  sendFriendRequest,
  acceptFriendRequest
} from "../api/friends.api";
import { getPostsByUser } from "../api/posts.api";
import PostCard from "../components/PostCard";
import { Button } from "primereact/button";

export default function UserProfilePage({ onLogout }) {
  const { userId } = useParams();

  const [status, setStatus] = useState("none");
  const [posts, setPosts] = useState([]);
  const [loading, setLoading] = useState(true);

  async function loadAll() {
    if (!userId) return;

    setLoading(true);
    try {
      const [st, ps] = await Promise.all([
        getFriendStatus(userId),
        getPostsByUser(userId)
      ]);

      setStatus(st.data.status);
      setPosts(ps.data);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    loadAll();
  }, [userId]);

  async function addFriend() {
    await sendFriendRequest(userId);
    loadAll();
  }

  async function accept() {
    await acceptFriendRequest(userId);
    loadAll();
  }

  return (
    <HomeLayout onLogout={onLogout}>
      {/* ===== PROFILE HEADER ===== */}
      <div className="pcard">
        <div style={{ display: "flex", alignItems: "center", gap: 16 }}>
          {/* avatar chữ */}
          <div
            style={{
              width: 64,
              height: 64,
              borderRadius: "50%",
              background: "linear-gradient(135deg,#667eea,#764ba2)",
              color: "#fff",
              fontSize: 28,
              fontWeight: 900,
              display: "flex",
              alignItems: "center",
              justifyContent: "center"
            }}
          >
            {posts[0]?.authorUserName?.charAt(0).toUpperCase() || "U"}
          </div>

          <div style={{ flex: 1 }}>
            <div style={{ fontSize: 22, fontWeight: 900 }}>
              {posts[0]?.authorUserName || "User"}
            </div>
            <div style={{ color: "#64748b", fontSize: 14 }}>
              {posts.length} posts
            </div>
          </div>

          {/* ===== FRIEND ACTION ===== */}
          {status === "none" && (
            <Button
              label="Add friend"
              icon="pi pi-user-plus"
              onClick={addFriend}
            />
          )}
          {status === "pending_out" && (
            <Button label="Request sent" icon="pi pi-clock" disabled />
          )}
          {status === "pending_in" && (
            <Button
              label="Accept friend"
              icon="pi pi-check"
              onClick={accept}
            />
          )}
          {status === "friends" && (
            <Button
              label="Friends"
              icon="pi pi-users"
              severity="success"
              outlined
            />
          )}
        </div>
      </div>

      {/* ===== POSTS ===== */}
      {loading && (
        <div className="pcard pcard-muted">Loading posts...</div>
      )}

      {!loading && posts.length === 0 && (
        <div className="pcard pcard-muted">
          This user hasn't posted anything yet.
        </div>
      )}

      {posts.map((p) => (
        <PostCard key={p.id} post={p} onUpdated={loadAll} />
      ))}
    </HomeLayout>
  );
}
