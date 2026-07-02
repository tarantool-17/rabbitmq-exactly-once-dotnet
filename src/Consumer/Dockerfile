FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/Consumer/Consumer.csproj src/Consumer/
RUN dotnet restore src/Consumer/Consumer.csproj

COPY src/Consumer/ src/Consumer/
RUN dotnet publish src/Consumer/Consumer.csproj --configuration Release --output /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Consumer.dll"]
