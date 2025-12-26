import * as signalR from "@microsoft/signalr";

const CHAT_BASE = "http://localhost:5255";
let callConn = null;

export async function getCallConnection() {
  if (callConn) return callConn;

  callConn = new signalR.HubConnectionBuilder()
    .withUrl(`${CHAT_BASE}/hubs/call`, {
      accessTokenFactory: () =>
        localStorage.getItem("access_token") || ""
    })
    .withAutomaticReconnect()
    .build();

  await callConn.start();
  return callConn;
}
