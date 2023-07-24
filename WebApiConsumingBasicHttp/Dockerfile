FROM mcr.microsoft.com/dotnet/sdk:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Install necessary tools
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        unzip \
        procps \
    && rm -rf /var/lib/apt/lists/*
# Install dotnet trace
RUN dotnet tool install --global dotnet-trace --version 7.0.0

# Install dotnet procdump
RUN dotnet tool install --global dotnet-monitor --version 7.0.0

# Install dotnet dump
RUN dotnet tool install --global dotnet-dump 
RUN dotnet tool install --global dotnet-counters
ENV PATH="$PATH:/root/.dotnet/tools"
RUN echo $PATH
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["WebApiConsumingBasicHttp/WebApiConsumingBasicHttp.csproj", "WebApiConsumingBasicHttp/"]
RUN dotnet restore "WebApiConsumingBasicHttp/WebApiConsumingBasicHttp.csproj"
COPY . .
WORKDIR "/src/WebApiConsumingBasicHttp"
RUN dotnet build "WebApiConsumingBasicHttp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebApiConsumingBasicHttp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ["/bin/bash"]
ENTRYPOINT ["dotnet", "WebApiConsumingBasicHttp.dll"]
