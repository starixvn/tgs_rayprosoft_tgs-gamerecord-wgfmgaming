#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameRecord.WGFmGaming/Tgs.GameRecord.WGFmGaming.csproj", "Tgs.GameRecord.WGFmGaming/"]
COPY ["Tgs.GameRecord.WGFmGaming.Core/Tgs.GameRecord.WGFmGaming.Core.csproj", "Tgs.GameRecord.WGFmGaming.Core/"]
COPY ["Tgs.GameRecord.WGFmGaming.Common/Tgs.GameRecord.WGFmGaming.Common.csproj", "Tgs.GameRecord.WGFmGaming.Common/"]
RUN dotnet restore "./Tgs.GameRecord.WGFmGaming/./Tgs.GameRecord.WGFmGaming.csproj"
COPY . .
WORKDIR "/src/Tgs.GameRecord.WGFmGaming"
RUN dotnet build "./Tgs.GameRecord.WGFmGaming.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tgs.GameRecord.WGFmGaming.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameRecord.WGFmGaming.dll"]