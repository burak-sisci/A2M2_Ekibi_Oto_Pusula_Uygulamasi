FROM python:3.11-slim

WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*

COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

COPY . .

# Railway Git LFS desteklemiyor, model dosyasini GitHub'dan indir
ARG MODEL_DOWNLOAD_URL
RUN if [ -n "$MODEL_DOWNLOAD_URL" ]; then \
      echo "Model indiriliyor..." && \
      curl -L -o araba_modeli.pkl "$MODEL_DOWNLOAD_URL" && \
      echo "Model indirildi: $(wc -c < araba_modeli.pkl) bytes"; \
    fi

EXPOSE 8000

CMD ["uvicorn", "api:app", "--host", "0.0.0.0", "--port", "8000"]
