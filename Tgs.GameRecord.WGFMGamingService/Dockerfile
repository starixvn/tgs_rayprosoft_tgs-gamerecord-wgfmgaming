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
COPY ["Tgs.GameRecord.WGFMGamingService/Tgs.GameRecord.WGFMGamingService.csproj", "Tgs.GameRecord.WGFMGamingService/"]
COPY ["Tgs.GameRecord.WGFMGamingService.Core/Tgs.GameRecord.WGFMGamingService.Core.csproj", "Tgs.GameRecord.WGFMGamingService.Core/"]
RUN dotnet restore "Tgs.GameRecord.WGFMGamingService/Tgs.GameRecord.WGFMGamingService.csproj"
COPY . .
WORKDIR "/src/Tgs.GameRecord.WGFMGamingService"
RUN dotnet build "Tgs.GameRecord.WGFMGamingService.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "Tgs.GameRecord.WGFMGamingService.csproj" -c $BuildConfiguration -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameRecord.WGFMGamingService.dll"]