#!/bin/bash
docker exec supervisor docker compose -f /Source/docker-compose-homeassistant.yaml --env-file /Source/.env up -d --build

echo "Do you wish to cleanup all unused images and volumes?"
select yn in "Yes" "No"; do
    case $yn in
        Yes ) docker system prune -fa; docker volume prune -f; break;;
        No ) exit;;
    esac
done
