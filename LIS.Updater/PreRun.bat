@echo off
set target=%1
set target=%target:"=%

set source=%2
set source=%source:"=%

set config=%3


if /i %config%==y (
echo getting backup for config file
copy "%target%\web.config" "temp\web.config.bak"
)

echo cleaning application folder
del /S /Q /F %target% 

echo deploying new application version
xcopy /e /y /I "%source%" "%target%"

if /i %config%==y (
echo restoring config file
copy "temp\web.config.bak" "%target%\web.config"
)

echo cleaning temp folder
del /S /Q /F temp 
rmdir /S /Q temp