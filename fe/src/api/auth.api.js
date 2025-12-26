import { identityApi } from "./axios";

export const login = (data) =>
  identityApi.post("/api/auth/login", data);

// ✅ THÊM HÀM NÀY
export const register = (data) =>
  identityApi.post("/api/auth/register", data);

// (đã chuẩn bị cho sau)
export const refresh = (data) =>
  identityApi.post("/api/auth/refresh", data);
