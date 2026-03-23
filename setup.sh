#!/bin/sh

set -eu

SCRIPT_DIR=$(CDPATH= cd -- "$(dirname "$0")" && pwd)

mkdir -p "$SCRIPT_DIR/Champversity.Web/wwwroot/Uploads"
mkdir -p "$SCRIPT_DIR/Champversity.Web/wwwroot/Templates"
mkdir -p "$SCRIPT_DIR/Champversity.Web/App_Data"

dotnet restore "$SCRIPT_DIR/ChampversityApp.sln"
dotnet build "$SCRIPT_DIR/ChampversityApp.sln"

echo "Setup complete. Start the app with: dotnet run --project Champversity.Web"