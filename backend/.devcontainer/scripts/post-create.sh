#!/bin/bash

sudo dotnet workload update
dotnet tool install dotnet-ef
dotnet tool install csharpier
dotnet dev-certs https
dotnet restore
