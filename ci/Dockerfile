FROM mcr.microsoft.com/dotnet/core/runtime:3.1
WORKDIR /App
COPY ./build/ .
ENTRYPOINT [ "dotnet", "NetCore.Docker.dll" ]