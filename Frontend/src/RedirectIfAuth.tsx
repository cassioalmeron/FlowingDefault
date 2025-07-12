import { Navigate, Outlet } from 'react-router-dom';

export default function RedirectIfAuth() {
  const token = localStorage.getItem('token');
  return token ? <Navigate to="/" replace /> : <Outlet />;
} 