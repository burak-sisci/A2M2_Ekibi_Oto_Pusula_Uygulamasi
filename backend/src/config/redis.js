const Redis = require('ioredis');

const REDIS_URL = process.env.REDIS_URL || 'redis://localhost:6379';

const redis = new Redis(REDIS_URL, {
    maxRetriesPerRequest: 3,
    retryStrategy(times) {
        const delay = Math.min(times * 200, 5000);
        return delay;
    },
    lazyConnect: true,
});

redis.on('connect', () => {
    console.log('Redis bağlantısı başarılı');
});

redis.on('error', (err) => {
    console.error('Redis bağlantı hatası:', err.message);
});

/**
 * Redis bağlantısını başlat
 */
const connectRedis = async () => {
    try {
        await redis.connect();
    } catch (error) {
        console.error('Redis başlatma hatası:', error.message);
    }
};

module.exports = { redis, connectRedis };
