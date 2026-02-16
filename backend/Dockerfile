FROM node:20-alpine AS base

WORKDIR /app

# Bağımlılıkları yükle
COPY package*.json ./
RUN npm ci --only=production

# Uygulama kodunu kopyala
COPY src/ ./src/

# Ortam değişkenleri
ENV NODE_ENV=production
ENV PORT=5000

EXPOSE 5000

# Sağlık kontrolü
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:5000/api/health || exit 1

CMD ["node", "src/server.js"]
