FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/Sample.AzureRedis.Api/Sample.AzureRedis.Api.csproj", "src/Sample.AzureRedis.Api/"]
RUN dotnet restore "src/Sample.AzureRedis.Api/Sample.AzureRedis.Api.csproj"
COPY . .
WORKDIR "/src/src/Sample.AzureRedis.Api"
RUN dotnet build "Sample.AzureRedis.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sample.AzureRedis.Api.csproj" -c Release -o /app/publish

ENV TZ=America/Sao_Paulo
ENV LANG pt-BR
ENV LANGUAGE pt-BR
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sample.AzureRedis.Api.dll"]