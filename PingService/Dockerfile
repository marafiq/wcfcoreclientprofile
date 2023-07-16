FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 5002
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["PingService/PingService.csproj", "PingService/"]
RUN dotnet restore "PingService/PingService.csproj"
COPY . .
WORKDIR "/src/PingService"
RUN dotnet build "PingService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PingService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PingService.dll"]
