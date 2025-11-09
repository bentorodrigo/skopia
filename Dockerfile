# Etapa base - runtime da aplicação
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Etapa de build - SDK do .NET
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia a solution e os arquivos de projeto
COPY ["Skopia.sln", "./"]
COPY ["src/API/API.csproj", "src/API/"]

RUN dotnet restore "src/API/API.csproj"

# Copia todo o código
COPY . .

# Compila
WORKDIR "/src/src/API"
RUN dotnet build "API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publica
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Imagem final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "API.dll"]