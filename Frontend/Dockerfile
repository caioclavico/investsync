# Use the official Node.js image for building
FROM node:18-alpine AS build

# Set the working directory
WORKDIR /app

# Copy package.json and package-lock.json (if available)
COPY package*.json ./

# Install dependencies
RUN npm install

# Copy the source code
COPY . .

# Create .env file with API URL
RUN echo 'REACT_APP_API_URL=http://localhost:5000' > .env

# Build the application
RUN npm run build

# Use nginx to serve the built application
FROM nginx:alpine AS runtime

# Copy the built application to nginx
COPY --from=build /app/build /usr/share/nginx/html

# Copy custom nginx configuration
COPY nginx.conf /etc/nginx/nginx.conf

# Expose port 80
EXPOSE 80

# Start nginx
CMD ["nginx", "-g", "daemon off;"]
