import axiosInstance from './axiosConfig';

export interface SiteResponse {
  id: string;
  name: string;
  description: string;
  address: string;
  countryId: number;
  countryName: string;
  stateId: number;
  stateName: string;
  cityId: number;
  cityName: string;
  zipCode: string;
  companyId: string;
  companyName: string;
}

export interface CreateSiteRequest {
  companyId: string;
  name: string;
  description: string;
  address: string;
  countryId: number;
  stateId: number;
  cityId: number;
  zipCode: string;
}

export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface PaginationParameters {
  pageNumber: number;
  pageSize: number;
}

export const siteService = {
  getAllSites: async (): Promise<SiteResponse[]> => {
    const response = await axiosInstance.get(`/Site/all`);
    return response.data;
  },

  getPaginatedSites: async (parameters: PaginationParameters): Promise<PaginatedResponse<SiteResponse>> => {
    const response = await axiosInstance.get(`/Site`, {
      params: parameters
    });
    return response.data;
  },

  getSitesByCompanyId: async (companyId: string): Promise<SiteResponse[]> => {
    const response = await axiosInstance.get(`/Site/company/${companyId}`);
    return response.data;
  },

  createSite: async (data: CreateSiteRequest): Promise<SiteResponse> => {
    const response = await axiosInstance.post(`/Site`, data);
    return response.data;
  },

  getSiteById: async (id: string): Promise<SiteResponse> => {
    const response = await axiosInstance.get(`/Site/${id}`);
    return response.data;
  },

  updateSite: async (id: string, data: CreateSiteRequest): Promise<void> => {
    await axiosInstance.put(`/Site/${id}`, data);
  },

  deleteSite: async (id: string): Promise<void> => {
    await axiosInstance.delete(`/Site/${id}`);
  },

  exportSitesToExcel: async (): Promise<Blob> => {
    const response = await axiosInstance.get('/api/sites/export', {
      responseType: 'blob'
    });
    return response.data;
  }
}; 