# GUIDE DE DEPLOIEMENT EN PRODUCTION (SERVEUR CLIENT EN FRANCE)

> **Projet :** Application Touristique et Audio-Guide de Marrakech  
> **Auteurs :** Mohamed Dicko & Khardi Sanae  
> **Technologies :** ASP.NET Core 9, SQL Server 2022, React Native / Expo, Docker  

---

## 1. Vue d'Ensemble de l'Architecture de Production

```
                                  [ INTERNET ]
                                       │
                        HTTPS (443)    │    HTTP (80)
                                       ▼
                       ┌──────────────────────────────┐
                       │   Reverse Proxy (Nginx)      │
                       │   Certificat SSL Let's Encrypt│
                       └──────────────┬───────────────┘
                                       │
                      Proxy Pass :8080 │
                                       ▼
                       ┌──────────────────────────────┐
                       │    Backend ASP.NET Core 9    │
                       │    (Container ou Service)    │
                       └──────────────┬───────────────┘
                                       │
                       Port 1433       │  Volumes : /app/wwwroot/uploads
                                       ▼
                       ┌──────────────────────────────┐
                       │   Base de Données SQL Server │
                       │    (Instance ou Container)   │
                       └──────────────────────────────┘
```

---

## 2. Option Recommandée : Déploiement Clé en Main avec Docker (Linux / Windows)

Cette méthode est la plus simple et la plus fiable pour livrer chez le client. Elle installe et connecte automatiquement le backend .NET 9 et Microsoft SQL Server en **une seule commande**.

### Prérequis sur le serveur client :
- **Docker** et **Docker Compose** installés ([docker.com](https://www.docker.com/))
- Ports `80`, `443` et `5240` ouverts sur le pare-feu.

### Procédure de déploiement :
1. Récupérer le code sur le serveur client :
   ```bash
   git clone https://github.com/MohaDicko/tourism_app.git
   cd tourism_app
   ```

2. (Optionnel) Ajuster les mots de passe de production dans `docker-compose.yml` :
   - `MSSQL_SA_PASSWORD` : mot de passe fort pour l'administrateur de base de données.

3. Démarrer les services en arrière-plan :
   ```bash
   docker compose up -d --build
   ```

4. Vérifier que tout tourne :
   ```bash
   docker compose ps
   ```

Le Back-Office est immédiatement disponible sur : `http://<IP_DU_SERVEUR>:5240`  
L'API REST est active sur : `http://<IP_DU_SERVEUR>:5240/api/sync/all`

---

## 3. Option Windows Server avec IIS (Si le client a une infrastructure Windows)

Si le client héberge le projet sur un serveur Windows Server dédié avec IIS :

1. **Installer sur Windows Server :**
   - [.NET 9 Hosting Bundle](https://dotnet.microsoft.com/download/dotnet/9.0)
   - Microsoft SQL Server 2019/2022 (ou SQL Server Express)
2. **Créer la Base de Données :**
   - Ouvrir SQL Server Management Studio (SSMS).
   - Ouvrir et exécuter le script inclus dans le projet : `TOURISMAPP/Create_TourismApp_Database.sql`.
3. **Publier l'Application Web :**
   ```powershell
   dotnet publish TourismApp.Web/TourismApp.Web.csproj -c Release -o C:\inetpub\wwwroot\TourismApp
   ```
4. **Dans le Gestionnaire IIS :**
   - Créer un nouveau Site Web pointant vers `C:\inetpub\wwwroot\TourismApp`.
   - Attribuer un Pool d'applications configuré sur **« Aucun code managé »** (No Managed Code).

---

## 4. Configuration HTTPS & Nom de Domaine (Certificat SSL Gratuit)

En France, la conformité de sécurité exige le protocole **HTTPS** avec un nom de domaine (ex: `https://tourisme.nom-du-client.fr`).

### Configuration Nginx type (Reverse Proxy) :
```nginx
server {
    server_name tourisme.nom-du-client.fr;

    client_max_body_size 50M; # Autorise l'upload de fichiers audios MP3 et photos

    location / {
        proxy_pass http://127.0.0.1:5240;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

### Génération automatique du certificat SSL Let's Encrypt :
```bash
sudo certbot --nginx -d tourisme.nom-du-client.fr
```

---

## 5. Configuration et Publication de l'Application Mobile

### 1. Pointer l'application mobile vers le serveur du client :
Dans `TourismApp.Mobile/src/services/api.js`, définir l'URL de production du client :
```javascript
export const DEFAULT_API_BASE = "https://tourisme.nom-du-client.fr";
```

### 2. Générer les binaires pour smartphones :
Avec **Expo Application Services (EAS)** :
```bash
cd TourismApp.Mobile
npm install -g eas-cli
eas login
eas build:configure

# Générer l'APK Android installable directement :
eas build -p android --profile preview

# Générer pour le Google Play Store & Apple App Store :
eas build -p all
```

---

## 6. Maintenance & Sauvegarde

- **Sauvegarde de la base de données SQL Server :**
  ```sql
  BACKUP DATABASE TourismAppDb TO DISK = '/var/opt/mssql/backup/TourismAppDb.bak' WITH FORMAT;
  ```
- **Sauvegarde des médias (images et fichiers audio) :**
  Sauvegarder simplement le dossier `uploads/` situé à la racine du projet.
