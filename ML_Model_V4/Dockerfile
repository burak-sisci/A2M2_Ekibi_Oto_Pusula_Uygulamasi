FROM python:3.11-slim

WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*

COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

COPY . .

# Railway Git LFS desteklemiyor, model dosyasini GitHub'dan indir
RUN curl -L -o araba_modeli.pkl \
    "https://media.githubusercontent.com/media/burak-sisci/A2M2_Ekibi_Oto_Pusula_Uygulamasi/main/ML_Model_V4/araba_modeli.pkl" \
    && echo "Model indirildi: $(wc -c < araba_modeli.pkl) bytes"

EXPOSE 8000

CMD ["uvicorn", "api:app", "--host", "0.0.0.0", "--port", "8000"]
