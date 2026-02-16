/**
 * A2M2 — Mobil API Servisi
 * Web'deki api.js ile aynı endpoint'leri kullanır.
 */

import axios from 'axios';
import AsyncStorage from '@react-native-async-storage/async-storage';

const API_BASE_URL = 'http://localhost:5000/api'; // Geliştirme ortamı

const api = axios.create({
    baseURL: API_BASE_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});

// Request interceptor — Token ekleme
api.interceptors.request.use(async (config) => {
    const userJson = await AsyncStorage.getItem('user');
    if (userJson) {
        const user = JSON.parse(userJson);
        if (user?.token) {
            config.headers.Authorization = `Bearer ${user.token}`;
        }
    }
    return config;
});

// Auth API
export const authAPI = {
    register: (data) => api.post('/auth/register', data),
    login: (data) => api.post('/auth/login', data),
    logout: () => api.post('/auth/logout'),
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
