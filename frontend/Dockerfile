# Build: Node.js 24 Alpine image
FROM node:24-alpine
WORKDIR /app

# Copy configuration files and install dependencies
COPY package*.json ./
RUN npm install


COPY . .


EXPOSE 4200

# Start the Angular server with external access for Docker
CMD ["npx", "ng", "serve", "--host", "0.0.0.0"]