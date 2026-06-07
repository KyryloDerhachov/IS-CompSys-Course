import apiClient from './apiClient';

export const stockService = {

  getBatches: (productId = null) => {
    const url = productId ? `/api/Batches?productId=${productId}` : '/api/Batches';
    return apiClient.get(url);
  },

  getExpiringStocks: (daysThreshold = 7) => {
    return apiClient.get(`/api/Batches/expiring-stocks?daysThreshold=${daysThreshold}`);
  },

  getBatchById: (id) => {
    return apiClient.get(`/api/Batches/${id}`);
  },

  reduceStock: (id, quantity, version) => {
    return apiClient.put(`/api/Batches/${id}/reduce-stock`, {
      quantity: parseFloat(quantity),
      version: parseInt(version, 10)
    });
  },

 
  createSupply: (supplyData) => {
    return apiClient.post('/api/Supplies', supplyData);
  },

  postSupply: (id, version) => {
    return apiClient.put(`/api/Supplies/${id}/post?version=${parseInt(version, 10)}`);
  }
};