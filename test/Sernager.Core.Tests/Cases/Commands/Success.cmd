@echo off

set env1=%ENV1%
set env2=%ENV2%
set env3=%ENV3%

echo %env1%
echo %env2%
echo %env3%

IF NOT "%1" == "" (
    echo Arguments: %*
)

exit 0