const amqp = require('amqplib');

let connection = null;
let channel = null;

const RABBITMQ_URL = process.env.RABBITMQ_URL || 'amqp://localhost:5672';

/**
 * RabbitMQ bağlantısını başlat
 */
const connectRabbitMQ = async () => {
    try {
        connection = await amqp.connect(RABBITMQ_URL);
        channel = await connection.createChannel();

        // Bağlantı kapandığında yeniden bağlan
        connection.on('close', () => {
            console.error('RabbitMQ bağlantısı kapandı. Yeniden bağlanılıyor...');
            setTimeout(connectRabbitMQ, 5000);
        });

        connection.on('error', (err) => {
            console.error('RabbitMQ bağlantı hatası:', err.message);
        });

        console.log('RabbitMQ bağlantısı başarılı');
        return channel;
    } catch (error) {
        console.error('RabbitMQ bağlantı hatası:', error.message);
        setTimeout(connectRabbitMQ, 5000);
    }
};

/**
 * Mevcut channel'ı döndür
 */
const getChannel = () => {
    if (!channel) {
        throw new Error('RabbitMQ channel henüz oluşturulmadı');
    }
    return channel;
};

/**
 * Bağlantıyı kapat
 */
const closeRabbitMQ = async () => {
    try {
        if (channel) await channel.close();
        if (connection) await connection.close();
        console.log('RabbitMQ bağlantısı kapatıldı');
    } catch (error) {
        console.error('RabbitMQ kapatma hatası:', error.message);
    }
};

module.exports = { connectRabbitMQ, getChannel, closeRabbitMQ };
