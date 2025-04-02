# This script installs the things needed for a droplet to run docker compose.
echo "Starting system update and prerequisite installation..."
apt-get update
apt-get install -y apt-transport-https ca-certificates curl software-properties-common
echo "Prerequisites installed."

echo "Installing Docker..."
# Add Docker's GPG key only if it hasn't been added already.
if ! apt-key list | grep -q 'Docker'; then
  curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo apt-key add -
fi
# Add the Docker repository if not already present.
if ! grep -q "download.docker.com" /etc/apt/sources.list /etc/apt/sources.list.d/*; then
  add-apt-repository "deb [arch=amd64] https://download.docker.com/linux/ubuntu jammy stable"
fi
apt-get update
apt-get install -y docker-ce docker-ce-cli containerd.io
usermod -aG docker vagrant
echo "Docker installed."

echo "Installing Docker Compose..."
# Only install docker-compose if it's not already present.
if [ ! -f /usr/local/bin/docker-compose ]; then
  curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
  chmod +x /usr/local/bin/docker-compose
fi
echo "Docker Compose installed."