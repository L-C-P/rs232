#!/bin/bash
dotnet publish -c RELEASE --runtime osx-x64 --no-self-contained
dotnet publish -c RELEASE --runtime win-x64 --no-self-contained