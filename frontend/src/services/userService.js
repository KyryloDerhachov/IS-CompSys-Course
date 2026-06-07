import apiClient from './apiClient';

export const userService = {
  login: (login, password) => 
    apiClient.post('/api/users/login', { login, password }),

  createUser: (userData) => 
    apiClient.post('/api/users', userData),

  getAllUsers: (includeInactive = false) => 
    apiClient.get('/api/users', { params: { includeInactive } }),

  getUserById: (id) => 
    apiClient.get(`/api/users/${id}`),


  deactivateUser: (id) => 
    apiClient.put(`/api/users/${id}/deactivate`),

  updateUserRoles: (id, roleIds) => 
    apiClient.put(`/api/users/${id}/roles`, roleIds),

  updateUserProfile: (id, profileData) => 
    apiClient.put(`/api/users/${id}`, profileData),
  
  getAvailableRoles: () => 
    apiClient.get('/api/Roles')
};