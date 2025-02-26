#!/bin/bash
set -ex  # prints every command + stops on error

source ~/.bash_profile

echo "DEBUG: Home directory is $HOME"
echo "DEBUG: Current directory is $(pwd)"
echo "DEBUG: Listing files in current directory..."
ls -la
echo

echo "DEBUG: Searching for docker-compose.yml everywhere..."
find / -name "docker-compose.yml" -maxdepth 5 -ls 2>/dev/null || true
# ^ This may produce a lot of output, but it’s thorough.

# If you *expect* docker-compose.yml in /minitwit:
if [ -d /minitwit ]; then
  echo "DEBUG: Listing files in /minitwit..."
  ls -la /minitwit
else
  echo "ERROR: /minitwit directory does not exist."
fi

# Now, cd into /minitwit if that’s where you keep docker-compose.yml
cd /minitwit || {
  echo "ERROR: /minitwit not found on the server"
  exit 1
}

echo "DEBUG: Now in $(pwd). Listing files again..."
ls -la

# Try a quick sanity check: is docker-compose.yml here?
if [ -f docker-compose.yml ]; then
  echo "DEBUG: Found docker-compose.yml in /minitwit"
else
  echo "ERROR: docker-compose.yml not found in /minitwit"
  exit 1
fi

# Pull and start containers
echo "Pulling latest Docker images..."
docker compose -f docker-compose.yml pull

echo "Starting Docker containers..."
docker compose -f docker-compose.yml up -d

echo "Deployment completed successfully."
