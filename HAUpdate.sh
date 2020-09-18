#!/bin/bash
docker exec supervisor docker-compose -f /Source/docker-compose-homeassistant.yaml pull
./deploy.sh
