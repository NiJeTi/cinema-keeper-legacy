FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /src

COPY . .
RUN dotnet publish --configuration Release --output ./build

FROM mcr.microsoft.com/dotnet/core/runtime:3.1
WORKDIR /app
COPY --from=build-env /src/build .
ENTRYPOINT [ "dotnet", "CinemaKeeper.Service.dll" ]
