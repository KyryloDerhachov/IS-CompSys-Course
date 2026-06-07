import apiClient from './apiClient';

export const reportsService = {
  getReceipts: () => apiClient.get('/api/Sales/receipts'),
  
  getProductClasses: () => apiClient.get('/api/product-classes?includeInactive=false'),
  
  getProductCategories: () => apiClient.get('/api/product-categories?includeInactive=false'),
  
  getProducts: () => apiClient.get('/api/Products?includeInactive=true'),
  
  getReturns: () => apiClient.get('/api/sales/returns')
};