@echo off

set fledir=%cd%
echo %fledir% 
echo Starting MicroService.Api1...
cd %fledir%"\MicroService.Api1\"
start dotnet watch run

echo Starting MicroService.Api2...
cd %fledir%"\MicroService.Api2\"
start dotnet watch run

echo Starting MicroService.APIGateway...
cd %fledir%"\MicroService.APIGateway\"
start dotnet watch run


 