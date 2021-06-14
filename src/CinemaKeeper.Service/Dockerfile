FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
COPY . .
RUN dotnet publish --configuration Release --output ./build ./src/CinemaKeeper.Service

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /build .
ENTRYPOINT [ "dotnet", "CinemaKeeper.Service.dll" ]
