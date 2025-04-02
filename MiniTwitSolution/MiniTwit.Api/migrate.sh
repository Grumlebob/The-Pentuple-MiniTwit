#!/bin/sh
set -e

echo "=== Debug: Listing files in /app ==="
ls -la ./

echo "=== Debug: Running migrations-bundle ==="
./ef-migrations-bundle

echo "=== Debug: Migrations complete ==="