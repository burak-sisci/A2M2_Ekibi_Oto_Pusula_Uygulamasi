import API from './axiosInstance';

export const getLists = () => API.get('/lists');
export const createList = (data) => API.post('/lists', data);
export const addItemToList = (id, data) => API.put(`/lists/${id}/items`, data);
export const deleteList = (id) => API.delete(`/lists/${id}`);
