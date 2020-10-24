CONFIG_FILE=appsettings.Production.json

if [ ! -f $CONFIG_FILE ]; then
    echo "{}" > $CONFIG_FILE
fi

docker-compose up -d
