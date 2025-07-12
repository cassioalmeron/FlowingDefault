import { BrowserRouter as Router, Routes, Route, Link, useLocation, useNavigate } from 'react-router-dom';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import Home from './Pages/Home/Index';
import Login from './Pages/Login/Index';
import Users from './Pages/Users/Index';
import './App.css';
import Projects from './Pages/Projects/Index';
import RequireAuth from './RequireAuth';
import RedirectIfAuth from './RedirectIfAuth';
import Tests from './Pages/Tests';
import { session } from './session';
import Profile from './Pages/Profile';

// Component to conditionally render layout
const AppLayout = () => {
  const location = useLocation();
  const navigate = useNavigate();
  const isLoginPage = location.pathname === '/login';

  const handleLogout = () => {
    session.logout();
    navigate('/login');
  };

  if (isLoginPage) {
    return (
      <div className="login-layout">
        <Routes>
          <Route element={<RedirectIfAuth />}>
            <Route path="/login" element={<Login />} />
          </Route>
        </Routes>
      </div>
    );
  }

  return (
    <div className="app-layout">
      {/* Sidebar Navigation */}
      <nav className="sidebar">
        <div className="sidebar-links">
          <Link to="/" className="sidebar-link">🏠 Home</Link>
          <Link to="/users" className="sidebar-link">👥 Users</Link>
          <Link to="/projects" className="sidebar-link">📁 Projects</Link>
          <Link to="/profile" className="sidebar-link">👤 Profile</Link>
        </div>
        <button className="sidebar-link logout-btn" onClick={handleLogout} style={{ marginTop: 'auto', width: '100%' }}>
          🚪 Logout
        </button>
      </nav>
      {/* Main Content Area */}
      <main className="main-content">
        <Routes>
          <Route element={<RequireAuth />}>
            <Route path="/" element={<Home />} />
            <Route path="/users" element={<Users />} />
            <Route path="/projects" element={<Projects />} />
            <Route path="/tests" element={<Tests />} />
            <Route path="/profile" element={<Profile />} />
          </Route>
        </Routes>
      </main>
    </div>
  );
};

function App() {
  return (
    <Router>
      <AppLayout />
      <ToastContainer />
    </Router>
  );
}

export default App;
