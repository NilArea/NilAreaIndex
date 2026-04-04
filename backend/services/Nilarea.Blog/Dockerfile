FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["backend/services/Nilarea.Blog/Nilarea.Blog.csproj", "backend/services/Nilarea.Blog/"]
COPY ["backend/shared/Nilarea.Contracts/Nilarea.Contracts.csproj", "backend/shared/Nilarea.Contracts/"]
COPY ["backend/shared/Nilarea.Common/Nilarea.Common.csproj", "backend/shared/Nilarea.Common/"]
RUN dotnet restore "backend/services/Nilarea.Blog/Nilarea.Blog.csproj"
COPY . .
WORKDIR "/src/backend/services/Nilarea.Blog"
RUN dotnet build "./Nilarea.Blog.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Nilarea.Blog.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nilarea.Blog.dll"]
