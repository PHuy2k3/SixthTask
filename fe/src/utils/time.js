export function timeAgo(date) {
  const now = new Date();
  const d = new Date(date);
  const diff = Math.floor((now - d) / 1000); // seconds

  if (diff < 10) return "just now";
  if (diff < 60) return `${diff}s ago`;

  const min = Math.floor(diff / 60);
  if (min < 60) return `${min}m ago`;

  const hour = Math.floor(min / 60);
  if (hour < 24) return `${hour}h ago`;

  const day = Math.floor(hour / 24);
  if (day === 1) return "yesterday";
  if (day < 7) return `${day}d ago`;

  return d.toLocaleDateString();
}
