#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BuildConfiguration=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameRecord.WGFmGamingService/Tgs.GameRecord.WGFmGamingService.csproj", "Tgs.GameRecord.WGFmGamingService/"]
COPY ["Tgs.GameRecord.WGFmGamingService.Core/Tgs.GameRecord.WGFmGamingService.Core.csproj", "Tgs.GameRecord.WGFmGamingService.Core/"]
RUN dotnet restore "Tgs.GameRecord.WGFmGamingService/Tgs.GameRecord.WGFmGamingService.csproj"
COPY . .
WORKDIR "/src/Tgs.GameRecord.WGFmGamingService"
RUN dotnet build "Tgs.GameRecord.WGFmGamingService.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "Tgs.GameRecord.WGFmGamingService.csproj" -c $BuildConfiguration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameRecord.WGFmGamingService.dll"]