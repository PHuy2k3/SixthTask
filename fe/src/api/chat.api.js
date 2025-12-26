import { chatApi } from "./axios";

export const getMyConversations = (size = 30) =>
  chatApi.get(`/api/chat/conversations?size=${size}`);

export const getOrCreateConversation = (otherUserId, otherUserName = "") =>
  chatApi.post(`/api/chat/conversations`, { otherUserId, otherUserName });

export const getMessages = (conversationId, size = 50) =>
  chatApi.get(`/api/chat/messages/${conversationId}?size=${size}`);

export const sendMessage = (conversationId, content) =>
  chatApi.post(`/api/chat/messages`, { conversationId, content });

export const markRead = (conversationId) =>
  chatApi.put(`/api/chat/read`, { conversationId });
