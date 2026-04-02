# ML Model - Kurulum Gereksinimleri

## Hizli Kurulum

```bash
cd ML_model
python -m venv venv
.\venv\Scripts\activate      # Windows
# source venv/bin/activate   # Linux/Mac
pip install -r requirements.txt
```

## Modeli Calistirma

```bash
.\venv\Scripts\activate
uvicorn api:app --host 0.0.0.0 --port 8000 --reload
```

## Bagimliliklari

| Paket         | Versiyon | Aciklama                        |
|---------------|----------|---------------------------------|
| numpy         | 2.4.3    | Sayisal hesaplama               |
| pandas        | 3.0.1    | Veri isleme ve analiz           |
| scikit-learn  | 1.8.0    | Makine ogrenmesi modeli         |
| matplotlib    | 3.10.8   | Grafik ve gorsellistirme        |
| seaborn       | 0.13.2   | Istatistiksel gorsellistirme    |
| fastapi       | 0.135.1  | API framework                   |
| uvicorn       | 0.34.0   | ASGI sunucu                     |

## Notlar

- Python 3.12+ gereklidir.
- Venv klasoru `.gitignore` icinde olmalidir.
- Paket versiyonlarini guncellemek icin: `pip freeze > requirements.txt`
