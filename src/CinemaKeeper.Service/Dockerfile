FROM mcr.microsoft.com/dotnet/core/sdk:5.0 AS build-env
WORKDIR /src
COPY . .
RUN dotnet publish --configuration Release --output ./build

FROM mcr.microsoft.com/dotnet/core/aspnet:5.0
WORKDIR /app
COPY --from=build-env /src/build .
ENTRYPOINT [ "dotnet", "CinemaKeeper.Service.dll" ]
