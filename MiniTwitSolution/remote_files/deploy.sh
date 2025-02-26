#!/bin/bash
set -e

# Load environment variables (DOCKER_USERNAME, etc.)
source ~/.bash_profile

# Debug: Print the current directory and list files
echo "DEBUG: Current working directory: $(pwd)"
echo "DEBUG: Listing files in current directory:"
ls -la
echo ""
echo "DEBUG: Recursively listing files (max depth 2):"
find . -maxdepth 2 -ls

# Optionally, check specifically for docker-compose.yml
if [ -f docker-compose.yml ]; then
    echo "DEBUG: docker-compose.yml found in $(pwd)"
else
    echo "DEBUG: docker-compose.yml NOT found in $(pwd)"
fi
