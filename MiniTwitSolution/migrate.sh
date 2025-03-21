#!/bin/sh
set -e

echo "=== Debug: Checking if 'ef-migrations-bundle' exists ==="
if [ -f ef-migrations-bundle ]; then
    echo "=== Found 'ef-migrations-bundle'! ==="
else
    echo "=== ERROR: 'ef-migrations-bundle' not found. Exiting. ==="
    exit 1
fi

echo "=== Debug: Listing files in /app ==="
ls -la ./

echo "=== Debug: Running migrations-bundle ==="
./ef-migrations-bundle

echo "=== Debug: Migrations complete ==="

# Create the marker file so the healthcheck passes
touch /tmp/migrations_done

# Keep container alive (so it stays "healthy")
tail -f /dev/null