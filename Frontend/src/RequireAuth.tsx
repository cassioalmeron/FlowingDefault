import { Navigate, Outlet } from 'react-router-dom';
import { session } from './session';

export default function RequireAuth() {
  return session.isAuthenticated() ? <Outlet /> : <Navigate to="/login" replace />;
}