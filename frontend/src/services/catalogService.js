import apiClient from './apiClient';

export const catalogService = {
  getClasses: (includeInactive = false) => 
    apiClient.get(`/api/product-classes?includeInactive=${includeInactive}`),
  createClass: (data) => 
    apiClient.post('/api/product-classes', data),
  updateClassDescription: (id, data) => 
    apiClient.put(`/api/product-classes/${id}/description`, data),
  deactivateClass: (id, version) => 
    apiClient.put(`/api/product-classes/${id}/deactivate?version=${version}`),
  activateClass: (id, version) => 
    apiClient.put(`/api/product-classes/${id}/activate?version=${version}`),

  getCategories: (includeInactive = false) => 
    apiClient.get(`/api/product-categories?includeInactive=${includeInactive}`),
  createCategory: (data) => 
    apiClient.post('/api/product-categories', data),
  updateCategoryDescription: (id, data) => 
    apiClient.put(`/api/product-categories/${id}/description`, data),
  deactivateCategory: (id, version) => 
    apiClient.put(`/api/product-categories/${id}/deactivate?version=${version}`),
  activateCategory: (id, version) => 
    apiClient.put(`/api/product-categories/${id}/activate?version=${version}`),


  getProducts: (includeInactive = false) => 
    apiClient.get(`/api/Products?includeInactive=${includeInactive}`),
  createProduct: (data) => 
    apiClient.post('/api/Products', data),
  updateProduct: (id, data) => 
    apiClient.put(`/api/Products/${id}`, data),
  deactivateProduct: (id, version) => 
    apiClient.put(`/api/Products/${id}/deactivate?version=${version}`),
  activateProduct: (id, version) => 
    apiClient.put(`/api/Products/${id}/activate?version=${version}`)
};