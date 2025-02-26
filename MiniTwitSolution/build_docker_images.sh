#!/usr/bin/env bash

echo "Building Minitwit Images"
docker build -t $DOCKER_USERNAME/minitwit-migrator -f Dockerfile.migrator .
docker build -t $DOCKER_USERNAME/minitwit-api -f MiniTwit.Api/Dockerfile .
docker build -t $DOCKER_USERNAME/minitwit-client -f MiniTwit.Client/Dockerfile .
docker build -t $DOCKER_USERNAME/minitwit-simulator -f Simulator/Dockerfile ./Simulator

echo "Login to Dockerhub, provide your password below..."
read -rs DOCKER_PASSWORD
echo $DOCKER_PASSWORD | docker login -u "$DOCKER_USERNAME" --password-stdin

echo "Pushing Minitwit Images to Dockerhub..."
docker push $DOCKER_USERNAME/minitwit-migrator:latest
docker push $DOCKER_USERNAME/minitwit-api:latest
docker push $DOCKER_USERNAME/minitwit-client:latest
docker push $DOCKER_USERNAME/minitwit-simulator:latest
