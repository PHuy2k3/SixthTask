import * as signalR from "@microsoft/signalr";

const CHAT_BASE = "http://localhost:5255"; // port Chat.Api
let conn = null;

export async function getChatConnection() {
  if (conn) return conn;

  conn = new signalR.HubConnectionBuilder()
    .withUrl(`${CHAT_BASE}/hubs/chat`, {   // ✅ SỬA Ở ĐÂY
      accessTokenFactory: () =>
        localStorage.getItem("access_token") || ""
    })
    .withAutomaticReconnect()
    .build();

  await conn.start();
  return conn;
}
