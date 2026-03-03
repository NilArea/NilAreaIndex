FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["backend/Nilarea.WebApi/Nilarea.WebApi.csproj", "backend/Nilarea.WebApi/"]
COPY ["backend/shared-libs/Nilarea.Contracts/Nilarea.Contracts.csproj", "backend/shared-libs/Nilarea.Contracts/"]
COPY ["backend/shared-libs/Nilarea.Common/Nilarea.Common.csproj", "backend/shared-libs/Nilarea.Common/"]
COPY ["backend/shared-libs/Nilarea.Interfaces/Nilarea.Interfaces.csproj", "backend/shared-libs/Nilarea.Interfaces/"]
RUN dotnet restore "backend/Nilarea.WebApi/Nilarea.WebApi.csproj"
COPY . .
WORKDIR "/src/backend/Nilarea.WebApi"
RUN dotnet build "./Nilarea.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Nilarea.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nilarea.WebApi.dll"]
