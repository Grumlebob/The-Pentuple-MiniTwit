#!/bin/bash

# Error handling: stop on errors
set -e

cd /minitwit

docker compose -f docker-compose.yml pull
docker compose -f docker-compose.yml up -d
