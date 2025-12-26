import { identityApi } from "./axios";

// ✅ status: chỉ gọi khi có userId hợp lệ
export const getFriendStatus = (userId) => {
  if (!userId) return Promise.resolve({ data: { status: "none" } });
  return identityApi.get(`/api/friends/status/${userId}`);
};

// ✅ request: không cho gọi nếu thiếu userId
export const sendFriendRequest = (userId) => {
  if (!userId) return Promise.reject(new Error("Missing userId"));
  return identityApi.post(`/api/friends/request/${userId}`);
};

// ✅ accept: không cho gọi nếu thiếu userId
export const acceptFriendRequest = (userId) => {
  if (!userId) return Promise.reject(new Error("Missing userId"));
  return identityApi.post(`/api/friends/accept/${userId}`);
};

// ✅ list: không cần userId (là list bạn bè của "me")
export const getFriends = () =>
  identityApi.get(`/api/friends/list`);
