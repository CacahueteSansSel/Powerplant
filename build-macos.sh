#!/bin/sh

APP_NAME="Powerplant"
BUILD_DIR="Powerplant/bin/Debug/net10.0/osx-arm64/publish"
APP_DIR="output/$APP_NAME.app"

cd $APP_NAME
dotnet publish -c Debug -f net10.0 -r osx-arm64 --self-contained true
cd ..

mkdir -p "$APP_DIR/Contents/MacOS"
mkdir -p "$APP_DIR/Contents/Resources"
cp -R "$BUILD_DIR/"* "$APP_DIR/Contents/MacOS/"

iconutil -c icns Resources/Icon.iconset
cp Resources/Icon.icns "$APP_NAME/Icon.icns"

cp "$APP_NAME/Info.plist" "$APP_DIR/Contents/Info.plist"
cp "$APP_NAME/Icon.icns" "$APP_DIR/Contents/Resources/Icon.icns"

chmod +x "$APP_DIR/Contents/MacOS/Powerplant"