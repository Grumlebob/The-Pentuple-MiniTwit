#!/usr/bin/env bash

echo "Building Minitwit Images"
docker build -t $DOCKER_USERNAME/migratorimage -f Dockerfile.migrator .
docker build -t $DOCKER_USERNAME/apiimage -f MiniTwit.Api/Dockerfile .
docker build -t $DOCKER_USERNAME/clientimage -f MiniTwit.Client/Dockerfile .
docker build -t $DOCKER_USERNAME/simulatorimage -f Simulator/Dockerfile ./Simulator

echo "Login to Dockerhub, provide your password below..."
read -rs DOCKER_PASSWORD
echo $DOCKER_PASSWORD | docker login -u "$DOCKER_USERNAME" --password-stdin

echo "Pushing Minitwit Images to Dockerhub..."
docker push $DOCKER_USERNAME/migratorimage:latest
docker push $DOCKER_USERNAME/apiimage:latest
docker push $DOCKER_USERNAME/clientimage:latest
docker push $DOCKER_USERNAME/simulatorimage:latest
