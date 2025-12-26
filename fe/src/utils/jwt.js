export function parseJwt(token) {
  try {
    const base64 = token.split(".")[1];
    const json = decodeURIComponent(
      atob(base64).split("").map(c => "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2)).join("")
    );
    return JSON.parse(json);
  } catch {
    return null;
  }
}

export function getMeFromToken() {
  const t = localStorage.getItem("access_token");
  if (!t) return { id: null, name: null };

  const p = parseJwt(t);
  // sub hoặc nameid
  const id = p?.sub || p?.nameid || p?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] || null;
  const name = p?.unique_name || p?.name || p?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] || null;

  return { id, name };
}

export function decodeJwt(token) {
  try {
    const payload = token.split(".")[1];
    const json = atob(payload.replace(/-/g, "+").replace(/_/g, "/"));
    return JSON.parse(decodeURIComponent(escape(json)));
  } catch {
    return null;
  }
}

