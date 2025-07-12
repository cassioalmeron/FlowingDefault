import React, { useState } from 'react'
import { toast } from 'react-toastify'
import 'react-toastify/dist/ReactToastify.css'
import * as yup from 'yup'
import axios from 'axios'
import './Styles.css'
import { api } from '../../api'
import { useNavigate } from 'react-router-dom'
import LoginPaper from '../../assets/LoginPaper.jpg'

// Define validation schema
const loginSchema = yup.object({
  username: yup
    .string()
    .required('Username is required!')
    .min(3, 'Username must be at least 3 characters')
    .max(20, 'Username must be less than 20 characters'),
  password: yup
    .string()
    .required('Password is required!')
    .min(3, 'Password must be at least 6 characters')
    .max(50, 'Password must be less than 50 characters')
})

const Index = () => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [errors, setErrors] = useState<{ username?: string; password?: string }>({});
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const handleLogin = async () => {
    try {
      // Validate form data
      await loginSchema.validate({ username, password }, { abortEarly: false })
      setErrors({});
      setLoading(true);
      
      const { token } = await api.login.auth(
        username,
        password
      )
      if (token) {
        navigate('/');
      } else {
        toast.error('Invalid response from server.')
      }
    } catch (error) {
      if (error instanceof yup.ValidationError) {
        const newErrors: { username?: string; password?: string } = {};
        error.inner.forEach((err) => {
          if (err.path) {
            newErrors[err.path as keyof typeof newErrors] = err.message;
          }
        })
        setErrors(newErrors)
      } else if (axios.isAxiosError(error))
        toast.error(error.response?.data?.message || 'Login failed!');
      else
        toast.error('An unexpected error occurred.');

    } finally {
      setLoading(false);
    }
  }

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter' && !loading) {
      e.preventDefault();
      handleLogin();
    }
  }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!loading) {
      handleLogin();
    }
  }

  return (
    <div className="login-split-layout">
      <div className="login-image-side">
        <img src={LoginPaper} alt="Login visual" className="login-image" />
      </div>
      <div className="login-form-side">
        <div className="login-container">
          <h2 className="login-title">{import.meta.env.VITE_APP_NAME}</h2>
          <form className="login-form" onSubmit={handleSubmit} onKeyDown={handleKeyDown}>
            <div>
              <input
                type="text"
                placeholder="Username"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                className={`login-input${errors.username ? ' error' : ''}`}
                disabled={loading}
              />
              {errors.username && (
                <div className="login-error">
                  {errors.username}
                </div>
              )}
            </div>
            <div>
              <input
                type="password"
                placeholder="Password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                className={`login-input${errors.password ? ' error' : ''}`}
                disabled={loading}
              />
              {errors.password && (
                <div className="login-error">
                  {errors.password}
                </div>
              )}
            </div>
            <button 
              type="submit"
              className="login-button"
              disabled={loading}
            >
              {loading ? 'Logging in...' : 'Login'}
            </button>
          </form>
        </div>
      </div>
    </div>
  )
}

export default Index