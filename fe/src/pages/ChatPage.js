import { useEffect, useMemo, useRef, useState } from "react";
import HomeLayout from "./HomeLayout";
import { useNavigate, useParams } from "react-router-dom";
import {
  getOrCreateConversation,
  getMessages,
  getMyConversations,
  markRead,
  sendMessage
} from "../api/chat.api";
import { getMeId } from "../utils/auth";
import { getChatConnection } from "../realtime/chatHub";
import "../css/chat.css";

export default function ChatPage({ onLogout }) {
  const nav = useNavigate();
  const { userId } = useParams(); // optional: /chat/u/:userId
  const meId = useMemo(() => getMeId(), []);
  const [convs, setConvs] = useState([]);
  const [active, setActive] = useState(null); // ConversationDto
  const [msgs, setMsgs] = useState([]);
  const [text, setText] = useState("");
  const listRef = useRef(null);

  // load conversations
  async function loadConvs() {
    const res = await getMyConversations(50);
    setConvs(res.data);
  }

  // open conversation by id
  async function openConversation(conv) {
    setActive(conv);

    const res = await getMessages(conv.id, 100);
    setMsgs(res.data);

    await markRead(conv.id).catch(() => {});
    scrollToBottom();
  }

  // if route has /chat/u/:userId -> create/get conversation with that user
  async function openThreadByUser(otherUserId) {
    // tìm userName từ list conv có sẵn (nếu có)
    const existed = convs.find(c => c.otherUser?.id === otherUserId);
    const otherName = existed?.otherUser?.userName || "unknown";

    const res = await getOrCreateConversation(otherUserId, otherName);
    const conv = res.data;

    // refresh conv list
    await loadConvs();

    // open it
    await openConversation(conv);
  }

  function scrollToBottom() {
    requestAnimationFrame(() => {
      if (!listRef.current) return;
      listRef.current.scrollTop = listRef.current.scrollHeight;
    });
  }

  async function handleSend() {
    const t = text.trim();
    if (!t || !active) return;

    setText("");

    // optimistic append (không nhảy bên nếu meId đúng)
    const optimistic = {
      id: "tmp-" + Date.now(),
      conversationId: active.id,
      senderId: meId,
      senderUserName: "me",
      content: t,
      createdAt: new Date().toISOString(),
      isRead: false
    };
    setMsgs(prev => [...prev, optimistic]);
    scrollToBottom();

    try {
      await sendMessage(active.id, t);
      // server sẽ push realtime "message:new" về lại, mình có thể reload nhẹ
      // hoặc bỏ tmp khi nhận realtime (dưới)
    } catch {
      // rollback đơn giản
      setMsgs(prev => prev.filter(x => x.id !== optimistic.id));
      setText(t);
    }
  }

  useEffect(() => {
    loadConvs();
    // eslint-disable-next-line
  }, []);

  // setup SignalR realtime
  useEffect(() => {
    let mounted = true;
    let conn;

    (async () => {
      conn = await getChatConnection();

      conn.off("message:new");
      conn.on("message:new", (msg) => {
        if (!mounted) return;

        // nếu đang mở đúng conversation -> append
        setMsgs(prev => {
          // tránh duplicate
          if (prev.some(x => x.id === msg.id)) return prev;
          // remove tmp same content gần nhất (optional)
          const cleaned = prev.filter(x => !(String(x.id).startsWith("tmp-") && x.content === msg.content));
          return [...cleaned, msg];
        });

        // update list conv "last message"
        setConvs(prev => {
          const next = [...prev];
          const idx = next.findIndex(c => c.id === msg.conversationId);
          if (idx >= 0) {
            next[idx] = {
              ...next[idx],
              lastMessage: msg.content,
              lastMessageAt: msg.createdAt
            };
          }
          return next;
        });

        scrollToBottom();
      });
    })();

    return () => {
      mounted = false;
      if (conn) conn.off("message:new");
    };
    // eslint-disable-next-line
  }, [meId, active?.id]);

  // join group when active changes
  useEffect(() => {
    let conn;
    (async () => {
      if (!active) return;
      conn = await getChatConnection();

      // ChatHub của bạn nên có method Join(conversationId)
      // Nếu bạn chưa có, tạm thời bỏ cũng vẫn nhận msg nếu server broadcast global,
      // nhưng chuẩn là join group.
      try {
        await conn.invoke("Join", active.id.toString());
      } catch {}
    })();
  }, [active?.id]);

  // open by url param
  useEffect(() => {
    if (!userId) return;
    if (!convs.length) return;
    openThreadByUser(userId);
    // eslint-disable-next-line
  }, [userId, convs.length]);

  return (
    <HomeLayout onLogout={onLogout} activeMenu="chat">
      <div className="chat-shell">
        {/* left list */}
        <div className="chat-left">
          <div className="chat-left-head">
            <div className="chat-title">Chats</div>
          </div>

          <div className="chat-list">
            {convs.map(c => (
              <button
                key={c.id}
                className={"chat-item " + (active?.id === c.id ? "active" : "")}
                onClick={() => openConversation(c)}
              >
                <div className="chat-ava">
                  {c.otherUser?.userName?.charAt(0)?.toUpperCase() || "U"}
                </div>
                <div className="chat-item-meta">
                  <div className="chat-item-name">{c.otherUser?.userName}</div>
                  <div className="chat-item-last">{c.lastMessage || "Say hi 👋"}</div>
                </div>
                {c.unreadCount > 0 && (
                  <div className="chat-badge">{c.unreadCount}</div>
                )}
              </button>
            ))}
          </div>
        </div>

        {/* right conversation */}
        <div className="chat-right">
          {!active ? (
            <div className="chat-empty">
              <div className="chat-empty-title">Chọn 1 cuộc trò chuyện</div>
              <div className="chat-empty-sub">Bạn có thể bấm Chat ở trang Friends.</div>
            </div>
          ) : (
            <>
              <div className="chat-topbar">
                <div
                  className="chat-top-name"
                  onClick={() => nav(`/u/${active.otherUser?.id}`)}
                  title="View profile"
                >
                  {active.otherUser?.userName}
                </div>
              </div>

              <div className="chat-messages" ref={listRef}>
                {msgs.map(m => {
                  const mine = String(m.senderId).toLowerCase() === String(meId).toLowerCase();
                  return (
                    <div key={m.id} className={"msg-row " + (mine ? "mine" : "theirs")}>
                      <div className="msg-bubble">
                        {m.content}
                        <div className="msg-time">
                          {new Date(m.createdAt).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" })}
                        </div>
                      </div>
                    </div>
                  );
                })}
              </div>

              <div className="chat-inputbar">
                <input
                  className="chat-input"
                  value={text}
                  onChange={(e) => setText(e.target.value)}
                  placeholder="Aa"
                  onKeyDown={(e) => {
                    if (e.key === "Enter" && !e.shiftKey) {
                      e.preventDefault();
                      handleSend();
                    }
                  }}
                />
                <button className="chat-send" onClick={handleSend}>Send</button>
              </div>
            </>
          )}
        </div>
      </div>
    </HomeLayout>
  );
}
