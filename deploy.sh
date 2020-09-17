#!/bin/bash
docker exec supervisor docker-compose -f /Source/docker-compose.yaml up -d --build homeassistant
docker system prune -fa
docker volume prune -f
