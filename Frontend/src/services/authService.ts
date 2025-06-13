import axios from 'axios';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
}

export interface AuthResponse {
  success: boolean;
  message: string;
  accessToken: string;
  refreshToken: string;
  hasCompany: boolean;
}

const API_URL = process.env.REACT_APP_API_URL || 'http://localhost:5000/api';

export const authService = {
  login: async (data: LoginRequest): Promise<AuthResponse> => {
    const response = await axios.post(`${API_URL}/auth/login`, data);
    return response.data;
  },

  register: async (data: RegisterRequest): Promise<AuthResponse> => {
    const response = await axios.post(`${API_URL}/auth/register`, data);
    return response.data;
  },

  refreshToken: async (refreshToken: string): Promise<AuthResponse> => {
    const response = await axios.post(`${API_URL}/auth/refresh-token`, { refreshToken });
    return response.data;
  },

  revokeToken: async (refreshToken: string): Promise<void> => {
    await axios.post(`${API_URL}/auth/revoke-token`, { refreshToken });
  },

  confirmEmail: async (userId: string, token: string): Promise<AuthResponse> => {
    const response = await axios.get(`${API_URL}/auth/confirm-email`, {
      params: { userId, token },
    });
    return response.data;
  },

  googleLogin: () => {
    window.location.href = `${API_URL}/auth/google-login`;
  },
}; 