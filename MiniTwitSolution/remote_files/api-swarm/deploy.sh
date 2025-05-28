#!/bin/bash
set -ex  # prints every command and stops on error
# get env vars
source ~/.bash_profile

echo "====== DEPLOYMENT STARTED ======"
echo "Running as user: $(whoami)"
echo "Current directory: $(pwd)"

echo "Pulling latest Docker images..."
docker compose -f docker-compose.yml pull || {
  echo "Docker pull failed. Checking Docker Compose file:"
  cat docker-compose.yml
  echo "Docker status:"
  docker info
  exit 1
}

echo "Deploying stack to the swarm..."
docker stack deploy -c docker-compose.yml minitwit || {
  echo "Stack deployment failed."
  docker stack ps minitwit
  exit 1
}

echo "====== DEPLOYMENT COMPLETED SUCCESSFULLY ======"

docker service scale minitwit_api=2 # specify number of workers

echo "====== Created 2 replicas ======"