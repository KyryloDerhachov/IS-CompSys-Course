import apiClient from './apiClient';

const API_URL = '/api/Feedback';

export const feedbackService = {

  createFeedback: async (command) => {
    return await apiClient.post(API_URL, command);
  },


  getAllFeedbacks: async () => {
    return await apiClient.get(API_URL);
  },

  respondToFeedback: async (id, responseText) => {
    return await apiClient.post(`${API_URL}/${id}/respond`, JSON.stringify(responseText), {
      headers: {
        'Content-Type': 'application/json'
      }
    });
  }
};