import API from './axiosInstance';

export const predictPrice = (data) => API.post('/api/prediction/predict', data);
