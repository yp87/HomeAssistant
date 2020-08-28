#!/bin/bash
docker-compose pull
docker-compose down
docker-compose up -d
docker system prune -fa
docker volume prune -f
