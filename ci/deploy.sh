CONFIG_FILE=appsettings.Production.json

cd $1

if [ ! -f $CONFIG_FILE ]; then
    echo "{}" > $CONFIG_FILE
fi

docker-compose up -d
