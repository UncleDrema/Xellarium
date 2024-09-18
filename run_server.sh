#!/bin/bash
echo "Building server..."
dotnet publish ./src/Xellarium.Server/ -o artifacts/server

echo "Starting server..."
cd artifacts/server
./Xellarium.Server.exe