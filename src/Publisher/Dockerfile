FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/Publisher/Publisher.csproj src/Publisher/
RUN dotnet restore src/Publisher/Publisher.csproj

COPY src/Publisher/ src/Publisher/
RUN dotnet publish src/Publisher/Publisher.csproj --configuration Release --output /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Publisher.dll"]
