#!/bin/bash
set -e

echo "Deployment script starting..."

# List files in the current directory and in /minitwit for debugging
echo "Current working directory: $(pwd)"
echo "Listing files in current directory:"
ls -la

echo "Listing files in /minitwit directory:"
ls -la /minitwit

# Change into the project directory where docker-compose.yml is expected to be
TARGET_DIR="/minitwit/MiniTwitSolution"
echo "Attempting to change directory to ${TARGET_DIR}..."
cd ${TARGET_DIR} || { echo "Failed to change directory to ${TARGET_DIR}"; exit 1; }

echo "Current working directory after change: $(pwd)"
echo "Listing files in the project directory:"
ls -la

echo "Pulling latest Docker images..."
docker compose pull

echo "Starting containers in detached mode..."
docker compose up -d

echo "Deployment completed successfully."
