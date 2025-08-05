FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiando csproj + appsettings.json
COPY ["src/Newsletter.Worker/Newsletter.Worker.csproj", "src/Newsletter.Worker/"]
COPY ["src/Newsletter.Worker/appsettings.json", "src/Newsletter.Worker/"]
COPY ["src/Newsletter.Application/Newsletter.Application.csproj", "src/Newsletter.Application/"]
COPY ["src/Newsletter.Domain/Newsletter.Domain.csproj", "src/Newsletter.Domain/"]
COPY ["src/Newsletter.Infrastructure/Newsletter.Infrastructure.csproj", "src/Newsletter.Infrastructure/"]

RUN dotnet restore "src/Newsletter.Worker/Newsletter.Worker.csproj"
COPY . .
WORKDIR "/src/src/Newsletter.Worker"
RUN dotnet build "./Newsletter.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Newsletter.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Newsletter.Worker.dll"]
