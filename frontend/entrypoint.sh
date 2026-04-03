#!/bin/bash

# Railway Variables'dan gelen REACT_APP_API_URL'yi build çıktısına inject et
# Bu sayede build argument'a gerek kalmaz

if [ -n "$REACT_APP_API_URL" ]; then
  echo "Injecting REACT_APP_API_URL: $REACT_APP_API_URL"
  find /usr/share/nginx/html/static/js -name '*.js' -exec sed -i "s|__REACT_APP_API_URL_PLACEHOLDER__|${REACT_APP_API_URL}|g" {} +
else
  echo "WARNING: REACT_APP_API_URL is not set!"
fi

# nginx başlat
nginx -g 'daemon off;'
