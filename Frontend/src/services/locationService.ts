import axios from 'axios';

export interface LocationDto {
  id: number;
  name: string;
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
}; 