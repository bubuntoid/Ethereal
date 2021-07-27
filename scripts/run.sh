#!/bin/bash

fuser -k 322/tcp

cd $(dirname "$0")
cd ../src/BE/YOVPS.WebAPI

dotnet run --urls "http://0.0.0.0:322"