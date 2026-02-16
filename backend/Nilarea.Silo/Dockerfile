FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["backend/Nilarea.Silo/Nilarea.Silo.csproj", "backend/Nilarea.Silo/"]
COPY ["backend/Nilarea.Grains/Nilarea.Grains.csproj", "backend/Nilarea.Grains/"]
COPY ["backend/shared-libs/Nilarea.Interfaces/Nilarea.Interfaces.csproj", "backend/shared-libs/Nilarea.Interfaces/"]
COPY ["backend/shared-libs/Nilarea.Contracts/Nilarea.Contracts.csproj", "backend/shared-libs/Nilarea.Contracts/"]
COPY ["backend/shared-libs/Nilarea.Common/Nilarea.Common.csproj", "backend/shared-libs/Nilarea.Common/"]
RUN dotnet restore "backend/Nilarea.Silo/Nilarea.Silo.csproj"
COPY . .
WORKDIR "/src/backend/Nilarea.Silo"
RUN dotnet build "./Nilarea.Silo.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Nilarea.Silo.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nilarea.Silo.dll"]
ENV a=a
