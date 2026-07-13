#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MigrationWeb/MigrationWeb.csproj", "MigrationWeb/"]
COPY ["Migration.Agro/Migration.Agro.csproj", "Migration.Agro/"]
COPY ["Migration.Contracts/Migration.Contracts.csproj", "Migration.Contracts/"]
COPY ["Migration.Shipbuilding/Migration.Shipbuilding.csproj", "Migration.Shipbuilding/"]
RUN dotnet restore "MigrationWeb/MigrationWeb.csproj"
COPY . .
WORKDIR "/src/MigrationWeb"
RUN dotnet build "MigrationWeb.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MigrationWeb.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MigrationWeb.dll"]
