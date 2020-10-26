BUILD_PATH=$1
MODULE_PATTERN=CinemaKeeper.Modules*.dll

cd $BUILD_PATH
find . -type f ! -name $MODULE_PATTERN -delete