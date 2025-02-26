#!/bin/bash
set -ex  # prints every command + stops on error

echo "====== DEPLOYMENT STARTED ======"
echo "Running as user: $(whoami)"
echo "Current directory: $(pwd)"
echo "Server environment:"
uname -a
docker --version
docker compose version

source ~/.bash_profile || echo "No .bash_profile found"

echo "====== CHECKING FILES ======"
echo "Checking /minitwit directory..."
if [ ! -d /minitwit ]; then
  echo "ERROR: /minitwit directory does not exist. Creating it..."
  mkdir -p /minitwit
fi
cd /minitwit
echo "Files in /minitwit:"
ls -la

echo "Checking for docker-compose.yml..."
if [ ! -f docker-compose.yml ]; then
  echo "ERROR: docker-compose.yml not found in /minitwit!"
  echo "Searching for docker-compose.yml elsewhere..."
  find / -name "docker-compose.yml" -maxdepth 3 2>/dev/null || echo "No docker-compose.yml found on system"
  exit 1
fi

echo "====== DOCKER OPERATIONS ======"
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

echo "Docker containers running:"
docker ps

echo "====== DEPLOYMENT COMPLETED SUCCESSFULLY ======"