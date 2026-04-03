import json
import re
import pickle
import numpy as np
import pandas as pd
from sklearn.ensemble import RandomForestRegressor
from sklearn.preprocessing import StandardScaler, OneHotEncoder
from sklearn.pipeline import Pipeline
from sklearn.compose import ColumnTransformer
from sklearn.model_selection import train_test_split
from sklearn.metrics import r2_score, mean_absolute_error

print("=== Veri yükleniyor ===")
with open('../arabam_merged.json', encoding='utf-8') as f:
    data = json.load(f)

df = pd.DataFrame(data)
print(f"Toplam kayıt: {len(df)}")

# 1. Sadece ikinci el araçları al
df = df[df['arac_durumu'] == 'İkinci El'].copy()
print(f"İkinci el araç sayısı: {len(df)}")

# 2. İstenen sütunları seç
istenen_sutunlar = [
    'ilan_tarihi', 'fiyat', 'marka', 'seri', 'model', 'yil', 'km',
    'vites_tipi', 'yakit_tipi', 'kasa_tipi', 'motor_hacmi', 'motor_gucu',
    'cekis', 'agir_hasarli',
    'sag_arka_camurluk', 'arka_kaput', 'sol_arka_camurluk',
    'sag_arka_kapi', 'sag_on_kapi', 'tavan', 'sol_arka_kapi', 'sol_on_kapi',
    'sag_on_camurluk', 'motor_kaputu', 'sol_on_camurluk', 'on_tampon', 'arka_tampon'
]
df2 = df[istened_sutunlar := istenen_sutunlar].copy()

# 3. Sayısal dönüşümler
df2['fiyat'] = pd.to_numeric(df2['fiyat'], errors='coerce')
df2['yil'] = pd.to_numeric(df2['yil'], errors='coerce')
df2['km'] = pd.to_numeric(df2['km'], errors='coerce')

# 4. ilan_tarihi dönüşümü
ay_map = {
    'Ocak': '01', 'Şubat': '02', 'Mart': '03', 'Nisan': '04',
    'Mayıs': '05', 'Haziran': '06', 'Temmuz': '07', 'Ağustos': '08',
    'Eylül': '09', 'Ekim': '10', 'Kasım': '11', 'Aralık': '12'
}
df2['ilan_tarihi'] = df2['ilan_tarihi'].replace(ay_map, regex=True)
df2['ilan_tarihi'] = pd.to_datetime(df2['ilan_tarihi'], dayfirst=True, errors='coerce').dt.strftime('%d.%m.%Y')

# 5. agir_hasarli NaN → "Belirtilmemiş"
df2['agir_hasarli'] = df2['agir_hasarli'].fillna('Belirtilmemiş')

df3 = df2.copy()

# 6. Zorunlu kolonlarda NaN olanları düşür
df4 = df3.copy()
df4.dropna(subset=['model', 'vites_tipi', 'km', 'yakit_tipi', 'kasa_tipi'], inplace=True)
df4.dropna(subset=['motor_hacmi', 'motor_gucu', 'cekis'], inplace=True)

# 7. Cekis = "-" olanları düşür
df5 = df4[df4['cekis'] != '-'].copy()

# 8. Nadir kategorileri "Diger" olarak grupla
def nadir_kategorileri_grupla(df, kolon, esik_deger):
    counts = df[kolon].value_counts()
    nadir_olanlar = counts[counts < esik_deger].index
    df[kolon] = df[kolon].replace(nadir_olanlar, 'Diger')
    return df

df5 = nadir_kategorileri_grupla(df5, 'model', esik_deger=15)
df5 = nadir_kategorileri_grupla(df5, 'seri', esik_deger=10)

print(f"Model çeşit sayısı: {df5['model'].nunique()}")
print(f"Seri çeşit sayısı: {df5['seri'].nunique()}")

# 9. Motor hacmi ve motor gücü sayısal dönüşüm
def parse_motor_hacmi(val):
    if pd.isna(val):
        return np.nan
    val = str(val)
    sayilar = re.findall(r'\d+', val)
    if len(sayilar) == 0:
        return np.nan
    nums = [int(s) for s in sayilar]
    return sum(nums) / len(nums)

df5['motor_hacmi'] = df5['motor_hacmi'].apply(parse_motor_hacmi)
df5['motor_gucu'] = df5['motor_gucu'].astype(str).str.extract(r'(\d+)').astype(float)

# 10. Son temizlik
df6 = df5.copy()
df6.dropna(subset=['model', 'vites_tipi', 'km', 'yakit_tipi', 'kasa_tipi',
                   'motor_hacmi', 'motor_gucu', 'cekis', 'fiyat'], inplace=True)
df6 = df6[df6['cekis'] != '-']
print(f"Temizlik sonrası: {df6.shape}")

# 11. Outlier temizliği
Q1, Q3 = df6['fiyat'].quantile([0.25, 0.75])
IQR = Q3 - Q1
df6 = df6[(df6['fiyat'] >= Q1 - 1.5*IQR) & (df6['fiyat'] <= Q3 + 1.5*IQR)]

Q1_km, Q3_km = df6['km'].quantile([0.25, 0.75])
IQR_km = Q3_km - Q1_km
df6 = df6[df6['km'] <= Q3_km + 1.5*IQR_km]

