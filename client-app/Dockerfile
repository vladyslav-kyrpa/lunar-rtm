ARG NODE_VERSION=24.6.0
ARG NGINX_VERSION=alpine

# ==========================================
# 1. Prepare React App
# ==========================================
FROM node:${NODE_VERSION} AS builder

WORKDIR /app

COPY package.json package-lock.json ./

# Install dep. using npm ci (clean install)
RUN npm ci

# copy app source code
COPY . .

# build
RUN npm run build

# ==========================================
# 2. Prepare Nginx to serve static files
# ==========================================
FROM nginxinc/nginx-unprivileged:${NGINX_VERSION} AS runner

# use build=in non-root user for security
USER nginx

COPY nginx.conf /etc/nginx/nginx.conf

COPY --chown=nginx:nginx --from=builder /app/dist /usr/share/nginx/html

# default nginx container listents to 8080
EXPOSE 8080

# start with custom config
ENTRYPOINT ["nginx", "-c", "/etc/nginx/nginx.conf"]
CMD ["-g", "daemon off;"]

