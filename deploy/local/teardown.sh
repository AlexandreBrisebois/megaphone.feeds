docker-compose -f docker-compose-app.yml down
docker-compose -f docker-compose-infra.yml down

docker volume prune -f