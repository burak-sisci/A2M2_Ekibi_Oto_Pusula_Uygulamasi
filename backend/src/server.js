const express = require('express');
const cors = require('cors');
const dotenv = require('dotenv');
const connectDB = require('./config/db');
const { connectRedis } = require('./config/redis');
const { connectRabbitMQ } = require('./config/rabbitmq');
const { requestPricePrediction } = require('./services/messageQueue');
const { cacheMiddleware, invalidateCache } = require('./middleware/cacheMiddleware');

// Route imports
const authRoutes = require('./routes/authRoutes');
const listingRoutes = require('./routes/listingRoutes');
const favoriteRoutes = require('./routes/favoriteRoutes');
const shareRoutes = require('./routes/shareRoutes');

// Ortam değişkenlerini yükle
dotenv.config();

// Veritabanı ve altyapı bağlantıları
const startServices = async () => {
    await connectDB();

    // Redis bağlantısı (hata olursa uygulama yine çalışsın)
    try {
        await connectRedis();
    } catch (err) {
        console.warn('Redis bağlantısı kurulamadı, cache devre dışı:', err.message);
    }

    // RabbitMQ bağlantısı (hata olursa fallback HTTP kullanılır)
    try {
        await connectRabbitMQ();
    } catch (err) {
        console.warn('RabbitMQ bağlantısı kurulamadı, HTTP fallback aktif:', err.message);
    }
};

startServices();

const app = express();

// Middleware
app.use(cors());
app.use(express.json());
app.use(express.urlencoded({ extended: true }));

// Routes — İlan listesi ve detay cache'li
app.use('/api/auth', authRoutes);
app.use('/api/listings', listingRoutes);
app.use('/api/favorites', favoriteRoutes);
app.use('/api/share', shareRoutes);

// AI servisine fiyat tahmin isteği — RabbitMQ ile (fallback: HTTP)
app.post('/api/ai/predict-price', async (req, res) => {
    try {
        // Önce RabbitMQ üzerinden dene
        try {
            const result = await requestPricePrediction(req.body);
            return res.json(result);
        } catch (mqError) {
            console.warn('RabbitMQ isteği başarısız, HTTP fallback:', mqError.message);
        }

        // Fallback: Doğrudan HTTP ile AI servisine bağlan
        const aiServiceUrl = process.env.AI_SERVICE_URL || 'http://localhost:5001';
        const response = await fetch(`${aiServiceUrl}/predict`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(req.body),
        });
        const data = await response.json();
        res.json(data);
    } catch (error) {
        res.status(500).json({ message: 'AI servisi ile iletişim hatası', error: error.message });
    }
});

// Sağlık kontrolü
app.get('/api/health', (req, res) => {
    res.json({
        status: 'OK',
        message: 'A2M2 API çalışıyor',
        services: {
            api: true,
            timestamp: new Date().toISOString(),
        },
    });
});

const PORT = process.env.PORT || 5000;
app.listen(PORT, () => {
    console.log(`A2M2 API sunucusu ${PORT} portunda çalışıyor`);
});
