CONFIG_FILE=appsettings.Production.json

DOCKER_LOGIN=$1
DOCKER_TOKEN=$2

if [ ! -f $CONFIG_FILE ]; then
    echo "{}" > $CONFIG_FILE
fi

docker login \
--username $DOCKER_LOGIN --password $DOCKER_TOKEN \
docker.pkg.github.com

docker-compose pull && docker-compose up -d --force-recreate
