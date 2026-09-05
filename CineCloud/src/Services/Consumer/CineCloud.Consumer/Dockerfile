# Esta fase é usada durante a execução no VS no modo rápido (Padrão para a configuração de Depuração)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# Esta fase é usada para compilar o projeto de serviço
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/Consumer/CineCloud.Consumer/CineCloud.Consumer.csproj", "src/Services/Consumer/CineCloud.Consumer/"]
COPY ["src/Shared/Queries/Infrastructure/CineCloud.Queries.Infrastructure/CineCloud.Queries.Infrastructure.csproj", "src/Shared/Queries/Infrastructure/CineCloud.Queries.Infrastructure/"]
COPY ["src/Shared/Queries/Application/CineCloud.Queries.Application/CineCloud.Queries.Application.csproj", "src/Shared/Queries/Application/CineCloud.Queries.Application/"]
COPY ["src/Shared/Queries/Domain/CineCloud.Queries.Domain/CineCloud.Queries.Domain.csproj", "src/Shared/Queries/Domain/CineCloud.Queries.Domain/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Core/BuildingBlocks.Core.csproj", "src/BuildingBlocks/BuildingBlocks.Core/"]
RUN dotnet restore "./src/Services/Consumer/CineCloud.Consumer/CineCloud.Consumer.csproj"
COPY . .
WORKDIR "/src/src/Services/Consumer/CineCloud.Consumer"
RUN dotnet build "./CineCloud.Consumer.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta fase é usada para publicar o projeto de serviço a ser copiado para a fase final
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CineCloud.Consumer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase é usada na produção ou quando executada no VS no modo normal (padrão quando não está usando a configuração de Depuração)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CineCloud.Consumer.dll"]
