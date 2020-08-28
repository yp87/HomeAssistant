#!/bin/bash
cd "${0%/*}"
docker-compose down
docker-compose up -d
