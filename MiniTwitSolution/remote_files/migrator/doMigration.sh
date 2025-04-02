#!/bin/bash
set -ex  # prints every command and stops on error
# get env vars
source ~/.bash_profile

echo "Pulling latest Docker images..."
docker compose -f docker-compose.yml pull || {
  echo "Docker pull failed. Checking Docker Compose file:"
  cat docker-compose.yml
  echo "Docker status:"
  docker info
  exit 1
}

echo "Running Docker migrator container..."
docker compose run --rm migrator || {
  echo "Docker up failed."
  docker compose -f docker-compose.yml logs
  exit 1
}

echo "====== DEPLOYMENT COMPLETED SUCCESSFULLY ======"
