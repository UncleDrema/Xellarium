@echo off
echo Building client...
dotnet publish .\src\Xellarium.Client\ -o artifacts\client

echo Starting client...
cd artifacts\client
start cmd /k dotnet serve --directory .\wwwroot\ -S