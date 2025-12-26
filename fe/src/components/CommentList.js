import { useEffect, useMemo, useState } from "react";
import { getComments, addComment, updateComment, deleteComment } from "../api/posts.api";
import { InputText } from "primereact/inputtext";
import { Button } from "primereact/button";
import { timeAgo } from "../utils/time";
import { useNavigate } from "react-router-dom";
import "../css/comment-list.css";

function getMyUserId() {
  // ✅ Cách 1: nếu bạn lưu userId sau login thì dùng cái này
  const id = localStorage.getItem("user_id");
  if (id) return id;

  // ✅ Cách 2: fallback decode JWT (không cần thư viện)
  const token = localStorage.getItem("access_token");
  if (!token) return null;
  try {
    const payload = JSON.parse(atob(token.split(".")[1]));
    return payload?.sub || payload?.nameid || payload?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] || null;
  } catch {
    return null;
  }
}

export default function CommentList({ postId }) {
  const [open, setOpen] = useState(false);
  const [items, setItems] = useState([]);
  const [text, setText] = useState("");
  const [loading, setLoading] = useState(false);

  // edit state
  const [editingId, setEditingId] = useState(null);
  const [editText, setEditText] = useState("");

  const nav = useNavigate();
  const meId = useMemo(() => getMyUserId(), []);

  async function load() {
    const res = await getComments(postId);
    setItems(res.data);
  }

  async function submit() {
    const t = text.trim();
    if (!t) return;

    setLoading(true);
    try {
      await addComment(postId, t);
      setText("");
      await load();
    } finally {
      setLoading(false);
    }
  }

  function startEdit(c) {
    setEditingId(c.id);
    setEditText(c.content || "");
  }

  function cancelEdit() {
    setEditingId(null);
    setEditText("");
  }

  async function saveEdit() {
    const t = editText.trim();
    if (!t) return;

    setLoading(true);
    try {
      await updateComment(editingId, t);
      cancelEdit();
      await load();
    } finally {
      setLoading(false);
    }
  }

  async function remove(c) {
    const ok = window.confirm("Delete this comment?");
    if (!ok) return;

    setLoading(true);
    try {
      await deleteComment(c.id);
      await load();
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    if (open) load();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [open, postId]);

  return (
    <div className="mt-2 clist">
      <Button
        label={open ? "Hide comments" : "View comments"}
        icon="pi pi-comments"
        text
        size="small"
        className="clist-toggle"
        onClick={() => setOpen((v) => !v)}
      />

      {open && (
        <div className="mt-2">
          <div className="clist-items">
            {items.map((c) => {
              const isMine = meId && String(c.userId).toLowerCase() === String(meId).toLowerCase();
              const isEditing = editingId === c.id;

              return (
                <div key={c.id} className="clist-item">
                  <div className="clist-avatar">
                    {(c.userName || "?").charAt(0).toUpperCase()}
                  </div>

                  <div className="clist-body">
                    <div className="clist-bubble">
                      <div className="clist-topRow">
                        <span
                          className="comment-user clickable-user clist-name"
                          onClick={() => nav(`/u/${c.userId}`)}
                          title="View profile"
                        >
                          {c.userName}
                        </span>

                        {/* ✅ chỉ hiện với comment của mình */}
                        {isMine && !isEditing && (
                          <div className="clist-actions">
                            <button className="clist-actionBtn" onClick={() => startEdit(c)} title="Edit">
                              ✏️
                            </button>
                            <button className="clist-actionBtn danger" onClick={() => remove(c)} title="Delete">
                              🗑️
                            </button>
                          </div>
                        )}
                      </div>

                      {/* content / edit */}
                      {!isEditing ? (
                        <div className="clist-text">{c.content}</div>
                      ) : (
                        <div className="clist-editBox">
                          <InputText
                            value={editText}
                            onChange={(e) => setEditText(e.target.value)}
                            className="w-full clist-editInput"
                            placeholder="Edit your comment..."
                          />
                          <div className="clist-editBtns">
                            <Button
                              label="Cancel"
                              severity="secondary"
                              outlined
                              size="small"
                              onClick={cancelEdit}
                              disabled={loading}
                            />
                            <Button
                              label="Save"
                              icon="pi pi-check"
                              size="small"
                              onClick={saveEdit}
                              loading={loading}
                            />
                          </div>
                        </div>
                      )}
                    </div>

                    <div className="clist-time">{timeAgo(c.createdAt)}</div>
                  </div>
                </div>
              );
            })}
          </div>

          {/* input add new comment */}
          <div className="mt-2 clist-inputRow">
            <InputText
              value={text}
              onChange={(e) => setText(e.target.value)}
              placeholder="Write a comment..."
              className="w-full clist-input"
            />
            <Button label="Send" icon="pi pi-send" loading={loading} onClick={submit} className="clist-send" />
          </div>
        </div>
      )}
    </div>
  );
}
