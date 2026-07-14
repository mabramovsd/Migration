FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Migration.Shipbuilding/Migration.Shipbuilding.csproj", "Migration.Shipbuilding/"]
COPY ["Migration.Contracts/Migration.Contracts.csproj", "Migration.Contracts/"]
RUN dotnet restore "Migration.Shipbuilding/Migration.Shipbuilding.csproj"
COPY . .
WORKDIR "/src/Migration.Shipbuilding"
RUN dotnet build "Migration.Shipbuilding.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Migration.Shipbuilding.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Migration.Shipbuilding.dll"]
