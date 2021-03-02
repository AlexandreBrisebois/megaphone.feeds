#!/bin/bash

echo "deploy infra"
docker-compose -f docker-compose-infra.yml up -d --force-recreate

echo "deploy app"
docker-compose -f docker-compose-app.yml up -d --force-recreate