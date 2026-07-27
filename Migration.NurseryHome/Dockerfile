FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5004

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Migration.NurseryHome/Migration.NurseryHome.csproj", "Migration.NurseryHome/"]
COPY ["Migration.Contracts/Migration.Contracts.csproj", "Migration.Contracts/"]
RUN dotnet restore "Migration.NurseryHome/Migration.NurseryHome.csproj"
COPY . .
WORKDIR "/src/Migration.NurseryHome"
RUN dotnet build "Migration.NurseryHome.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Migration.NurseryHome.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Migration.NurseryHome.dll"]
