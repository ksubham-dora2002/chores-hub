# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore
COPY server/ChoresHub.Domain/ChoresHub.Domain.csproj ./ChoresHub.Domain.csproj
COPY server/ChoresHub.Application/ChoresHub.Application.csproj ./ChoresHub.Application.csproj
COPY server/ChoresHub.Infrastructure/ChoresHub.Infrastructure.csproj ./ChoresHub.Infrastructure.csproj
COPY server/ChoresHub.WebAPI/ChoresHub.WebAPI.csproj ./ChoresHub.WebAPI.csproj
RUN dotnet restore ./ChoresHub.WebAPI.csproj

# Copy all source files
COPY . ./

# Publish the app
RUN dotnet publish ./server/ChoresHub.WebAPI/ChoresHub.WebAPI.csproj -c Release -o out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "ChoresHub.WebAPI.dll"]
