# Esta fase é usada durante a execução no VS no modo rápido (Padrão para a configuração de Depuração)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# Esta fase é usada para compilar o projeto de serviço
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/Publisher/Presentation/CineCloud.WebApi/CineCloud.WebApi.csproj", "src/Services/Publisher/Presentation/CineCloud.WebApi/"]
COPY ["src/Services/Publisher/Infrastructure/CineCloud.Infrastructure/CineCloud.Infrastructure.csproj", "src/Services/Publisher/Infrastructure/CineCloud.Infrastructure/"]
COPY ["src/Services/Publisher/Application/CineCloud.Application/CineCloud.Application.csproj", "src/Services/Publisher/Application/CineCloud.Application/"]
COPY ["src/Services/Publisher/Domain/CineCloud.Domain/CineCloud.Domain.csproj", "src/Services/Publisher/Domain/CineCloud.Domain/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Core/BuildingBlocks.Core.csproj", "src/BuildingBlocks/BuildingBlocks.Core/"]
COPY ["src/Shared/Queries/Infrastructure/CineCloud.Queries.Infrastructure/CineCloud.Queries.Infrastructure.csproj", "src/Shared/Queries/Infrastructure/CineCloud.Queries.Infrastructure/"]
COPY ["src/Shared/Queries/Application/CineCloud.Queries.Application/CineCloud.Queries.Application.csproj", "src/Shared/Queries/Application/CineCloud.Queries.Application/"]
COPY ["src/Shared/Queries/Domain/CineCloud.Queries.Domain/CineCloud.Queries.Domain.csproj", "src/Shared/Queries/Domain/CineCloud.Queries.Domain/"]
RUN dotnet restore "./src/Services/Publisher/Presentation/CineCloud.WebApi/CineCloud.WebApi.csproj"
COPY . .
WORKDIR "/src/src/Services/Publisher/Presentation/CineCloud.WebApi"
RUN dotnet build "./CineCloud.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta fase é usada para publicar o projeto de serviço a ser copiado para a fase final
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CineCloud.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase é usada na produção ou quando executada no VS no modo normal (padrão quando não está usando a configuração de Depuração)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CineCloud.WebApi.dll"]
