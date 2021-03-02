#!/bin/bash

echo "deploy infra"
docker-compose -p dev-infra -f docker-compose-infra.yml up -d

echo "deploy app"
docker-compose -p megaphoneresources_devcontainer -f docker-compose-app.yml up -d