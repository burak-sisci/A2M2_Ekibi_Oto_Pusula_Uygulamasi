"""
A2M2 — AI Model Unit Testleri
Kullanım: python -m pytest tests/test_predictor.py
"""

import sys
import os
sys.path.insert(0, os.path.join(os.path.dirname(__file__), '..'))

from model.predictor import CarPricePredictor


def test_load_data():
    """Veri setinin yüklenip yüklenemediğini test et."""
    predictor = CarPricePredictor()
    df = predictor.load_data()
    assert len(df) > 0, "Veri seti boş olmamalı"
    assert 'brand' in df.columns, "brand kolonu bulunmalı"
    assert 'price' in df.columns, "price kolonu bulunmalı"


def test_train_model():
    """Model eğitimini test et."""
    predictor = CarPricePredictor()
    metrics = predictor.train()
    assert 'mae' in metrics, "MAE metriği döndürülmeli"
    assert 'r2' in metrics, "R2 metriği döndürülmeli"
    assert metrics['mae'] >= 0, "MAE negatif olmamalı"


def test_predict():
    """Fiyat tahminini test et."""
    predictor = CarPricePredictor()
    predictor.train()

    test_car = {
        'brand': 'Toyota',
        'model': 'Corolla',
        'year': 2020,
        'km': 50000,
        'fuelType': 'Benzin',
        'gearType': 'Otomatik',
    }

    price = predictor.predict(test_car)
    assert price > 0, "Tahmin edilen fiyat pozitif olmalı"
    assert isinstance(price, float), "Tahmin float olmalı"


if __name__ == '__main__':
    test_load_data()
    print("✅ test_load_data geçti")
    test_train_model()
    print("✅ test_train_model geçti")
    test_predict()
    print("✅ test_predict geçti")
    print("\nTüm testler başarılı!")
