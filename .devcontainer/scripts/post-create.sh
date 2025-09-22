#!/bin/bash

sudo chown -R $USER:$USER /workspace/persistence
sudo chown -R $USER:$USER /workspace/backend/FiletOFiles.Api/db
sudo chown -R $USER:$USER /workspace/backend/FiletOFiles.Api/filestorage

dotnet tool install dotnet-ef
dotnet tool install csharpier
dotnet dev-certs https
dotnet restore ./backend

cd ./frontend 
npm install

