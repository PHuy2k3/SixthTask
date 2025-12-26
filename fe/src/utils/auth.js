export function getMyUserId() {
  const token = localStorage.getItem("access_token");
  if (!token) return null;

  try {
    const payload = JSON.parse(atob(token.split(".")[1]));
    // NameIdentifier hoặc sub
    return payload?.nameid || payload?.sub || payload?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] || null;
  } catch {
    return null;
  }
}
export function parseJwt(token) {
  try {
    const payload = token.split(".")[1];
    const base64 = payload.replace(/-/g, "+").replace(/_/g, "/");
    const json = decodeURIComponent(
      atob(base64)
        .split("")
        .map((c) => "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2))
        .join("")
    );
    return JSON.parse(json);
  } catch {
    return null;
  }
}

export function getMeId() {
  const token = localStorage.getItem("access_token");
  if (!token) return null;
  const p = parseJwt(token);
  // NameIdentifier/sub
  return p?.sub || p?.nameid || p?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] || null;
}

export function getMeUserName() {
  const token = localStorage.getItem("access_token");
  if (!token) return "unknown";
  const p = parseJwt(token);
  return (
    p?.name ||
    p?.unique_name ||
    p?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] ||
    "unknown"
  );
}
