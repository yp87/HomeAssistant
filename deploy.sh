#!/bin/bash

# moving to script directory regardless of the execution path
cd "${0%/*}"

docker-compose up -d --build
docker system prune -fa
docker volume prune -f