#!/bin/sh
dotnet clean -c Debug
dotnet clean -c Release
rm -r src/bin
dotnet build -c Release
dotnet pack -c Release src/ADLMidi.NET.csproj
