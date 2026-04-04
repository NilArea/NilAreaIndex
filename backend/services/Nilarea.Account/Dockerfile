FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["backend/services/Nilarea.Account/Nilarea.Account.csproj", "backend/services/Nilarea.Account/"]
COPY ["backend/shared/Nilarea.Contracts/Nilarea.Contracts.csproj", "backend/shared/Nilarea.Contracts/"]
COPY ["backend/shared/Nilarea.Common/Nilarea.Common.csproj", "backend/shared/Nilarea.Common/"]
RUN dotnet restore "backend/services/Nilarea.Account/Nilarea.Account.csproj"
COPY . .
WORKDIR "/src/backend/services/Nilarea.Account"
RUN dotnet build "./Nilarea.Account.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Nilarea.Account.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nilarea.Account.dll"]
