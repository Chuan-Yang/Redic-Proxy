# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

COPY *.sln .
# Copy csproj and restore as distinct layers
COPY src/RedisProxy.Service/*.csproj src/RedisProxy.Service/
COPY src/RedisProxy.Service.IntegrationTest/*.csproj src/RedisProxy.Service.IntegrationTest/
RUN dotnet restore

# copy the rest and build
COPY . .
RUN dotnet build

WORKDIR /app/src/RedisProxy.Service
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/src/RedisProxy.Service/out ./
ENTRYPOINT ["dotnet", "RedisProxy.Service.dll"]
