import axios from 'axios';
import axiosInstance from './axiosConfig';

export interface LocationDto {
  id: number;
  name: string;
}

export interface CreateLocationRequest {
  name: string;
  siteId: string;
  companyId: string;
}

export interface LocationResponse {
  id: string;
  name: string;
  siteId: string;
  siteName: string;
  companyId: string;
  companyName: string;
}

const API_URL = process.env.REACT_APP_API_URL || 'http://localhost:5000/api';

export const locationService = {
  getCountries: async (): Promise<LocationDto[]> => {
    const response = await axios.get(`${API_URL}/locations/countries`);
    return response.data;
  },

  getStates: async (countryId: number): Promise<LocationDto[]> => {
    const response = await axios.get(`${API_URL}/locations/states`, {
      params: { countryId },
    });
    return response.data;
  },

  getCities: async (stateId: number): Promise<LocationDto[]> => {
    const response = await axios.get(`${API_URL}/locations/cities`, {
      params: { stateId },
    });
    return response.data;
  },

  createLocation: async (data: CreateLocationRequest): Promise<LocationResponse> => {
    const response = await axiosInstance.post(`/Location`, data);
    return response.data;
  },

  getLocationById: async (id: string): Promise<LocationResponse> => {
    const response = await axiosInstance.get(`/Location/${id}`);
    return response.data;
  },

  getAllLocations: async (): Promise<LocationResponse[]> => {
    const response = await axiosInstance.get(`/Location`);
    return response.data;
  },

  getLocationsBySiteId: async (siteId: string): Promise<LocationResponse[]> => {
    const response = await axiosInstance.get(`/Location/site/${siteId}`);
    return response.data;
  },

  getLocationsByCompanyId: async (companyId: string): Promise<LocationResponse[]> => {
    const response = await axiosInstance.get(`/Location/company/${companyId}`);
    return response.data;
  },

  updateLocation: async (id: string, data: CreateLocationRequest): Promise<void> => {
    await axiosInstance.put(`/Location/${id}`, data);
  },

  deleteLocation: async (id: string): Promise<void> => {
    await axiosInstance.delete(`/Location/${id}`);
  },
}; 