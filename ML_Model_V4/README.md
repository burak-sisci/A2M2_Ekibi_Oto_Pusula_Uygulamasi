---
title: OtoPusula ML
emoji: 🚗
colorFrom: red
colorTo: orange
sdk: docker
app_port: 7860
pinned: false
---

# OtoPusula — Fiyat Tahmini Servisi

FastAPI + sklearn RandomForest pipeline (1.1 GB).

## Endpoint'ler

- `GET /health` → `{"status":"healthy"}`
- `POST /predict` → araç özellikleri → tahmini fiyat (TL)

## Model

`araba_modeli.pkl` (~1.1 GB) — Hugging Face repo LFS'i ile saklanır. Container build sırasında image'a kopyalanır.

## Backend bağlantısı

Backend `FASTAPI_BASE_URL` env'ini bu HF Space URL'ine (`https://<user>-<space>.hf.space`) ayarlar.

> Gerçek kod: <https://github.com/burak-sisci/A2M2_Ekibi_Oto_Pusula_Uygulamasi/tree/main/ML_Model_V4>
