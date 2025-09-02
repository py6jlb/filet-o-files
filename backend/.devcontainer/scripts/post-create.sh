#!/bin/bash

dotnet tool install dotnet-ef
dotnet tool install csharpier
dotnet dev-certs https
dotnet restore
