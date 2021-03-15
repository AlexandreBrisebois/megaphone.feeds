docker-compose -p megaphone-app -f docker-compose-app.yml down
docker-compose -p megaphone-infra -f docker-compose-infra.yml down

docker image prune -a -f
docker volume prune -f