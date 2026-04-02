import API from './axiosInstance';

export const getCars = (params) => API.get('/cars', { params });
export const getCarById = (id) => API.get(`/cars/${id}`);
export const createCar = (data) => API.post('/cars', data);
export const updateCar = (id, data) => API.put(`/cars/${id}`, data);
export const deleteCar = (id) => API.delete(`/cars/${id}`);

export const uploadImage = (file) => {
  const formData = new FormData();
  formData.append('file', file);
  return API.post('/api/upload/image', formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });
};
