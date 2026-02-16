import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_URL || '/api';

const api = axios.create({
    baseURL: API_BASE_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});

// Request interceptor — Token ekleme
api.interceptors.request.use((config) => {
    const user = JSON.parse(localStorage.getItem('user') || 'null');
    if (user?.token) {
        config.headers.Authorization = `Bearer ${user.token}`;
    }
    return config;
});

// Response interceptor — Hata yönetimi
api.interceptors.response.use(
    (response) => response,
    (error) => {
        if (error.response?.status === 401) {
            localStorage.removeItem('user');
            window.location.href = '/login';
        }
        return Promise.reject(error);
    }
);

// Auth API
export const authAPI = {
    register: (data) => api.post('/auth/register', data),
    login: (data) => api.post('/auth/login', data),
    logout: () => api.post('/auth/logout'),
    getMe: () => api.get('/auth/me'),
};

// Listings API
export const listingsAPI = {
    getAll: (params) => api.get('/listings', { params }),
    getById: (id) => api.get(`/listings/${id}`),
    create: (data) => api.post('/listings', data),
    update: (id, data) => api.put(`/listings/${id}`, data),
    delete: (id) => api.delete(`/listings/${id}`),
};

// Favorites API
export const favoritesAPI = {
    getAll: () => api.get('/favorites'),
    add: (listingId) => api.post(`/favorites/${listingId}`),
    remove: (listingId) => api.delete(`/favorites/${listingId}`),
};

// Share API
export const shareAPI = {
    getShareInfo: (listingId) => api.get(`/share/${listingId}`),
};

// AI API
export const aiAPI = {
    predictPrice: (data) => api.post('/ai/predict-price', data),
};

export default api;
