import { Navigate, Outlet } from 'react-router-dom';
import { session } from './session';

export default function RedirectIfAuth() {
  return session.isAuthenticated() ? <Navigate to="/" replace /> : <Outlet />;
}