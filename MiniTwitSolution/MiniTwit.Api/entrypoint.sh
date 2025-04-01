#!/bin/bash
set -e

echo "Waiting for database migrations to complete..."
# Poll every 2 seconds until the /tmp/migrations_done file is found.
# this is necessary since docker swarm cannot do depends_on 
while [ ! -f /tmp/migrations_done ]; do
    sleep 2
done

echo "Migrations completed. Starting the API..."
exec dotnet MiniTwit.Api.dll
