# ========================================================
# Dockerfile Multi-Stage pour Backend ASP.NET Core 9
# Prêt pour Déploiement en Production (Linux / Windows)
# ========================================================

# 1. Stage de Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["TourismApp.Web.csproj", "./"]
RUN dotnet restore "TourismApp.Web.csproj"

COPY . .
RUN dotnet publish "TourismApp.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 2. Stage d'Exécution Runtime (Léger & Sécurisé)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Dossier pour les uploads d'images et audios persistants
RUN mkdir -p /app/wwwroot/uploads/images /app/wwwroot/uploads/audio

COPY --from=build /app/publish .

# Définition des variables d'environnement de production
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "TourismApp.Web.dll"]
