#!/bin/bash
docker exec supervisor docker-compose -f /Source/docker-compose-homeassistant.yaml down
docker compose pull
docker compose up --build -d
./HAUpdate.sh
