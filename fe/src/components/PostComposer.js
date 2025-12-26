import { useState } from "react";
import { createPost } from "../api/posts.api";
import { Button } from "primereact/button";
import { InputTextarea } from "primereact/inputtextarea";

export default function PostComposer({ onCreated }) {
  const [content, setContent] = useState("");

  async function submit() {
    const text = content.trim();
    if (!text) return;

    await createPost({ content: text, privacy: "public" });
    setContent("");
    onCreated?.();
  }

  return (
    <div className="card p-3 mb-3">
      <InputTextarea
        value={content}
        onChange={(e) => setContent(e.target.value)}
        rows={3}
        className="w-full"
        placeholder="What's on your mind?"
      />
      <div className="mt-2 flex justify-content-end">
        <Button label="Post" onClick={submit} />
      </div>
    </div>
  );
}
