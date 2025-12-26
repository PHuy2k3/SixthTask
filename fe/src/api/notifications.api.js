import axios from "axios";

const notificationsApi = axios.create({
  baseURL: "http://localhost:5207", // 🔥 đổi đúng port Notifications.Api của bạn
});

notificationsApi.interceptors.request.use((config) => {
  const token = localStorage.getItem("access_token");
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

export const getMyNotifications = (size = 20) =>
  notificationsApi.get(`/api/notifications/me?size=${size}`);

export const markRead = (id) =>
  notificationsApi.post(`/api/notifications/${id}/read`);

export const notificationsBaseUrl = "http://localhost:5207"; // dùng cho SignalR
