docker-compose -f docker-compose-app.yml down
docker-compose -f docker-compose-infra.yml down

docker image prune -a -f
docker volume prune -f