FROM python:3.11-slim

WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*

COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

# Model dosyası HF Spaces LFS ile bu image'a kopyalanır (1.1 GB).
COPY . .

# HF Spaces default port 7860
ENV PORT=7860
EXPOSE 7860

CMD ["sh", "-c", "uvicorn api:app --host 0.0.0.0 --port ${PORT}"]
