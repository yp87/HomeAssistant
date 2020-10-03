#!/bin/bash
docker exec supervisor docker-compose -f /Source/docker-compose-homeassistant.yaml --env-file /Source/.env up -d --build
docker system prune -fa
docker volume prune -f
