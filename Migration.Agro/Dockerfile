FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5002

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Migration.Agro/Migration.Agro.csproj", "Migration.Agro/"]
COPY ["Migration.Contracts/Migration.Contracts.csproj", "Migration.Contracts/"]
RUN dotnet restore "Migration.Agro/Migration.Agro.csproj"
COPY . .
WORKDIR "/src/Migration.Agro"
RUN dotnet build "Migration.Agro.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Migration.Agro.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Migration.Agro.dll"]
