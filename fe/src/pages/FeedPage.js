import { useEffect, useState } from "react";
import { getLatestPosts } from "../api/posts.api";
import PostComposer from "../components/PostComposer";
import PostCard from "../components/PostCard";
import HomeLayout from "./HomeLayout";

export default function FeedPage({ onLogout }) {
  const [posts, setPosts] = useState([]);

  async function load() {
    const res = await getLatestPosts(30);
    setPosts(res.data);
  }

  useEffect(() => { load(); }, []);

  return (
    <HomeLayout onLogout={onLogout} activeMenu="home">
      <PostComposer onCreated={load} />
      {posts.map(p => (
        <PostCard key={p.id} post={p} onUpdated={load} />
      ))}
    </HomeLayout>
  );
}
