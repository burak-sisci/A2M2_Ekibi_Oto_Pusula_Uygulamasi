const { redis } = require('../config/redis');

const DEFAULT_TTL = 300; // 5 dakika

/**
 * Cache'den veri oku
 * @param {string} key - Cache anahtarı
 * @returns {any|null} - Cache'deki veri veya null
 */
const get = async (key) => {
    try {
        const data = await redis.get(key);
        return data ? JSON.parse(data) : null;
    } catch (error) {
        console.error('Cache okuma hatası:', error.message);
        return null;
    }
};

/**
 * Cache'e veri yaz
 * @param {string} key - Cache anahtarı
 * @param {any} value - Saklanacak veri
 * @param {number} ttl - Yaşam süresi (saniye), varsayılan 5 dk
 */
const set = async (key, value, ttl = DEFAULT_TTL) => {
    try {
        await redis.set(key, JSON.stringify(value), 'EX', ttl);
    } catch (error) {
        console.error('Cache yazma hatası:', error.message);
    }
};

/**
 * Tek bir cache anahtarını sil
 * @param {string} key - Silinecek anahtar
 */
const del = async (key) => {
    try {
        await redis.del(key);
    } catch (error) {
        console.error('Cache silme hatası:', error.message);
    }
};

/**
 * Pattern'e uyan tüm cache anahtarlarını sil
 * @param {string} pattern - Glob pattern (ör: "listings:*")
 */
const delPattern = async (pattern) => {
    try {
        const keys = await redis.keys(pattern);
        if (keys.length > 0) {
            await redis.del(...keys);
        }
    } catch (error) {
        console.error('Cache pattern silme hatası:', error.message);
    }
};

module.exports = { get, set, del, delPattern };
