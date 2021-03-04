#!/bin/bash

echo "deploy infra"
docker-compose -p megaphone-infra -f docker-compose-infra.yml pull
docker-compose -p megaphone-infra -f docker-compose-infra.yml up -d --force-recreate

echo "deploy app"
docker-compose -p megaphone-app -f docker-compose-app.yml pull
docker-compose -p megaphone-app -f docker-compose-app.yml up -d --force-recreate

docker image prune -a -f
docker volume prune -f