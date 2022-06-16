#!/bin/bash
docker exec supervisor docker compose -f /Source/docker-compose-homeassistant.yaml --env-file /Source/.env pull
./deploy.sh
