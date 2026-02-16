const { getChannel } = require('../config/rabbitmq');
const { v4: uuidv4 } = require('uuid');

const PRICE_PREDICTION_QUEUE = 'price_prediction_queue';

/**
 * Mesaj kuyruğuna yayınla
 * @param {string} queue - Kuyruk adı
 * @param {object} data - Gönderilecek veri
 */
const publishMessage = async (queue, data) => {
    try {
        const channel = getChannel();
        await channel.assertQueue(queue, { durable: true });
        channel.sendToQueue(queue, Buffer.from(JSON.stringify(data)), {
            persistent: true,
        });
        console.log(`Mesaj kuyruğa gönderildi: ${queue}`);
    } catch (error) {
        console.error('Mesaj gönderme hatası:', error.message);
        throw error;
    }
};

/**
 * Kuyruktan mesaj dinle
 * @param {string} queue - Kuyruk adı
 * @param {function} handler - Mesaj işleme fonksiyonu
 */
const consumeMessage = async (queue, handler) => {
    try {
        const channel = getChannel();
        await channel.assertQueue(queue, { durable: true });
        channel.prefetch(1);

        channel.consume(queue, async (msg) => {
            if (msg) {
                try {
                    const data = JSON.parse(msg.content.toString());
                    await handler(data);
                    channel.ack(msg);
                } catch (error) {
                    console.error('Mesaj işleme hatası:', error.message);
                    // Hatalı mesajı dead letter queue'ya gönder
                    channel.nack(msg, false, false);
                }
            }
        });

        console.log(`Kuyruk dinleniyor: ${queue}`);
    } catch (error) {
        console.error('Kuyruk dinleme hatası:', error.message);
        throw error;
    }
};

/**
 * RPC pattern ile fiyat tahmini isteği gönder ve yanıt bekle
 * @param {object} carData - Araç bilgileri
 * @returns {Promise<object>} - Tahmin sonucu
 */
const requestPricePrediction = (carData) => {
    return new Promise(async (resolve, reject) => {
        try {
            const channel = getChannel();
            const correlationId = uuidv4();

            // Yanıt kuyruğu oluştur (exclusive, auto-delete)
            const { queue: replyQueue } = await channel.assertQueue('', {
                exclusive: true,
                autoDelete: true,
            });

            // Timeout: 30 saniye
            const timeout = setTimeout(() => {
                reject(new Error('Fiyat tahmin isteği zaman aşımına uğradı'));
            }, 30000);

            // Yanıt dinle
            channel.consume(replyQueue, (msg) => {
                if (msg && msg.properties.correlationId === correlationId) {
                    clearTimeout(timeout);
                    const result = JSON.parse(msg.content.toString());
                    resolve(result);
                    channel.ack(msg);
                }
            });

            // İsteği gönder
            await channel.assertQueue(PRICE_PREDICTION_QUEUE, { durable: true });
            channel.sendToQueue(
                PRICE_PREDICTION_QUEUE,
                Buffer.from(JSON.stringify(carData)),
                {
                    correlationId,
                    replyTo: replyQueue,
                    persistent: true,
                }
            );
        } catch (error) {
            reject(error);
        }
    });
};

module.exports = { publishMessage, consumeMessage, requestPricePrediction, PRICE_PREDICTION_QUEUE };
