#!/bin/bash
set -euo pipefail

ASP_NET_ROOT="$(cd "$(dirname "$0")/.." && pwd)"
REPO_ROOT="$(cd "$ASP_NET_ROOT/.." && pwd)"
RELEASE_ROOT="$ASP_NET_ROOT/release"
MIGRATOR_RELEASE="$RELEASE_ROOT/migrator"
WEB_HOST_RELEASE="$RELEASE_ROOT/webhost"
ANGULAR_RELEASE="$RELEASE_ROOT/angular"

dotnet publish "$ASP_NET_ROOT/src/PawnCloud.Migrator/PawnCloud.Migrator.csproj" -c Release -o "$MIGRATOR_RELEASE"
dotnet publish "$ASP_NET_ROOT/src/PawnCloud.Web.Host/PawnCloud.Web.Host.csproj" -c Release -o "$WEB_HOST_RELEASE"

cd "$REPO_ROOT/angular"
yarn install
yarn run ng build --configuration production

mkdir -p "$ANGULAR_RELEASE"
cp -R "$REPO_ROOT/angular/dist/browser/." "$ANGULAR_RELEASE/"
cp "$REPO_ROOT/angular/Dockerfile" "$ANGULAR_RELEASE/"
cp "$REPO_ROOT/angular/fast-nginx-default.conf" "$ANGULAR_RELEASE/"

echo "Deployment folders are ready:"
echo "$MIGRATOR_RELEASE"
echo "$WEB_HOST_RELEASE"
echo "$ANGULAR_RELEASE"
