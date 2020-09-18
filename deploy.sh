#!/bin/bash
docker exec supervisor docker-compose -f /Source/docker-compose-homeassistant.yaml up -d --build
docker system prune -fa
docker volume prune -f
