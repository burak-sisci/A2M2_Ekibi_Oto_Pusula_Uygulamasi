"""
A2M2 — Araç Fiyat Tahmin Modeli
Anıl tarafından geliştirilecek modülün iskeletidir.
"""

import json
import os
import numpy as np
import pandas as pd
from sklearn.ensemble import GradientBoostingRegressor
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import LabelEncoder
from sklearn.metrics import mean_absolute_error, r2_score
import joblib

BASE_DIR = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
DATA_DIR = os.path.join(BASE_DIR, 'data')
MODEL_DIR = os.path.dirname(os.path.abspath(__file__))


class CarPricePredictor:
    """Araç fiyat tahmin sınıfı."""

    def __init__(self):
        self.model = None
        self.label_encoders = {}
        self.feature_columns = ['brand', 'model', 'year', 'km', 'fuelType', 'gearType']
        self.categorical_columns = ['brand', 'model', 'fuelType', 'gearType']

    def load_data(self):
        """JSON veri setini yükle ve DataFrame'e dönüştür."""
        dataset_path = os.path.join(DATA_DIR, 'cars_dataset.json')
        with open(dataset_path, 'r', encoding='utf-8') as f:
            data = json.load(f)
        return pd.DataFrame(data)

    def preprocess(self, df):
        """Veriyi model için ön işle."""
        df_processed = df.copy()

        for col in self.categorical_columns:
            if col not in self.label_encoders:
                self.label_encoders[col] = LabelEncoder()
                df_processed[col] = self.label_encoders[col].fit_transform(df_processed[col])
            else:
                df_processed[col] = self.label_encoders[col].transform(df_processed[col])

        return df_processed

    def train(self):
        """Modeli eğit."""
        df = self.load_data()
        df_processed = self.preprocess(df)

        X = df_processed[self.feature_columns]
        y = df_processed['price']

        X_train, X_test, y_train, y_test = train_test_split(
            X, y, test_size=0.2, random_state=42
        )

        self.model = GradientBoostingRegressor(
            n_estimators=100,
            learning_rate=0.1,
            max_depth=5,
            random_state=42,
        )
        self.model.fit(X_train, y_train)

        # Değerlendirme
        y_pred = self.model.predict(X_test)
        mae = mean_absolute_error(y_test, y_pred)
        r2 = r2_score(y_test, y_pred)

        print(f"Model Eğitim Tamamlandı!")
        print(f"  MAE (Ortalama Mutlak Hata): {mae:,.0f} TL")
        print(f"  R² Skoru: {r2:.4f}")

        return {'mae': mae, 'r2': r2}

    def predict(self, car_data: dict) -> float:
        """Tek bir araç için fiyat tahmini yap."""
        if self.model is None:
            raise ValueError("Model henüz eğitilmedi! Önce train() çağırın.")

        df = pd.DataFrame([car_data])
        df_processed = self.preprocess(df)
        X = df_processed[self.feature_columns]
        prediction = self.model.predict(X)[0]
        return round(max(prediction, 0), 2)

    def save_model(self, filename='car_price_model.pkl'):
        """Modeli dosyaya kaydet."""
        model_path = os.path.join(MODEL_DIR, filename)
        joblib.dump({
            'model': self.model,
            'label_encoders': self.label_encoders,
        }, model_path)
        print(f"Model kaydedildi: {model_path}")

    def load_model(self, filename='car_price_model.pkl'):
        """Kaydedilmiş modeli yükle."""
        model_path = os.path.join(MODEL_DIR, filename)
        if not os.path.exists(model_path):
            raise FileNotFoundError(f"Model dosyası bulunamadı: {model_path}")

        data = joblib.load(model_path)
        self.model = data['model']
        self.label_encoders = data['label_encoders']
        print(f"Model yüklendi: {model_path}")
