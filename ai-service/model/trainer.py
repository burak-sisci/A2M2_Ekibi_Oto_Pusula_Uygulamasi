"""
A2M2 — Model Eğitim Scripti
Kullanım: python -m model.trainer
"""

from predictor import CarPricePredictor


def main():
    print("=" * 50)
    print("A2M2 — Araç Fiyat Tahmin Modeli Eğitimi")
    print("=" * 50)

    predictor = CarPricePredictor()

    # Modeli eğit
    metrics = predictor.train()

    # Modeli kaydet
    predictor.save_model()

    # Test tahmini
    test_car = {
        'brand': 'Toyota',
        'model': 'Corolla',
        'year': 2020,
        'km': 50000,
        'fuelType': 'Benzin',
        'gearType': 'Otomatik',
    }

    predicted_price = predictor.predict(test_car)
    print(f"\nTest Tahmini:")
    print(f"  Araç: {test_car['brand']} {test_car['model']} ({test_car['year']})")
    print(f"  Tahmini Fiyat: {predicted_price:,.0f} TL")
    print("=" * 50)


if __name__ == '__main__':
    main()
