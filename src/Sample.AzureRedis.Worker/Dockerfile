FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/Sample.AzureRedis.Worker/Sample.AzureRedis.Worker.csproj", "src/Sample.AzureRedis.Worker/"]
RUN dotnet restore "src/Sample.AzureRedis.Worker/Sample.AzureRedis.Worker.csproj"
COPY . .
WORKDIR "/src/src/Sample.AzureRedis.Worker"
RUN dotnet build "Sample.AzureRedis.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sample.AzureRedis.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sample.AzureRedis.Worker.dll"]