import { useState, useRef, useMemo } from "react";
import { OverlayPanel } from "primereact/overlaypanel";
import { Dialog } from "primereact/dialog";
import { InputTextarea } from "primereact/inputtextarea";
import { Button } from "primereact/button";
import { ConfirmDialog, confirmDialog } from "primereact/confirmdialog";

import { setReaction, updatePost, deletePost } from "../api/posts.api";
import CommentList from "./CommentList";
import { timeAgo } from "../utils/time";
import { useNavigate } from "react-router-dom";
import "../css/post-card.css";

const POSTS_BASE = "http://localhost:5028";

const REACTIONS = [
  { type: 0, emoji: "👍" },
  { type: 1, emoji: "❤️" },
  { type: 2, emoji: "😂" },
  { type: 3, emoji: "😮" },
  { type: 4, emoji: "😢" },
  { type: 5, emoji: "😡" }
];

// ✅ lấy userId từ JWT để biết post nào là của mình
function getMyUserId() {
  const token = localStorage.getItem("access_token");
  if (!token) return null;
  try {
    const payload = JSON.parse(atob(token.split(".")[1]));
    return (
      payload?.nameid ||
      payload?.sub ||
      payload?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] ||
      null
    );
  } catch {
    return null;
  }
}

export default function PostCard({ post, onUpdated }) {
  const nav = useNavigate();
  const op = useRef(null);
  const actions = useRef(null);

  const [viewImg, setViewImg] = useState(null);

  // edit post
  const [editOpen, setEditOpen] = useState(false);
  const [editText, setEditText] = useState(post.content || "");
  const [saving, setSaving] = useState(false);

  const myId = useMemo(() => getMyUserId(), []);
  const isMine = myId && String(myId).toLowerCase() === String(post.authorId).toLowerCase();

  async function react(type) {
    await setReaction(post.id, type);
    op.current?.hide();
    onUpdated?.();
  }

  const myEmoji =
    post.myReactionType != null
      ? REACTIONS.find((x) => x.type === post.myReactionType)?.emoji
      : "👍";

  // ✅ EDIT
  function openEdit() {
    actions.current?.hide?.();
    setEditText(post.content || "");
    setEditOpen(true);
  }

  async function saveEdit() {
    const t = (editText || "").trim();
    setSaving(true);
    try {
      await updatePost(post.id, t);
      setEditOpen(false);
      onUpdated?.();
    } finally {
      setSaving(false);
    }
  }

  // ✅ DELETE
  function askDelete() {
    actions.current?.hide?.();
    confirmDialog({
      message: "Xóa bài post này?",
      header: "Xác nhận",
      icon: "pi pi-exclamation-triangle",
      acceptClassName: "p-button-danger",
      accept: async () => {
        await deletePost(post.id);
        onUpdated?.();
      }
    });
  }

  // ảnh url chuẩn
  function resolveImgUrl(u) {
    return u.startsWith("http") ? u : `${POSTS_BASE}${u}`;
  }

  return (
    <div className="card p-3 mb-3">
      <ConfirmDialog />

      <div style={{ display: "flex", alignItems: "flex-start", gap: 10 }}>
        <div style={{ flex: 1 }}>
          <div style={{ display: "flex", flexDirection: "column" }}>
            <div
              className="post-user clickable-user"
              onClick={() => nav(`/u/${post.authorId}`)}
              title="View profile"
            >
              {post.authorUserName}
            </div>
            <div style={{ fontSize: 12, color: "#64748b" }}>
              {timeAgo(post.createdAt)}
            </div>
          </div>
        </div>

        {/* ✅ NÚT ... CHỈ HIỆN KHI LÀ POST CỦA MÌNH */}
        {isMine && (
          <button
            className="post-more-btn"
            onClick={(e) => actions.current?.toggle(e)}
            type="button"
            aria-label="More"
          >
            ⋯
          </button>
        )}
      </div>

      {post.content && <div className="mt-2">{post.content}</div>}

      {post.imageUrls?.length > 0 && (
        <div className="mt-2" style={{ display: "grid", gap: 8 }}>
          {post.imageUrls.map((u, i) => (
            <img
              key={i}
              src={resolveImgUrl(u)}
              alt=""
              className="post-img"
              onClick={() => setViewImg(resolveImgUrl(u))}
            />
          ))}
        </div>
      )}

      <div className="mt-2 text-600" style={{ fontSize: 13 }}>
        {post.reactionCounts?.map((rc) => {
          const emoji = REACTIONS.find((x) => x.type === rc.type)?.emoji;
          return (
            <span key={rc.type} style={{ marginRight: 10 }}>
              {emoji} {rc.count}
            </span>
          );
        })}
        <span style={{ marginLeft: 10 }}>💬 {post.commentCount}</span>
      </div>

      <div className="mt-2" style={{ display: "flex", gap: 12 }}>
        <button onClick={(e) => op.current.toggle(e)} style={btnStyle}>
          {myEmoji} React
        </button>
      </div>

      <OverlayPanel ref={op}>
        <div style={{ display: "flex", gap: 10, fontSize: 24 }}>
          {REACTIONS.map((r) => (
            <span
              key={r.type}
              style={{ cursor: "pointer" }}
              onClick={() => react(r.type)}
            >
              {r.emoji}
            </span>
          ))}
        </div>
      </OverlayPanel>

      {/* ✅ MENU EDIT/DELETE */}
      <OverlayPanel ref={actions}>
        <div className="post-actions-menu">
          <button className="post-actions-item" onClick={openEdit}>
            ✏️ Edit
          </button>
          <button className="post-actions-item danger" onClick={askDelete}>
            🗑 Delete
          </button>
        </div>
      </OverlayPanel>

      {/* ✅ COMMENT UI */}
      <CommentList postId={post.id} onChanged={onUpdated} />

      {/* ✅ VIEW IMAGE */}
      {viewImg && (
        <div className="img-viewer" onClick={() => setViewImg(null)}>
          <div className="img-viewer-inner" onClick={(e) => e.stopPropagation()}>
            <img src={viewImg} alt="" />
            <button
              className="img-viewer-close"
              onClick={() => setViewImg(null)}
              type="button"
              aria-label="Close"
            >
              ×
            </button>
          </div>
        </div>
      )}

      {/* ✅ EDIT DIALOG */}
      <Dialog
        header="Edit post"
        visible={editOpen}
        style={{ width: "560px", maxWidth: "92vw" }}
        onHide={() => setEditOpen(false)}
        footer={
          <div style={{ display: "flex", justifyContent: "flex-end", gap: 8 }}>
            <Button label="Cancel" outlined onClick={() => setEditOpen(false)} />
            <Button label="Save" loading={saving} onClick={saveEdit} />
          </div>
        }
      >
        <InputTextarea
          value={editText}
          onChange={(e) => setEditText(e.target.value)}
          autoResize
          rows={5}
          className="w-full"
          placeholder="What's on your mind?"
        />
      </Dialog>
    </div>
  );
}

const btnStyle = {
  cursor: "pointer",
  border: "1px solid #e2e8f0",
  background: "#fff",
  padding: "8px 12px",
  borderRadius: 10
};
