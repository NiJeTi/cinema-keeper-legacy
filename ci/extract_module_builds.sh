ARTIFACTS_PATH=$1
MODULES_PATH=$2
MODULE_PATTERN=CinemaKeeper.Modules*.dll

find $ARTIFACTS_PATH -type f -name "$MODULE_PATTERN" -exec mv -t $MODULES_PATH {} + \;
rm -rf $ARTIFACTS_PATH