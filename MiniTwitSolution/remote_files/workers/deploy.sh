#!/bin/bash
set -ex  # prints every command and stops on error

echo "====== DEPLOYMENT STARTED ======"
echo "Running as user: $(whoami)"
echo "Current directory: $(pwd)"

echo "Deploying stack to the swarm..."
docker stack deploy -c docker-stack.yml minitwit || {
  echo "Stack deployment failed."
  docker stack ps minitwit
  exit 1
}

echo "====== DEPLOYMENT COMPLETED SUCCESSFULLY ======"
