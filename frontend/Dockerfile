# Stage 1: Build
FROM node:20-alpine AS build
WORKDIR /app

COPY package.json package-lock.json ./
RUN npm ci

COPY . .

# Build sırasında placeholder kullan
ENV REACT_APP_API_URL=__REACT_APP_API_URL_PLACEHOLDER__

RUN npm run build

# Stage 2: Serve with nginx
FROM nginx:alpine

RUN apk add --no-cache bash

COPY --from=build /app/build /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Entrypoint scriptini container içinde oluştur (CRLF sorununu önler)
RUN printf '#!/bin/bash\n\
if [ -n "$REACT_APP_API_URL" ]; then\n\
  echo "Injecting REACT_APP_API_URL: $REACT_APP_API_URL"\n\
  find /usr/share/nginx/html/static/js -name "*.js" -exec sed -i "s|__REACT_APP_API_URL_PLACEHOLDER__|${REACT_APP_API_URL}|g" {} +\n\
fi\n\
nginx -g "daemon off;"\n' > /entrypoint.sh && chmod +x /entrypoint.sh

EXPOSE 80

CMD ["bash", "/entrypoint.sh"]
