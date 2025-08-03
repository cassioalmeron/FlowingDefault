import axios from 'axios';
import { session } from './session';

const BASE_URL = import.meta.env.VITE_API_URL;

// Centralized function to generate headers with authorization
const getAuthHeaders = () => {
  // Check if token has expired before making request
  if (session.isTokenExpired()) {
    session.logout();
    throw new Error('Token has expired. Please login again.');
  }
  
  const token = session.getToken();
  return {
    Authorization: `Bearer ${token}`
  };
};

// Utility function to check token expiration
export const checkTokenExpiration = () => {
  if (session.isTokenExpired()) {
    session.logout();
    // Redirect to login page
    window.location.href = '/login';
    return false;
  }
  return true;
};

// React hook for token expiration checking (for use in components)
export const useTokenExpiration = () => {
  const checkExpiration = () => {
    if (session.isTokenExpired()) {
      session.logout();
      window.location.href = '/login';
      return false;
    }
    return true;
  };

  return { checkExpiration };
};

// Centralized error handler for API calls
const handleApiError = (error: unknown) => {
  if (error instanceof Error && error.message === 'Token has expired. Please login again.') {
    session.logout();
    window.location.href = '/login';
    return;
  }
  throw error;
};

// Centralized API request functions
const apiRequest = {
  get: async <T>(url: string): Promise<T> => {
    try {
      const response = await axios.get<T>(url, {
        headers: getAuthHeaders()
      });
      return response.data;
    } catch (error) {
      handleApiError(error);
      throw error;
    }
  },

  post: async <T>(url: string, data?: unknown): Promise<T> => {
    try {
      const response = await axios.post<T>(url, data, {
        headers: {
          ...getAuthHeaders(),
          'Content-Type': 'application/json'
        }
      });
      return response.data;
    } catch (error) {
      handleApiError(error);
      throw error;
    }
  },

  put: async <T>(url: string, data?: unknown): Promise<T> => {
    try {
      const response = await axios.put<T>(url, data, {
        headers: {
          ...getAuthHeaders(),
          'Content-Type': 'application/json'
        }
      });
      return response.data;
    } catch (error) {
      handleApiError(error);
      throw error;
    }
  },

  patch: async <T>(url: string, data?: unknown): Promise<T> => {
    try {
      const response = await axios.patch<T>(url, data, {
        headers: {
          ...getAuthHeaders(),
          'Content-Type': 'application/json'
        }
      });
      return response.data;
    } catch (error) {
      handleApiError(error);
      throw error;
    }
  },

  delete: async (url: string): Promise<void> => {
    try {
      await axios.delete(url, {
        headers: getAuthHeaders()
      });
    } catch (error) {
      handleApiError(error);
      throw error;
    }
  }
};

// Types
export interface AuthResponse {
    name: string;
    isAdmin: boolean;
    token: string;
    expires: Date;
}

export interface User {
    id: number;
    name: string;
    username: string;
}

export interface Project {
    id: number;
    name: string;
}

export interface Label {
    id: number;
    name: string;
}

// API functions
export const api = {
  // Chat endpoints
  login: {
    // Get all chats
    auth: async (username: string, password: string): Promise<AuthResponse> => {
      const response = await axios.post<AuthResponse>(`${BASE_URL}/Auth`, {
        username,
        password
      });

      const data = response.data;
      
      const user = {
        name: data.name,
        isAdmin: data.isAdmin,
        token: data.token,
        expires: data.expires
      };

      session.login(user);

      return data;
    },
  },
  users: {
    list: async (): Promise<User[]> => {
      return apiRequest.get<User[]>(`${BASE_URL}/User`);
    },
    delete: async (id: number): Promise<void> => {
      return apiRequest.delete(`${BASE_URL}/User/${id}`);
    },
    create: async (user: Omit<User, 'id'>): Promise<User> => {
      return apiRequest.post<User>(`${BASE_URL}/User`, user);
    },
    update: async (id: number, user: Omit<User, 'id'>): Promise<User> => {
      return apiRequest.put<User>(`${BASE_URL}/User/${id}`, user);
    },
  },
  projects: {
    list: async (): Promise<Project[]> => {
      return apiRequest.get<Project[]>(`${BASE_URL}/Project`);
    },
    delete: async (id: number): Promise<void> => {
      return apiRequest.delete(`${BASE_URL}/Project/${id}`);
    },
    create: async (project: Omit<Project, 'id'>): Promise<Project> => {
      return apiRequest.post<Project>(`${BASE_URL}/Project`, project);
    },
    update: async (id: number, project: Omit<Project, 'id'>): Promise<Project> => {
      return apiRequest.put<Project>(`${BASE_URL}/Project/${id}`, project);
    },
  },
  labels: {
    list: async (): Promise<Label[]> => {
      return apiRequest.get<Label[]>(`${BASE_URL}/Label`);
    },
    delete: async (id: number): Promise<void> => {
      return apiRequest.delete(`${BASE_URL}/Label/${id}`);
    },
    create: async (label: Omit<Label, 'id'>): Promise<Label> => {
      return apiRequest.post<Label>(`${BASE_URL}/Label`, label);
    },
    update: async (id: number, label: Omit<Label, 'id'>): Promise<Label> => {
      return apiRequest.put<Label>(`${BASE_URL}/Label/${id}`, label);
    },
  },
  profile: {
    getProfile: async (): Promise<{ name: string; username: string }> => {
      return apiRequest.get<{ name: string; username: string }>(`${BASE_URL}/Profile`);
    },
    updateProfile: async (data: { name: string; username: string }): Promise<void> => {
      return apiRequest.put(`${BASE_URL}/Profile`, data);
    },
    changePassword: async (newPassword: string): Promise<void> => {
      return apiRequest.put(`${BASE_URL}/Profile/Password`, { newPassword });
    },
  },
};