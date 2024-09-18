#!/bin/bash
echo "Building technical UI..."
dotnet publish ./src/Xellarium.TechUi/ -o artifacts/techui

echo "Starting technical UI..."
cd artifacts/techui
./Xellarium.TechUi.exe