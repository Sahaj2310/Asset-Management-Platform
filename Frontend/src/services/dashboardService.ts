import axios from 'axios';

export interface DashboardStats {
  totalAssets: number;
  totalSites: number;
  totalUsers: number;
  recentActivities: Activity[];
}

export interface Activity {
  id: string;
  type: string;
  description: string;
  timestamp: string;
  userId: string;
  userName: string;
}

const API_URL = process.env.REACT_APP_API_URL || 'http://localhost:5000/api';

export const dashboardService = {
  getStats: async (): Promise<DashboardStats> => {
    const response = await axios.get(`${API_URL}/dashboard/stats`);
    return response.data;
  },

  getRecentActivities: async (): Promise<Activity[]> => {
    const response = await axios.get(`${API_URL}/dashboard/activities`);
    return response.data;
  },

  getAssetValue: async (): Promise<number> => {
    const response = await axios.get(`${API_URL}/dashboard/asset-value`);
    return response.data;
  },
}; 