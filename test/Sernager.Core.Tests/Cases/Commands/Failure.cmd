@echo off

set notDeclaredEnvironmentVariable=%NOT_DECLARED_ENVIRONMENT_VARIABLE%

IF NOT "%1" == "" (
    echo Arguments: %*
)

echo Env variable doesn't exist: %notDeclaredEnvironmentVariable%

exit 1