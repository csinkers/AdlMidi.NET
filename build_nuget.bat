@echo off
dotnet clean -c Debug
dotnet clean -c Release
rm -rf .\bin
dotnet build -c Release
dotnet pack -c Release
