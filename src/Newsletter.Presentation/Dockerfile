# Stage base: runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80


FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src


COPY ["src/Newsletter.Presentation/Newsletter.Presentation.csproj", "src/Newsletter.Presentation/"]
COPY ["src/Newsletter.Domain/Newsletter.Domain.csproj", "src/Newsletter.Domain/"]
COPY ["src/Newsletter.Application/Newsletter.Application.csproj", "src/Newsletter.Application/"]
COPY ["src/Newsletter.Infrastructure/Newsletter.Infrastructure.csproj", "src/Newsletter.Infrastructure/"]

RUN dotnet restore "src/Newsletter.Presentation/Newsletter.Presentation.csproj"


COPY . .


WORKDIR "/src/src/Newsletter.Presentation"


RUN dotnet build "Newsletter.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build


FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Newsletter.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Newsletter.Presentation.dll"]


