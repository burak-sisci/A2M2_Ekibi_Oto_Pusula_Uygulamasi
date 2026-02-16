const cacheService = require('../services/cacheService');

/**
 * Route-level cache middleware
 * Belirli endpoint'leri cache'ler ve cache invalidation yapar
 *
 * @param {number} ttl - Cache yaşam süresi (saniye)
 * @returns {function} Express middleware
 */
const cacheMiddleware = (ttl = 300) => {
    return async (req, res, next) => {
        // Sadece GET isteklerini cache'le
        if (req.method !== 'GET') {
            return next();
        }

        // Cache anahtarı: URL + query parametreleri
        const cacheKey = `cache:${req.originalUrl}`;

        try {
            const cached = await cacheService.get(cacheKey);

            if (cached) {
                console.log(`Cache hit: ${cacheKey}`);
                return res.json(cached);
            }

            // Orijinal res.json'ı yakala ve cache'e yaz
            const originalJson = res.json.bind(res);
            res.json = (data) => {
                // Sadece başarılı yanıtları cache'le
                if (res.statusCode >= 200 && res.statusCode < 300) {
                    cacheService.set(cacheKey, data, ttl);
                    console.log(`Cache set: ${cacheKey} (TTL: ${ttl}s)`);
                }
                return originalJson(data);
            };

            next();
        } catch (error) {
            console.error('Cache middleware hatası:', error.message);
            next();
        }
    };
};

/**
 * Cache invalidation middleware
 * CUD işlemlerinden sonra ilgili cache'leri temizler
 *
 * @param {string[]} patterns - Temizlenecek cache pattern'leri
 * @returns {function} Express middleware
 */
const invalidateCache = (...patterns) => {
    return async (req, res, next) => {
        const originalJson = res.json.bind(res);

        res.json = async (data) => {
            // Başarılı CUD işlemlerinden sonra cache temizle
            if (res.statusCode >= 200 && res.statusCode < 300) {
                for (const pattern of patterns) {
                    await cacheService.delPattern(pattern);
                    console.log(`Cache invalidated: ${pattern}`);
                }
            }
            return originalJson(data);
        };

        next();
    };
};

module.exports = { cacheMiddleware, invalidateCache };
