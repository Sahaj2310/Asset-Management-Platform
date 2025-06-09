import axios from 'axios';

export interface Company {
  id: string;
  name: string;
  description: string;
  logoPath: string;
  address: string;
  countryId: number;
  stateId: number;
  cityId: number;
  zipCode: string;
  phone: string;
  email: string;
  website: string;
}

export interface CompanyRegistrationRequest {
  name: string;
  description: string;
  logoPath?: string;
  address: string;
  countryId: number;
  stateId: number;
  cityId: number;
  zipCode: string;
  phone: string;
  email: string;
  website: string;
}

const API_URL = process.env.REACT_APP_API_URL || 'http://localhost:5000/api';

export const companyService = {
  getCompany: async (): Promise<Company> => {
    const response = await axios.get(`${API_URL}/company`);
    return response.data;
  },

  registerCompany: async (data: FormData): Promise<Company> => {
    const response = await axios.post(`${API_URL}/company/register`, data, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
    return response.data;
  },

  updateCompany: async (data: FormData): Promise<Company> => {
    const response = await axios.put(`${API_URL}/company/update`, data, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
    return response.data;
  },
}; 