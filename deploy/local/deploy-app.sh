#!/bin/bash

echo "deploy app only"
docker-compose -f docker-compose-app.yml up -d --force-recreate