def group_based_iqr(df, group_cols, target_col):
    df_filtered_list = []
    for name, group in df.groupby(group_cols):
        if len(group) > 5:
            Q1g = group[target_col].quantile(0.25)
            Q3g = group[target_col].quantile(0.75)
            IQRg = Q3g - Q1g
            mask = (group[target_col] >= Q1g - 1.5*IQRg) & (group[target_col] <= Q3g + 1.5*IQRg)
            df_filtered_list.append(group[mask])
        else:
            df_filtered_list.append(group)
    return pd.concat(df_filtered_list)

df6 = group_based_iqr(df6, ['marka', 'seri'], 'fiyat')
print(f"Outlier temizliği sonrası: {df6.shape}")

# 12. Yeni özellikler
boya_sutunlari = [
    'sag_arka_camurluk', 'arka_kaput', 'sol_arka_camurluk',
    'sag_arka_kapi', 'sag_on_kapi', 'tavan', 'sol_arka_kapi', 'sol_on_kapi',
    'sag_on_camurluk', 'motor_kaputu', 'sol_on_camurluk', 'on_tampon', 'arka_tampon'
]

df6['hasar_skoru'] = df6[boya_sutunlari].apply(lambda row: (row != 'Orijinal').sum(), axis=1)
df6['degisim_skoru'] = df6[boya_sutunlari].apply(lambda row: (row == 'Değişmiş').sum(), axis=1)
df6['marka_seri'] = df6['marka'] + '_' + df6['seri']

# 13. Gereksiz kolonları düşür
df6 = df6.drop(columns=['marka', 'seri', 'ilan_tarihi'])

# 14. agir_hasarli binary dönüşüm
df6['agir_hasarli_binary'] = df6['agir_hasarli'].map({
    'Evet': 1,
    'Hayır': 0,
    'Belirtilmemiş': 0
})
df6 = df6.drop(columns=['agir_hasarli'])

# 15. C# enum uyumlaştırma
cekis_map = {
    'Önden Çekiş': 'ÖndenÇekiş',
    'Arkadan İtiş': 'Arkadanİtiş',
    '4WD (Sürekli)': 'DörtÇeker',
    'AWD (Elektronik)': 'DörtÇeker'
}
df6['cekis'] = df6['cekis'].replace(cekis_map)

yakit_map = {'LPG & Benzin': 'Benzin_LPG'}
df6['yakit_tipi'] = df6['yakit_tipi'].replace(yakit_map)

kasa_map = {
    'Hatchback/5': 'Hatchback',
    'Hatchback/3': 'Hatchback',
    'Station Wagon': 'StationWagon',
    'Panelvan': 'Van',
    'Roadster': 'Cabrio'
}
df6['kasa_tipi'] = df6['kasa_tipi'].replace(kasa_map)

vites_map = {'Yarı Otomatik': 'YariOtomatik'}
df6['vites_tipi'] = df6['vites_tipi'].replace(vites_map)

boya_map = {
    'Boyanmış': 'Boyali',
    'Lokal Boyanmış': 'Boyali',
    'Belirtilmemiş': 'Orijinal'
}
for col in boya_sutunlari:
    df6[col] = df6[col].replace(boya_map)

# 16. Fiyat filtresi
df_temiz = df6[(df6['fiyat'] >= Q1 - 1.5*IQR) & (df6['fiyat'] <= Q3 + 1.5*IQR)].copy()
df_temiz = df_temiz[df_temiz['fiyat'] > 100000]
print(f"Final veri seti: {df_temiz.shape}")

# 17. Model eğitimi
df_final = df_temiz.copy()
df_final = df_final.dropna(subset=['fiyat'])

X = df_final.drop('fiyat', axis=1)
y = df_final['fiyat']

X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)
print(f"X_train: {X_train.shape}, X_test: {X_test.shape}")

sayisal_sutunlar = [
    'yil', 'km', 'motor_hacmi', 'motor_gucu',
    'hasar_skoru', 'degisim_skoru', 'agir_hasarli_binary'
]

kartegorik_sutunlar = [
    'marka_seri', 'model', 'vites_tipi', 'yakit_tipi', 'kasa_tipi',
    'cekis', 'sag_arka_camurluk', 'arka_kaput', 'sol_arka_camurluk',
    'sag_arka_kapi', 'sag_on_kapi', 'tavan',
    'sol_arka_kapi', 'sol_on_kapi', 'sag_on_camurluk',
    'motor_kaputu', 'sol_on_camurluk', 'on_tampon', 'arka_tampon'
]

preprocessor = ColumnTransformer(
    transformers=[
        ('num', StandardScaler(), sayisal_sutunlar),
        ('cat', OneHotEncoder(handle_unknown='ignore'), kartegorik_sutunlar)
    ])

model_pipeline = Pipeline(steps=[
    ('preprocessor', preprocessor),
    ('regressor', RandomForestRegressor(n_estimators=100, random_state=42, n_jobs=-1))
])

print("\n=== Model eğitiliyor... ===")
model_pipeline.fit(X_train, y_train)

y_pred = model_pipeline.predict(X_test)
r2 = r2_score(y_test, y_pred)
mae = mean_absolute_error(y_test, y_pred)

print(f"\n=== Sonuçlar ===")
print(f"R2 Skoru  : {r2:.4f}")
print(f"MAE       : {mae:,.0f} TL")

# 18. Modeli kaydet
with open('araba_modeli.pkl', 'wb') as dosya:
    pickle.dump(model_pipeline, dosya)

print("\nModel başarıyla 'araba_modeli.pkl' olarak kaydedildi!")
