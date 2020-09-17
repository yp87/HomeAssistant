#!/bin/bash
docker exec supervisor docker-compose -f /Source/docker-compose.yaml pull homeassistant
./deploy.sh
