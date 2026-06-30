FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY albums-api/albums-api.csproj albums-api/
RUN dotnet restore albums-api/albums-api.csproj

COPY albums-api/ albums-api/
RUN dotnet publish albums-api/albums-api.csproj --configuration Release --output /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "albums-api.dll"]