#!/bin/bash

echo "deploy app only"
docker-compose -p megaphone-app -f docker-compose-app.yml pull
docker-compose -p megaphone-app -f docker-compose-app.yml up -d --force-recreate

docker image prune -a -f
docker volume prune -f