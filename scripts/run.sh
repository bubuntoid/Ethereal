#!/bin/bash

fuser -k 80/tcp

cd $(dirname "$0")
cd ../src/Ethereal.WebAPI

dotnet run --urls "http://0.0.0.0:80"
