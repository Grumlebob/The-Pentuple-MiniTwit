#!/bin/bash
set -ex  # prints every command + stops on error

echo "====== DEPLOYMENT STARTED ======"
echo "Running as user: $(whoami)"
echo "Current directory: $(pwd)"
echo "Server environment:"
uname -a

source ~/.bash_profile

echo "Pulling latest Docker images..."
docker compose -f docker-compose.yml pull || {
  echo "Docker pull failed. Checking Docker Compose file:"
  cat docker-compose.yml
  echo "Docker status:"
  docker info
  exit 1
}

echo "Starting Docker containers..."
docker compose -f docker-compose.yml up -d || {
  echo "Docker up failed."
  docker compose -f docker-compose.yml logs
  exit 1
}

echo "====== DEPLOYMENT COMPLETED SUCCESSFULLY ======"