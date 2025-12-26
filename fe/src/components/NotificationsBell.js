import { useEffect, useMemo, useRef, useState } from "react";
import { OverlayPanel } from "primereact/overlaypanel";
import { Button } from "primereact/button";
import { Badge } from "primereact/badge";
import { Toast } from "primereact/toast";
import { getMyNotifications, markRead } from "../api/notifications.api";
import { buildNotificationConnection } from "../realtime/notificationhub";

function labelOf(n) {
  const action = n.type === "comment" ? "đã bình luận" : "đã thích";
  const extra = n.content ? `: "${n.content}"` : "";
  return `${n.actorUserName} ${action} bài viết của bạn${extra}`;
}

export default function NotificationsBell() {
  const op = useRef(null);
  const toast = useRef(null);

  const [items, setItems] = useState([]);
  const unread = useMemo(() => items.filter(x => !x.isRead).length, [items]);

  // load list lần đầu
  useEffect(() => {
    getMyNotifications(20).then(res => setItems(res.data)).catch(() => {});
  }, []);

  // realtime SignalR
  useEffect(() => {
    const conn = buildNotificationConnection((n) => {
      setItems(prev => [n, ...prev]); // prepend

      toast.current?.show({
        severity: "info",
        summary: "Thông báo",
        detail: labelOf(n),
        life: 3000
      });
    });

    conn.start().catch(() => {});
    return () => { conn.stop().catch(() => {}); };
  }, []);

  async function onOpen(e) {
    op.current?.toggle(e);
    // refresh nhẹ mỗi lần mở
    try {
      const res = await getMyNotifications(20);
      setItems(res.data);
    } catch {}
  }

  async function readOne(n) {
    if (n.isRead) return;
    try {
      await markRead(n.id);
      setItems(prev => prev.map(x => x.id === n.id ? { ...x, isRead: true } : x));
    } catch {}
  }

  return (
    <>
      <Toast ref={toast} position="top-right" />

      <span className="p-overlay-badge">
        <Button icon="pi pi-bell" rounded text onClick={onOpen} />
        {unread > 0 && <Badge value={unread} />}
      </span>

      <OverlayPanel ref={op} style={{ width: 360 }}>
        <div className="flex justify-content-between align-items-center mb-2">
          <div className="font-bold">Notifications</div>
          <small className="text-600">{unread} unread</small>
        </div>

        <div className="flex flex-column gap-2">
          {items.length === 0 && (
            <div className="text-600">Chưa có thông báo.</div>
          )}

          {items.map(n => (
            <div
              key={n.id}
              className="p-2"
              style={{
                borderRadius: 12,
                background: n.isRead ? "#f8fafc" : "#eef2ff",
                cursor: "pointer"
              }}
              onClick={() => readOne(n)}
              title="Click để đánh dấu đã đọc"
            >
              <div className="font-bold">{n.actorUserName}</div>
              <div>{n.type === "comment" ? "comment" : "like"} • Post: {String(n.postId).slice(0, 8)}…</div>
              {n.content && <div className="text-700 mt-1">“{n.content}”</div>}
              <small className="text-600">{new Date(n.createdAt).toLocaleString()}</small>
            </div>
          ))}
        </div>
      </OverlayPanel>
    </>
  );
}
