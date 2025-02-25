#!/bin/bash

# Error handling: stops on errors
set -e

echo "Loading environment variables..."
source ~/.bash_profile

echo "Changing directory to MiniTwitSolution..."
cd /minitwit/MiniTwitSolution

echo "Pulling latest Docker images..."
docker compose pull

echo "Starting containers in detached mode..."
docker compose up -d

echo "Deployment completed successfully."
