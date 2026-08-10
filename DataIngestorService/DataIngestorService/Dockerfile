# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DataIngestorService/DataIngestorService/DataIngestorService.csproj", "DataIngestorService/DataIngestorService/"]
COPY ["DataIngestorService/DataIngestorService.Contracts/DataIngestorService.Contracts.csproj", "DataIngestorService/DataIngestorService.Contracts/"]
COPY ["DataIngestorService/DataIngestorService.MessagingService/DataIngestorService.MessagingService.csproj", "DataIngestorService/DataIngestorService.MessagingService/"]
RUN dotnet restore "./DataIngestorService/DataIngestorService/DataIngestorService.csproj"
COPY . .
WORKDIR "/src/DataIngestorService/DataIngestorService"
RUN dotnet build "./DataIngestorService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DataIngestorService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DataIngestorService.dll"]