# Tourism App Marrakech - Solution Complète (Backend ASP.NET Core & Mobile React Native)

Application touristique et audio-guide pour la ville de Marrakech et sa région (rayon de 100 km), développée d'après le Cahier des Charges du Projet de Fin d'Études (Sup MTI Rabat / Algo Consulting Group).

---

## 📌 Documentation & Cahier des Charges
- **Cahier des charges complet et spécifications techniques :** Voir [`CAHIER_DES_CHARGES.md`](./CAHIER_DES_CHARGES.md) à la racine du projet. Ce document contient tous les diagrammes UML, les cas d'utilisation, le modèle conceptuel de données, les règles métier et la spécification des API.

---

## 🚀 Composants du Projet

### 1. Backend Web & Administration (`TourismApp.Web`)
Développé avec **ASP.NET Core MVC (.NET 9)** et **Entity Framework Core**.
- **Gestion des Endroits / Sites (Figures 13, 14, 15, 18) :**
  - Liste, recherche, filtrage par statut et catégorie.
  - Formulaire d'ajout / modification avec upload photo et upload fichier audio pour l'audio-guide.
  - Fiche détaillée avec carte interactive Leaflet (OpenStreetMap), lecteur audio intégré et géolocalisation.
- **Gestion des Parcours & Itinéraires :**
  - Définition de circuits recommandés avec ordre séquentiel des étapes (table `Ordre` / `Rang`).
  - Tracé cartographique de l'itinéraire.
- **Agenda des Événements Culturels :**
  - Planification avec dates, contacts, lieux associés et suivi des inscriptions.
- **Gestion des Profils Visiteurs (Figure 16) :**
  - Liste des comptes utilisateurs enregistrés.
- **Page d'Erreur Personnalisée (Figure 17) :**
  - Écran 404 stylisé avec bouton de redirection.
- **API REST pour le Mobile :**
  - `GET /api/sync/all` : Synchronisation globale automatique des données (toutes les 15 min côté mobile).
  - `GET /api/endroits` : Recherche, filtrage par catégorie et calcul de proximité géographique ("Mapp me").
  - `GET /api/endroits/{id}` : Fiche détaillée d'un endroit avec métadonnées audio-guide.

---

## 🔑 Identifiants d'Administration par Défaut

- **URL de connexion :** `http://localhost:5240/Account/Login`
- **Username :** `admin` (ou `admin@tourismapp.ma`)
- **Mot de passe :** `admin123`

---

## 💻 Démarrage Rapide du Backend

```powershell
# Cloner ou se placer dans le projet
cd c:\Users\tinkpad\Desktop\ASP.net\tourism_app

# Lancer le serveur backend
dotnet run --project TourismApp.Web/TourismApp.Web.csproj --urls "http://localhost:5240"
```

La base de données SQLite `tourism_app.db` est automatiquement créée et pré-remplie avec les monuments et sites incontournables de Marrakech (Place Jemaa el-Fna, Jardin Majorelle, Palais El Badi, etc.).

Pour utiliser **SQL Server**, il suffit d'activer `"UseSqlServer": true` dans `appsettings.json`.

---

## 📱 Application Mobile (React Native / Expo)

Située dans le dossier [`TourismApp.Mobile/`](./TourismApp.Mobile/), l'application mobile reprend l'ensemble des spécifications du cahier des charges :
- **Mapp :** Consultation des sites par catégories avec barre de recherche.
- **Audio-Guide :** Récits vocaux et descriptions des monuments incontournables.
- **Mapp Me :** Détection de position GPS et calcul de distance par rapport aux sites dans un rayon de 100 km.
- **Parcours :** Circuits pédestres recommandés avec étapes ordonnées.
- **Événements :** Agenda culturel avec inscription et notification.
- **Multilingue :** Français, Anglais, Espagnol (commutable instantanément).
- **Mode Hors-ligne :** Cache local synchronisé automatiquement via `GET /api/sync/all`.

### Lancer l'Application Mobile :

```powershell
cd c:\Users\tinkpad\Desktop\ASP.net\tourism_app\TourismApp.Mobile

# Démarrer le serveur de développement Expo
npx expo start
```
*Scannez simplement le QR Code affiché dans votre terminal avec l'application **Expo Go** (iOS / Android) sur votre smartphone !*
