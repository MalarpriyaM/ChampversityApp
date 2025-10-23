@echo off
echo Setting up Champversity Legacy Application...

echo Creating directories...
mkdir "%~dp0Champversity.Web\wwwroot\Uploads"
mkdir "%~dp0Champversity.Web\wwwroot\Templates"

echo Copying project files...
copy /Y "%~dp0Champversity.Web\Champversity.Web.csproj.new" "%~dp0Champversity.Web\Champversity.Web.csproj"
copy /Y "%~dp0Champversity.DataAccess\Champversity.DataAccess.csproj.new" "%~dp0Champversity.DataAccess\Champversity.DataAccess.csproj"
copy /Y "%~dp0Champversity.Web\appsettings.json.new" "%~dp0Champversity.Web\appsettings.json"

echo Installing required packages...
dotnet restore

echo Creating database...
dotnet ef database update --project Champversity.DataAccess --startup-project Champversity.Web

echo Setup complete!
echo You can now build and run the application.
pause