import { Navigate } from "react-router-dom";

export default function PrivateRoute({ authed, children }) {
  if (!authed) {
    return <Navigate to="/login" replace />;
  }
  return children;
}
