CONFIG_FILE=appsettings.Production.json

DOCKER_LOGIN=$1
DOCKER_TOKEN=$2

if [ ! -f $CONFIG_FILE ]; then
    echo "{}" > $CONFIG_FILE
fi

docker login docker.pkg.github.com \
--username $DOCKER_LOGIN --password $DOCKER_TOKEN

docker-compose up -d
