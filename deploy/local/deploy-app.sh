#!/bin/bash

echo "deploy app only"
docker-compose -p megaphoneresources_devcontainer -f docker-compose-app.yml up -d --force-recreate