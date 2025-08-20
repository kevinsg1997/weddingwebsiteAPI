# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copia todos os arquivos primeiro
COPY . .

# Restaura pacotes
RUN dotnet restore

# Publica a aplicação
RUN dotnet publish -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_USE_POLLING_FILE_WATCHER=1
EXPOSE 5000
ENTRYPOINT ["dotnet", "WeddingMerchantApi.dll"]