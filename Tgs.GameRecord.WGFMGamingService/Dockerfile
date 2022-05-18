#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BuildConfiguration=Release
RUN --mount=type=secret,id=nuget,dst=/pat cp /pat ~/.nuget/NuGet/NuGet.Config
RUN dotnet nuget remove source 'Microsoft Visual Studio Offline Packages' || true
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["Tgs.GameRecord.WGFmGamingService/Tgs.GameRecord.WGFmGamingService.csproj", "Tgs.GameRecord.WGFmGamingService/"]
COPY ["Tgs.GameRecord.WGFmGamingService.Core/Tgs.GameRecord.WGFmGamingService.Core.csproj", "Tgs.GameRecord.WGFmGamingService.Core/"]
RUN dotnet restore "Tgs.GameRecord.WGFmGamingService/Tgs.GameRecord.WGFmGamingService.csproj"
COPY . .
WORKDIR "/src/Tgs.GameRecord.WGFmGamingService"
RUN dotnet build "Tgs.GameRecord.WGFmGamingServicee.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "Tgs.GameRecord.WGFmGamingService.csproj" -c $BuildConfiguration -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameRecord.WGFmGamingService.dll"]