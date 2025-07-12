import axios from 'axios';
import { session } from './session';

const BASE_URL = import.meta.env.VITE_API_URL;

// Types
export interface AuthResponse {
    token: string;
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

      session.login(response.data.token);

      return response.data;
    },
  },
  users: {
    list: async (): Promise<User[]> => {
      const token = session.getToken();
      const response = await axios.get<User[]>(`${BASE_URL}/User`, {
        headers: {
          Authorization: `Bearer ${token}`
        }
      });
      return response.data;
    },
    delete: async (id: number): Promise<void> => {
      const token = session.getToken();
      await axios.delete(`${BASE_URL}/User/${id}`, {
        headers: {
          Authorization: `Bearer ${token}`
        }
      });
    },
    create: async (user: Omit<User, 'id'>): Promise<User> => {
      const token = session.getToken();
      const response = await axios.post<User>(`${BASE_URL}/User`, user, {
        headers: {
          Authorization: `Bearer ${token}`
        }
      });
      return response.data;
    },
    update: async (id: number, user: Omit<User, 'id'>): Promise<User> => {
      const token = session.getToken();
      const response = await axios.put<User>(`${BASE_URL}/User/${id}`, user, {
        headers: {
          Authorization: `Bearer ${token}`
        }
      });
      return response.data;
    },
  },
  projects: {
    list: async (): Promise<Project[]> => {
      const token = session.getToken();
      const response = await axios.get<Project[]>(`${BASE_URL}/Project`, {
        headers: {
          Authorization: `Bearer ${token}`
        }
      });
      return response.data;
    },
    delete: async (id: number): Promise<void> => {
      const token = session.getToken();
      await axios.delete(`${BASE_URL}/Project/${id}`, {
        headers: {
          Authorization: `Bearer ${token}`
        }
      });
    },
    create: async (project: Omit<Project, 'id'>): Promise<Project> => {
      const token = session.getToken();
      const response = await axios.post<Project>(`${BASE_URL}/Project`, project, {
        headers: {
          Authorization: `Bearer ${token}`
        }
      });
      return response.data;
    },
    update: async (id: number, project: Omit<Project, 'id'>): Promise<Project> => {
      const token = session.getToken();
      const response = await axios.put<Project>(`${BASE_URL}/Project/${id}`, project, {
        headers: {
          Authorization: `Bearer ${token}`
        }
      });
      return response.data;
    },
  },
  labels: {
    list: async (): Promise<Label[]> => {
      const token = session.getToken();
      const response = await axios.get<Label[]>(`${BASE_URL}/Label`, {
        headers: {
          Authorization: `Bearer ${token}`
        }
      });
      return response.data;
    },
    delete: async (id: number): Promise<void> => {
      const token = session.getToken();
      await axios.delete(`${BASE_URL}/Label/${id}`, {
        headers: {
          Authorization: `Bearer ${token}`
        }
      });
    },
    create: async (label: Omit<Label, 'id'>): Promise<Label> => {
      const token = session.getToken();
      const response = await axios.post<Label>(`${BASE_URL}/Label`, label, {
        headers: {
          Authorization: `Bearer ${token}`
        }
      });
      return response.data;
    },
    update: async (id: number, label: Omit<Label, 'id'>): Promise<Label> => {
      const token = session.getToken();
      const response = await axios.put<Label>(`${BASE_URL}/Label/${id}`, label, {
        headers: {
          Authorization: `Bearer ${token}`
        }
      });
      return response.data;
    },
  },
  profile: {
    getProfile: async (): Promise<{ name: string; username: string }> => {
      const token = session.getToken();
      const response = await axios.get(`${BASE_URL}/Profile`, {
        headers: {
          Authorization: `Bearer ${token}`
        }
      });
      return response.data;
    },
    updateProfile: async (data: { name: string; username: string }): Promise<void> => {
      const token = session.getToken();
      await axios.put(`${BASE_URL}/Profile`, data, {
        headers: {
          Authorization: `Bearer ${token}`
        }
      });
    },
    changePassword: async (newPassword: string): Promise<void> => {
      const token = session.getToken();
      await axios.put(`${BASE_URL}/Profile/Password`, { newPassword }, {
        headers: {
          Authorization: `Bearer ${token}`
        }
      });
    },
  },
};