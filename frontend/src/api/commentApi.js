import API from './axiosInstance';

export const getComments = (carId, params) => API.get(`/cars/${carId}/comments`, { params });
export const getComment = (carId, id) => API.get(`/cars/${carId}/comments/${id}`);
export const addComment = (carId, data) => API.post(`/cars/${carId}/comments`, data);
export const updateComment = (carId, id, data) => API.put(`/cars/${carId}/comments/${id}`, data);
export const deleteComment = (carId, id) => API.delete(`/cars/${carId}/comments/${id}`);
