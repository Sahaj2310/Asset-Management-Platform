import axios from 'axios';
import { API_BASE_URL } from '../config';

export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  role: string;
  emailConfirmed: boolean;
}

export interface UserProfileResponse {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  role: string;
  emailConfirmed: boolean;
}

export interface CreateUserRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  role: string;
}

export interface UpdateUserRequest {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: string;
  status: 'active' | 'inactive';
}

export interface UpdatePasswordRequest {
  currentPassword: string;
  newPassword: string;
}

export interface UpdateProfileRequest {
  firstName: string;
  lastName: string;
  email: string;
}

const API_URL = process.env.REACT_APP_API_URL || 'http://localhost:5000/api';

const userService = {
  getAll: async (): Promise<User[]> => {
    const response = await axios.get(`${API_BASE_URL}/api/users`);
    return response.data;
  },

  getById: async (id: string): Promise<User> => {
    const response = await axios.get(`${API_BASE_URL}/api/users/${id}`);
    return response.data;
  },

  create: async (user: CreateUserRequest): Promise<User> => {
    const response = await axios.post(`${API_BASE_URL}/api/users`, user);
    return response.data;
  },

  update: async (user: UpdateUserRequest): Promise<User> => {
    const response = await axios.put(
      `${API_BASE_URL}/api/users/${user.id}`,
      user
    );
    return response.data;
  },

  delete: async (id: string): Promise<void> => {
    await axios.delete(`${API_BASE_URL}/api/users/${id}`);
  },

  search: async (query: string): Promise<User[]> => {
    const response = await axios.get(`${API_BASE_URL}/api/users/search`, {
      params: { query },
    });
    return response.data;
  },

  getProfile: async (): Promise<UserProfileResponse> => {
    const response = await axios.get(`${API_URL}/profile`);
    return response.data;
  },

  updateProfile: async (data: UpdateProfileRequest): Promise<UserProfileResponse> => {
    const response = await axios.put(`${API_URL}/profile`, data);
    return response.data;
  },

  changePassword: async (data: { currentPassword: string; newPassword: string }): Promise<void> => {
    await axios.put(`${API_URL}/profile/change-password`, data);
  },

  updateSettings: async (settings: any): Promise<void> => {
    await axios.put(`${API_BASE_URL}/api/users/settings`, settings);
  },
};

export default userService; 