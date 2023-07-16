FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["BasicHttpWcfBenchmarks/BasicHttpWcfBenchmarks.csproj", "BasicHttpWcfBenchmarks/"]
RUN dotnet restore "BasicHttpWcfBenchmarks/BasicHttpWcfBenchmarks.csproj"
COPY . .
WORKDIR "/src/BasicHttpWcfBenchmarks"
RUN dotnet build "BasicHttpWcfBenchmarks.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BasicHttpWcfBenchmarks.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM build AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BasicHttpWcfBenchmarks.dll"]