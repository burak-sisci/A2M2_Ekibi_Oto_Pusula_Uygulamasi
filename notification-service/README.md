---
title: OtoPusula Notification
emoji: 📨
colorFrom: blue
colorTo: indigo
sdk: docker
app_port: 7860
pinned: false
---

# OtoPusula — Notification Service

RabbitMQ tüketicisi: yorum bildirimi (FCM) + şifre sıfırlama email (SMTP).

## Endpoint'ler

- `GET /health` → durum kontrolü (HF Spaces "running" mesajı için)

## Workers

- **CommentNotificationWorker** — `yorum.olusturuldu` queue → Firebase Cloud Messaging
- **EmailWorker** — `email.send` queue → SMTP (Gmail App Password)

## Env değişkenleri (HF Spaces Settings → Repository secrets)

| Key | Açıklama |
|-----|---------|
| `RABBITMQ_CONNECTION_STRING` | CloudAMQP amqps URL |
| `CONNECTION_STRING` | MongoDB Atlas connection string |
| `DATABASE_NAME` | `projctdb` |
| `FIREBASE_CREDENTIALS_B64` | firebase-credentials.json base64 |
| `SMTP_HOST`, `SMTP_PORT`, `SMTP_USER`, `SMTP_PASS`, `SMTP_FROM` | Gmail SMTP |

> Gerçek kod: <https://github.com/burak-sisci/A2M2_Ekibi_Oto_Pusula_Uygulamasi/tree/main/notification-service>
