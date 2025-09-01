#!/bin/bash

sudo dotnet workload update

dotnet tool install -g dotnet-ef

dotnet dev-certs https

dotnet restore
