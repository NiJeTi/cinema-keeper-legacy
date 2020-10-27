ARTIFACTS_PATH=$1
MODULES_PATH=$2
MODULE_PATTERN=CinemaKeeper.Modules*.dll

cd $ARTIFACTS_PATH

find . -type f -name "$MODULE_PATTERN" -exec mv {} $MODULES_PATH \;

cd ..
rm -rf $ARTIFACTS_PATH