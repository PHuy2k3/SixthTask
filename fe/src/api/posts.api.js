import { postsApi } from "./axios";

export const getLatestPosts = () => postsApi.get("/api/posts/latest");
export const createPost     = (payload) => postsApi.post("/api/posts", payload);
export const setReaction    = (postId, type) => postsApi.put(`/api/posts/${postId}/reaction`, { type });
export const addComment     = (postId, content) => postsApi.post(`/api/posts/${postId}/comments`, { content });
export const getComments    = (postId) => postsApi.get(`/api/posts/${postId}/comments`);
export const getMyPosts = (size = 20) => postsApi.get(`/api/posts/me?size=${size}`);
export const getPostsByUser = (userId, size = 20) =>
  postsApi.get(`/api/posts/user/${userId}?size=${size}`);
export const updatePost = (postId, payload) => postsApi.put(`/api/posts/${postId}`, payload);
export const deletePost = (postId) => postsApi.delete(`/api/posts/${postId}`);
export const updateComment = (commentId, content) =>
  postsApi.put(`/api/posts/comments/${commentId}`, { content });
export const deleteComment = (commentId) =>
  postsApi.delete(`/api/posts/comments/${commentId}`);
