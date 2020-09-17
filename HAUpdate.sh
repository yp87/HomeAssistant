#!/bin/bash
docker exec supervisor docker-compose -f /Source/docker-compose.yaml pull
docker exec supervisor docker-compose -f /Source/docker-compose.yaml down
./deploy.sh
