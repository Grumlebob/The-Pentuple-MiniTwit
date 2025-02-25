#!/bin/bash

# Error handling: exit immediately if a command fails
set -e 

# Load environment variables (DOCKER_USERNAME, etc.)
source ~/.bash_profile

# Pull the latest images
echo "Pulling latest Docker images..."
docker compose -f docker-compose.yml pull

# Bring up the containers in detached mode
echo "Starting Docker containers..."
docker compose -f docker-compose.yml up -d

echo "Deployment completed successfully."
