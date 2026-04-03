from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
import pandas as pd
import pickle
import os

app = FastAPI(
    title="Oto Pusula Fiyat Tahmini API",
    description="Ham veriyi işleyip tahmine dönüştüren servis."
)

# Lazy loading: model ilk istek gelince yüklenir (startup'ta RAM tüketmez)
model_pipeline = None

def get_model():
    global model_pipeline
    if model_pipeline is None:
        model_path = os.path.join(os.path.dirname(__file__) or '.', 'araba_modeli.pkl')
        try:
            with open(model_path, 'rb') as f:
                model_pipeline = pickle.load(f)
        except FileNotFoundError:
            model_pipeline = None
    return model_pipeline

@app.get("/health")
def health():
    return {"status": "healthy"}

class CarFeatures(BaseModel):
    # .NET tarafındaki SnakeCaseLower policy ile tam uyumlu alanlar
    ilan_tarihi: str 
    marka: str
    seri: str
    model: str
    yil: int
    km: float
    vites_tipi: str
    yakit_tipi: str
    kasa_tipi: str
    renk: str 
    motor_hacmi: str  
    motor_gucu: str   
    cekis: str
    arac_durumu: str 
    ort_yakit_tuketimi: str 
    yakit_deposu: str
    agir_hasarli: str 
    boya_degisen: str 
    takasa_uygun: str 
    kimden: str 
    
    # Boya ve Değişen Detayları (Default "Orijinal")
    sag_arka_camurluk: str = "Orijinal"
    arka_kaput: str = "Orijinal"
    sol_arka_camurluk: str = "Orijinal"
    sag_arka_kapi: str = "Orijinal"
    sag_on_kapi: str = "Orijinal"
    tavan: str = "Orijinal"
    sol_arka_kapi: str = "Orijinal"
    sol_on_kapi: str = "Orijinal"
    sag_on_camurluk: str = "Orijinal"
    motor_kaputu: str = "Orijinal"
    sol_on_camurluk: str = "Orijinal"
    on_tampon: str = "Orijinal"
    arka_tampon: str = "Orijinal"

boya_sutunlari = [
    'sag_arka_camurluk', 'arka_kaput', 'sol_arka_camurluk',
    'sag_arka_kapi', 'sag_on_kapi', 'tavan', 'sol_arka_kapi', 'sol_on_kapi',
    'sag_on_camurluk', 'motor_kaputu', 'sol_on_camurluk', 'on_tampon', 'arka_tampon'
]

def feature_engineering_yap(data: dict) -> pd.DataFrame:
    import re
    import numpy as np
    df = pd.DataFrame([data])

    # 1. Motor hacmi: train_model ile aynı parse_motor_hacmi mantığı
    def parse_motor_hacmi(val):
        if pd.isna(val):
            return np.nan
        val = str(val)
        sayilar = re.findall(r'\d+', val)
        if len(sayilar) == 0:
            return np.nan
        nums = [int(s) for s in sayilar]
        return sum(nums) / len(nums)

    df['motor_hacmi'] = df['motor_hacmi'].apply(parse_motor_hacmi)
    df['motor_gucu'] = df['motor_gucu'].astype(str).str.extract(r'(\d+)').astype(float)

    # 2. Enum uyumlaştırma (train_model.py ile birebir aynı)
    cekis_map = {
        'Önden Çekiş': 'ÖndenÇekiş',
        'Arkadan İtiş': 'Arkadanİtiş',
        '4WD (Sürekli)': 'DörtÇeker',
        'AWD (Elektronik)': 'DörtÇeker'
    }
    df['cekis'] = df['cekis'].replace(cekis_map)

    yakit_map = {'LPG & Benzin': 'Benzin_LPG'}
    df['yakit_tipi'] = df['yakit_tipi'].replace(yakit_map)

    kasa_map = {
        'Hatchback/5': 'Hatchback',
        'Hatchback/3': 'Hatchback',
        'Station Wagon': 'StationWagon',
        'Panelvan': 'Van',
        'Roadster': 'Cabrio'
    }
    df['kasa_tipi'] = df['kasa_tipi'].replace(kasa_map)

    vites_map = {'Yarı Otomatik': 'YariOtomatik'}
    df['vites_tipi'] = df['vites_tipi'].replace(vites_map)

    boya_map = {
        'Boyanmış': 'Boyali',
        'Lokal Boyanmış': 'Boyali',
        'Belirtilmemiş': 'Orijinal'
    }
    for col in boya_sutunlari:
        df[col] = df[col].replace(boya_map)

    # 3. Hasar Skoru: Orijinal olmayan her parça 1 puan
    df['hasar_skoru'] = df[boya_sutunlari].apply(
        lambda row: (row != 'Orijinal').sum(), axis=1
    )

    # 4. Değişim Skoru: Sadece 'Değişmiş' olanlar
    df['degisim_skoru'] = df[boya_sutunlari].apply(
        lambda row: (row == 'Değişmiş').sum(), axis=1
    )

    # 5. Marka Seri Birleştirme (train_model ile aynı: '_' ayracı)
    df['marka_seri'] = df['marka'] + '_' + df['seri']

    # 6. Ağır Hasarlı Binary Dönüşümü
    df['agir_hasarli_binary'] = df['agir_hasarli'].apply(
        lambda x: 1 if x == "Evet" else 0
    )

    # 7. Sütun Sıralaması (train_model X_train sırası ile birebir aynı)
    beklenen_sutunlar = [
        'model', 'yil', 'km', 'vites_tipi', 'yakit_tipi', 'kasa_tipi',
        'motor_hacmi', 'motor_gucu', 'cekis', 'sag_arka_camurluk',
        'arka_kaput', 'sol_arka_camurluk', 'sag_arka_kapi', 'sag_on_kapi',
        'tavan', 'sol_arka_kapi', 'sol_on_kapi', 'sag_on_camurluk',
        'motor_kaputu', 'sol_on_camurluk', 'on_tampon', 'arka_tampon',
        'hasar_skoru', 'degisim_skoru', 'marka_seri', 'agir_hasarli_binary'
    ]

    return df[beklenen_sutunlar]

@app.post("/predict")
def predict_price(car: CarFeatures):
    pipeline = get_model()
    if pipeline is None:
        raise HTTPException(status_code=500, detail="Model dosyası bulunamadı.")

    try:
        # 1. Veriyi işle
        processed_df = feature_engineering_yap(car.model_dump())

        # 2. Tahmin yap
        tahmin = pipeline.predict(processed_df)[0]
        
        # 3. Para formatını hazırla
        formatli_fiyat = f"{float(tahmin):,.2f}".replace(",", "X").replace(".", ",").replace("X", ".")
        
        # --- .NET DTO YAPISIYLA TAM UYUM (SnakeCaseLower dikkate alınarak) ---
        return {
            "durum": "basarili",
            "tahmin_sonucu": {
                "fiyat_etiketi": f"{formatli_fiyat} TL",
                "birim": "TL"
            }
        }

    except Exception as e:
        raise HTTPException(status_code=400, detail=f"Hata: {str(e)}")