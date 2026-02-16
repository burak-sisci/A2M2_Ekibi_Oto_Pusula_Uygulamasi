"""
A2M2 — Flask AI Servisi
Araç fiyat tahmini için REST API + RabbitMQ consumer sunar.
"""

from flask import Flask, request, jsonify
from flask_cors import CORS
from model.predictor import CarPricePredictor
import os
import json
import threading

app = Flask(__name__)
CORS(app)

# Model yükle veya eğit
predictor = CarPricePredictor()

model_path = os.path.join(os.path.dirname(__file__), 'model', 'car_price_model.pkl')
if os.path.exists(model_path):
    predictor.load_model()
else:
    print("Kayıtlı model bulunamadı, yeni model eğitiliyor...")
    predictor.train()
    predictor.save_model()


# ───────────── RabbitMQ Consumer ─────────────

def start_rabbitmq_consumer():
    """RabbitMQ'dan fiyat tahmin isteklerini dinle."""
    try:
        import pika

        rabbitmq_url = os.environ.get('RABBITMQ_URL', 'amqp://localhost:5672')
        params = pika.URLParameters(rabbitmq_url)
        connection = pika.BlockingConnection(params)
        channel = connection.channel()

        queue_name = 'price_prediction_queue'
        channel.queue_declare(queue=queue_name, durable=True)
        channel.basic_qos(prefetch_count=1)

        def on_request(ch, method, properties, body):
            """Gelen fiyat tahmin isteğini işle ve yanıtla."""
            try:
                car_data = json.loads(body)
                predicted_price = predictor.predict(car_data)

                response = {
                    'predictedPrice': predicted_price,
                    'currency': 'TRY',
                    'car': car_data,
                    'message': 'Fiyat tahmini başarılı',
                }
            except Exception as e:
                response = {'error': str(e)}

            # Yanıtı gönder (RPC pattern)
            ch.basic_publish(
                exchange='',
                routing_key=properties.reply_to,
                properties=pika.BasicProperties(
                    correlation_id=properties.correlation_id,
                ),
                body=json.dumps(response),
            )
            ch.basic_ack(delivery_tag=method.delivery_tag)

        channel.basic_consume(queue=queue_name, on_message_callback=on_request)
        print(f"RabbitMQ consumer başlatıldı, kuyruk dinleniyor: {queue_name}")
        channel.start_consuming()

    except ImportError:
        print("pika kütüphanesi bulunamadı, RabbitMQ consumer devre dışı")
    except Exception as e:
        print(f"RabbitMQ consumer hatası: {e}")


# ───────────── Flask API Endpoints ─────────────

@app.route('/predict', methods=['POST'])
def predict_price():
    """Araç fiyat tahmini endpoint'i."""
    try:
        data = request.get_json()

        required_fields = ['brand', 'model', 'year', 'km', 'fuelType', 'gearType']
        missing = [f for f in required_fields if f not in data]
        if missing:
            return jsonify({
                'error': f'Eksik alanlar: {", ".join(missing)}'
            }), 400

        car_data = {
            'brand': data['brand'],
            'model': data['model'],
            'year': int(data['year']),
            'km': int(data['km']),
            'fuelType': data['fuelType'],
            'gearType': data['gearType'],
        }

        predicted_price = predictor.predict(car_data)

        return jsonify({
            'predictedPrice': predicted_price,
            'currency': 'TRY',
            'car': car_data,
            'message': 'Fiyat tahmini başarılı',
        })

    except ValueError as e:
        return jsonify({'error': f'Geçersiz değer: {str(e)}'}), 400
    except Exception as e:
        return jsonify({'error': f'Sunucu hatası: {str(e)}'}), 500


@app.route('/health', methods=['GET'])
def health_check():
    """Sağlık kontrolü endpoint'i."""
    return jsonify({
        'status': 'OK',
        'service': 'A2M2 AI Price Prediction Service',
    })


@app.route('/brands', methods=['GET'])
def get_brands():
    """Desteklenen marka ve modelleri döndür."""
    try:
        df = predictor.load_data()
        brands = {}
        for _, row in df.iterrows():
            brand = row['brand']
            model = row['model']
            if brand not in brands:
                brands[brand] = set()
            brands[brand].add(model)

        # Set'leri listeye çevir
        brands = {k: sorted(list(v)) for k, v in sorted(brands.items())}
        return jsonify(brands)
    except Exception as e:
        return jsonify({'error': str(e)}), 500


if __name__ == '__main__':
    # RabbitMQ consumer'ı arka plan thread'inde başlat
    consumer_thread = threading.Thread(target=start_rabbitmq_consumer, daemon=True)
    consumer_thread.start()

    port = int(os.environ.get('PORT', 5001))
    app.run(host='0.0.0.0', port=port, debug=True)
