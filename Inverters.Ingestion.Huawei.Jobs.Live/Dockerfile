FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Inverters.Ingestion.Huawei.Jobs.Live/Inverters.Ingestion.Huawei.Jobs.Live.csproj", "Inverters.Ingestion.Huawei.Jobs.Live/"]
RUN dotnet restore "Inverters.Ingestion.Huawei.Jobs.Live/Inverters.Ingestion.Huawei.Jobs.Live.csproj"
COPY . .
WORKDIR "/src/Inverters.Ingestion.Huawei.Jobs.Live"
RUN dotnet build "Inverters.Ingestion.Huawei.Jobs.Live.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Inverters.Ingestion.Huawei.Jobs.Live.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Inverters.Ingestion.Huawei.Jobs.Live.dll"]
