import axios from "axios";

export const identityApi = axios.create({ baseURL: "http://localhost:5205" });
export const postsApi    = axios.create({ baseURL: "http://localhost:5028" });
export const chatApi     = axios.create({ baseURL: "http://localhost:5255" });

function attachAuth(instance) {
  instance.interceptors.request.use((config) => {
    const token = localStorage.getItem("access_token");
    if (token) config.headers.Authorization = `Bearer ${token}`;
    return config;
  });
}

attachAuth(identityApi);
attachAuth(postsApi);
attachAuth(chatApi);