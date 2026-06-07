import apiClient from './apiClient';

export const salesService = {
  createReceipt: (receiptData) => {
    return apiClient.post('/api/Sales/receipts', receiptData);
  },

  getReceiptById: (id) => {
    return apiClient.get(`/api/Sales/receipts/${id}`);
  },

  getHistory: () => {
    return apiClient.get('/api/Sales/receipts');
  },

  processReturn: (receiptId, returnData) => {
    return apiClient.post(`/api/Sales/receipts/${receiptId}/returns`, returnData);
  }
};