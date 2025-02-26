#!/bin/bash
set -ex  # -e to exit on error, -x to print each command

# Load environment variables (DOCKER_USERNAME, etc.)
source ~/.bash_profile

# Debug: Print current working directory and file listing
echo "DEBUG: Current working directory: $(pwd)"
echo "DEBUG: Listing files in current directory:"
ls -la
echo ""
echo "DEBUG: Recursively listing files (max depth 3):"
find . -maxdepth 3 -ls

# Ensure we're in the correct directory
cd /minitwit || { echo "ERROR: /minitwit not found"; exit 1; }
echo "DEBUG: Now in directory: $(pwd)"
ls -la

# Now use the local docker-compose.yml
echo "Pulling latest Docker images..."
docker compose -f docker-compose.yml pull

echo "Starting Docker containers..."
docker compose -f docker-compose.yml up -d

echo "Deployment completed successfully."
