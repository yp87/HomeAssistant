#!/bin/bash

# moving to script directory regardless of the execution path
cd "${0%/*}"

docker-compose down
docker-compose up -d
docker system prune -fa
docker volume prune -f

