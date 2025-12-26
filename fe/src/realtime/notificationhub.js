import * as signalR from "@microsoft/signalr";
import { notificationsBaseUrl } from "../api/notifications.api";

export function buildNotificationConnection(onNotify) {
  const conn = new signalR.HubConnectionBuilder()
    .withUrl(`${notificationsBaseUrl}/hubs/notifications`, {
      accessTokenFactory: () => localStorage.getItem("access_token") || ""
    })
    .withAutomaticReconnect()
    .build();

  conn.on("notify", (payload) => onNotify(payload));
  return conn;
}
