#!/bin/bash
echo "Building client..."
dotnet publish ./src/Xellarium.Client/ -o artifacts/client

echo "Starting client..."
cd artifacts/client
dotnet serve --directory ./wwwroot/ -S