# CAHIER DES CHARGES : APPLICATION AUDIO-GUIDE ET TOURISME MARRAKECH (M-TOURISME)

> **Projet de Fin d'Études (PFE) - Sup MTI Rabat & Algo Consulting Group**  
> **Auteurs :** Mohamed Dicko & Khardi Sanae  
> **Encadrants :** M. Anass El Hamdi (Algo Consulting Group) & M. Mounir Omari (Sup MTI)  
> **Technologies cibles :** Backend ASP.NET Core MVC / Web API + SQL Server, Mobile React Native (iOS & Android avec Expo / React Native) - *Modernisation recommandée remplaçant Xamarin.Forms (obsolète)*

---

## 1. Contexte et Objectifs Généraux

### 1.1 Contexte
Dans le cadre de l'évolution du E-tourisme vers le **M-tourisme** (tourisme mobile), l'objectif du projet est de doter la ville de Marrakech et sa région (dans un rayon de 100 km) d'une solution numérique innovante offrant autonomie, enrichissement culturel et flexibilité aux touristes.

### 1.2 Objectif de la solution
Développer un écosystème logiciel composé de :
1. **Une application Web Backend (Administration centralisée sous ASP.NET Core MVC)** :
   - Gestion de l'ensemble des données (endroits/sites, parcours guidés, événements culturels, profils utilisateurs).
   - Module d'administration sécurisé par authentification.
   - Exposition de services web / API REST pour synchronisation automatique et consommation par l'application mobile.
   - Hébergement et diffusion des médias (images et guides audios).
2. **Une application Mobile (React Native / iOS & Android)** :
   - Géolocalisation temps réel sur carte interactive.
   - Audio-guide avec lecture vocale / audio et fiches descriptives enrichies.
   - Découverte d'itinéraires et de parcours thématiques.
   - Agenda des événements culturels et possibilité de s'inscrire / participer.
   - Système de notation, avis, favoris et appel téléphonique direct des établissements.
   - Support multilingue (Français, Anglais, Espagnol).
   - Mode synchronisation automatique / hors-ligne via base locale SQLite.

---

## 2. Acteurs du Système

| Acteur | Type | Rôles et Responsabilités |
|---|---|---|
| **Administrateur** | Acteur Principal (Web) | S'authentifier sur le backoffice web, gérer le catalogue des endroits/sites touristiques, gérer les parcours touristiques, gérer l'agenda des événements, consulter et gérer les profils utilisateurs, administrer les rubriques, templates et contenus multimédias. |
| **Utilisateur / Touriste** | Acteur Principal (Mobile) | S'authentifier / créer un compte (ou login social), consulter la carte géographique interactive, rechercher des sites/endroits par catégorie, écouter les guides audio et lire les descriptions, suivre un itinéraire/parcours recommandé, noter un endroit/événement, ajouter aux favoris, contacter les établissements par téléphone. |
| **Système Web (Backend ASP.NET)** | Acteur Secondaire | Stocker les données centralisées, valider les règles métier, héberger et servir les fichiers médias (images, audios), fournir les points de terminaison de synchronisation (API REST). |
| **Système Mobile (Background Service)** | Acteur Secondaire | Exécuter les synchronisations périodiques (ex. toutes les 15 minutes / 900 000 ms), gérer le cache SQLite local, calculer la distance géographique utilisateur-sites. |
| **Service Cartographique (Google Maps / OpenStreetMap)** | Acteur Secondaire | Géolocalisation, affichage cartographique (modes Normal, Satellite, Terrain, Hybride), géocodage et traçage des itinéraires. |

---

## 3. Spécifications Fonctionnelles Détaillées (SFD)

### 3.1 Module Administration Web (ASP.NET Core MVC)

#### SF-WEB-01 : Authentification de l'administrateur
- Formulaire de connexion : Identifiant / Login / Email, Mot de passe, option « Se souvenir de moi ».
- Protection des pages d'administration via filtre d'autorisation.
- Affichage de messages d'erreur explicites en cas d'échec de connexion.
- Option de déconnexion sécurisée.

#### SF-WEB-02 : Tableau de bord & Gestion des Endroits / Sites Touristiques
- **Liste des sites :** Tableau interactif affichant ID, Nom, Type/Catégorie, coordonnées, statut, et boutons d'actions (*Voir*, *Modifier*, *Supprimer*). Bouton *Ajouter un site*.
- **Ajout d'un site touristique (Formulaire Figure 14) :**
  - Nom du site
  - Description détaillée
  - Longitude & Latitude (coordonnées GPS)
  - GSM / Numéro de téléphone de contact
  - Statut (ex: Ouvert, Fermé, En rénovation)
  - Type / Catégorie : Place Publique, Monument, Musée, Jardin, Café/Restaurant, Hébergement/Hôtel, Shopping, Santé, Transport Public, Administration.
  - Attributs spécialisés selon la catégorie (ex: Nombre d'étoiles pour hôtel, horaires ouverture pour café, menu pour restaurant, date de création pour monument).
  - Upload d'image de couverture (PNG/JPG).
  - Upload de fichier audio pour l'audio-guide (MP3/WAV).
- **Détails d'un site (Écran Figure 15) :**
  - Carte interactive centrée sur l'emplacement GPS du site.
  - Affichage de l'image, de la description, des coordonnées, du statut et du contact.
  - Lecteur audio intégré pour écouter le fichier audio associé.
  - Boutons de modification, de suppression et retour à la liste.
- **Modification & Suppression :** Mise à jour en temps réel des informations et suppression logique ou physique.

#### SF-WEB-03 : Gestion des Parcours Touristiques
- Création de parcours touristiques conseillés (Nom, Description, Durée estimée, Distance, Image).
- Association ordonnée d'une suite d'endroits (Table d'association `Ordre` avec `Rang` séquentiel 1, 2, 3...).
- Affichage de l'itinéraire sur carte.

#### SF-WEB-04 : Gestion des Événements Culturels
- Création d'événements (Nom, Description, Date de début, Date de fin, Contact, Lieu/Endroit associé optionnel).
- Suivi de la liste des participants.

#### SF-WEB-05 : Gestion des Profils Utilisateurs (Écran Figure 16)
- Consultation de la liste des utilisateurs enregistrés (Nom, Prénom, Username, Email, Téléphone, Nationalité, Date d'inscription).

#### SF-WEB-06 : Page d'erreur personnalisée (Écran Figure 17)
- Page 404 conviviale avec bouton retour vers l'accueil.

#### SF-WEB-07 : API REST de Synchronisation pour l'application Mobile
- `GET /api/sync/all` : Fournit l'ensemble des données (sites, catégories, parcours, événements).
- `GET /api/sync/delta?lastSyncTimestamp=...` : Synchronisation différentielle.
- `GET /api/endroits` et `GET /api/endroits/{id}` : Liste et détail d'un site (avec URL image, URL audio).
- `GET /api/parcours` et `GET /api/parcours/{id}` : Liste des parcours avec étapes ordonnées.
- `GET /api/evenements` : Liste des événements actuels et à venir.
- `POST /api/evenements/{id}/participer` : Inscription d'un utilisateur à un événement.
- `POST /api/favoris/toggle` : Ajout / Retrait d'un site aux favoris utilisateur.
- `POST /api/notes` : Dépôt d'une note / avis sur un endroit.

---

### 3.2 Spécifications de l'Application Mobile (Client)
- **Choix de la langue :** Français, Anglais, Espagnol (Tableau 2).
- **Authentification mobile :** Login, Inscription complète (Nom, prénom, nationalité, téléphone, login, mot de passe), Login social (Facebook/Google) (Tableau 3).
- **Page d'accueil :**
  - **Mapp :** Carte avec filtre horizontal par catégories (Hébergement, Café/restaurant, Monuments, Shopping, Santé, Transport Public, Administration, Tous, Aller à) et barre de recherche.
  - **Audio-guide :** Liste des incontournables avec lecteur vocal / fichier audio et description.
  - **Mapp me :** Géolocalisation instantanée et affichage des sites dans le périmètre proche.
  - **Paramètres :** Gestion des filtres de notifications, choix de langue, à propos.
- **Fiche Endroit :** Visualisation carte, contact téléphonique immédiat (GSM), ajout/retrait favoris, écoute vocale/audio, notation.
- **Fiche Parcours :** Détail des étapes ordonnées, durée, distance, tracé d'itinéraire sur la carte.
- **Fiche Événement :** Détail, dates, localisation, bouton de participation avec notifications de rappel.
- **Synchronisation automatique en arrière-plan :** Requête vers le serveur web toutes les 15 minutes (`Sleep(900000)`) pour alimenter la base SQLite locale.

---

## 4. Modèle Conceptuel et Relationnel de Données (MCD / MLD)

D'après le diagramme de classe globale (Figure 11, page 40 du rapport) :

### 4.1 Entités Principales

#### 1. `Endroit` (Table: `Endroits`)
- `Id` (int, Clé Primaire, auto-incrément)
- `Nom` (nvarchar(150), obligatoire)
- `Description` (nvarchar(max))
- `Numero` / `GSM` (nvarchar(50))
- `Statut` (int: 1=Ouvert, 0=Fermé, 2=Travaux)
- `Latitude` (float / double, obligatoire)
- `Longitude` (float / double, obligatoire)
- `TypeCategorie` (nvarchar(50) : Monument, Jardin, Restaurant, Cafe, Hebergement, Musee, Shopping, Sante, Transport, Administration, Autre)
- `ImageUrl` (nvarchar(255))
- `AudioUrl` (nvarchar(255))
- *Champs spécialisés (Héritage/Table unique ou tables filles) :*
  - `JardinType` (nvarchar(100))
  - `MonumentDateCreation` (nvarchar(100))
  - `RestaurantMenu` (nvarchar(max))
  - `RestaurantType` (nvarchar(100))
  - `HebergementNombreEtoile` (int, optionnel)
  - `HebergementType` (nvarchar(100) : Hôtel, Riad, Auberge...)
  - `CafeDateOuvert` (nvarchar(50) / TimeSpan)
  - `CafeDateFermer` (nvarchar(50) / TimeSpan)
- `DateCreation` (datetime2)
- `DateModification` (datetime2)

#### 2. `Evenement` (Table: `Evenements`)
- `Id` (int, Clé Primaire)
- `Nom` (nvarchar(150), obligatoire)
- `Description` (nvarchar(max))
- `Date_debut` (datetime2, obligatoire)
- `Date_fin` (datetime2, obligatoire)
- `Contacte` (nvarchar(50))
- `EndroitId` (int, Clé Étrangère optionnelle vers `Endroit`)
- `ImageUrl` (nvarchar(255))

#### 3. `Parcours` / `DestinationConseillee` (Table: `Parcours`)
- `Id` (int, Clé Primaire)
- `Nom` (nvarchar(150), obligatoire)
- `Description` (nvarchar(max))
- `Duree` (nvarchar(50), ex: "2 heures 30 min")
- `Distance` (nvarchar(50), ex: "4.5 km")
- `ImageUrl` (nvarchar(255))

#### 4. `ParcoursEndroit` / `Ordre` (Table d'association: `ParcoursEndroits`)
- `Id` (int, Clé Primaire)
- `ParcoursId` (int, Clé Étrangère vers `Parcours`)
- `EndroitId` (int, Clé Étrangère vers `Endroit`)
- `Rang` (int, Ordre dans le parcours : 1, 2, 3...)

#### 5. `Utilisateur` (Table: `Utilisateurs`)
- `Id` (int, Clé Primaire)
- `Nom` (nvarchar(100))
- `Prenom` (nvarchar(100))
- `Username` (nvarchar(100), unique)
- `Email` (nvarchar(150), unique)
- `PasswordHash` (nvarchar(255))
- `Telephone` (nvarchar(50))
- `Nationalite` (nvarchar(100))
- `Role` (nvarchar(50) : "Admin", "User")
- `DateInscription` (datetime2)

#### 6. `Favori` (Table: `Favoris`)
- `Id` (int, Clé Primaire)
- `Date_ajout` (datetime2)
- `UtilisateurId` (int, Clé Étrangère vers `Utilisateurs`)
- `EndroitId` (int, Clé Étrangère vers `Endroits`)

#### 7. `ParticiperEvenement` (Table: `ParticiperEvenements`)
- `Id` (int, Clé Primaire)
- `Date_part` (datetime2)
- `UtilisateurId` (int, Clé Étrangère vers `Utilisateurs`)
- `EvenementId` (int, Clé Étrangère vers `Evenements`)

#### 8. `NoteEndroit` (Table: `NoteEndroits`)
- `Id` (int, Clé Primaire)
- `Note` (int, de 1 à 5)
- `Commentaire` (nvarchar(max))
- `DateNote` (datetime2)
- `UtilisateurId` (int, Clé Étrangère vers `Utilisateurs`)
- `EndroitId` (int, Clé Étrangère vers `Endroits`)

---

## 5. Architecture Technique Backend (ASP.NET Core MVC)

### 5.1 Structure du Projet
```
tourism_app/
├── CAHIER_DES_CHARGES.md                 <-- Ce document de référence
├── TourismApp.sln                        <-- Solution .NET
└── src/
    └── TourismApp.Web/                   <-- Projet principal ASP.NET Core MVC & API
        ├── Controllers/
        │   ├── AccountController.cs      <-- Authentification Admin & Gestion session
        │   ├── HomeController.cs         <-- Redirections & pages publiques
        │   ├── EndroitsController.cs     <-- CRUD Endroits (Voir, Ajouter, Modifier, Supprimer)
        │   ├── ParcoursController.cs     <-- CRUD Parcours touristiques & étapes ordonnées
        │   ├── EvenementsController.cs   <-- CRUD Événements
        │   ├── UsersController.cs        <-- Affichage des profils visiteurs
        │   └── Api/
        │       ├── SyncApiController.cs  <-- Synchronisation globale (mobile)
        │       ├── EndroitsApiController.cs
        │       ├── ParcoursApiController.cs
        │       └── EvenementsApiController.cs
        ├── Data/
        │   ├── ApplicationDbContext.cs   <-- DbContext Entity Framework Core
        │   └── DbInitializer.cs          <-- Jeux de données initial (Marrakech)
        ├── Models/                       <-- Entités du domaine
        │   ├── Endroit.cs
        │   ├── Evenement.cs
        │   ├── Parcours.cs
        │   ├── ParcoursEndroit.cs
        │   ├── Utilisateur.cs
        │   ├── Favori.cs
        │   ├── ParticiperEvenement.cs
        │   └── NoteEndroit.cs
        ├── ViewModels/                   <-- Modèles de vue (Login, EndroitForm, etc.)
        ├── Views/                        <-- Vues Razor respectant les captures du PFE
        │   ├── Shared/
        │   │   ├── _Layout.cshtml        <-- Layout bleu & moderne (Figure 13)
        │   │   └── _Error404.cshtml      <-- Page d'erreur personnalisée (Figure 17)
        │   ├── Account/Login.cshtml      <-- Page de connexion (Figure 12)
        │   ├── Endroits/
        │   │   ├── Index.cshtml          <-- Liste des sites (Figure 13 & 18)
        │   │   ├── Details.cshtml        <-- Détails avec carte & audio (Figure 15)
        │   │   ├── Create.cshtml         <-- Ajout d'un endroit (Figure 14)
        │   │   └── Edit.cshtml           <-- Modification d'un endroit
        │   ├── Parcours/                 <-- Index, Details, Create, Edit
        │   ├── Evenements/               <-- Index, Details, Create, Edit
        │   └── Users/Index.cshtml        <-- Liste des utilisateurs (Figure 16)
        ├── wwwroot/
        │   ├── css/                      <-- Feuilles de style personnalisées & modernes
        │   ├── js/                       <-- Scripts de validation, carte Leaflet
        │   └── uploads/
        │       ├── images/               <-- Stockage des photos de sites
        │       └── audio/                <-- Stockage des fichiers MP3 audio-guide
        └── appsettings.json
```

---

## 6. Charte Graphique & Expérience Utilisateur (UI/UX)
Conformément aux maquettes du rapport de stage (Figures 12 à 18) :
- **Couleur Primaire :** Bleu océan / turquoise `#0ea5e9` ou `#0099cc`, avec accents foncés `#0f172a`.
- **Top Bar / Header :**
  - Barre de recherche globale `Search for...`
  - Profil connecté avec avatar et nom (ex. "Mohamed Dicko" / Administrateur)
- **Sidebar de navigation :**
  - *Site* (Gestion des sites / endroits)
  - *Profile* (Gestion des utilisateurs)
  - *Parcours* (Gestion des itinéraires)
  - *Événements* (Agenda événementiel)
  - *Error 404* (Accès de démonstration)
- **Composants interactifs :**
  - Cartographie OpenStreetMap / Leaflet (sans clé API restrictive) avec marqueurs personnalisés pour chaque monument de Marrakech.
  - Lecteur audio HTML5 pour écouter l'audio-guide directement dans le backoffice.
  - Modales ou alertes de confirmation avant suppression.
  - Tableaux stylisés avec badges colorés pour les statuts et types.

---

## 7. Données d'Exemple Préchargées (Marrakech et Région)
Pour un rendu immédiat et conforme au cahier des charges :
1. **Place Jemaa el-Fna** (Place publique, Lat: 31.6258, Lng: -7.9891)
2. **Médina de Marrakech** (Place publique / Patrimoine, Lat: 31.6295, Lng: -7.9811)
3. **Palais El Badi** (Monument / Palais, Lat: 31.6183, Lng: -7.9859)
4. **Les Tombeaux Saadiens** (Monument / Tombeau, Lat: 31.6172, Lng: -7.9889)
5. **Musée Dar Si Said** (Musée, Lat: 31.6231, Lng: -7.9836)
6. **Domaine Rosaroum** (Hôtel / Hébergement, Lat: 31.6350, Lng: -8.0050)
7. **La Palette** (Restaurant, Lat: 31.6380, Lng: -8.0120)
8. **Hôpital Ibn Nafis** (Santé / Hôpital, Lat: 31.6420, Lng: -8.0200)
9. **Aéroport de Marrakech-Ménara** (Transport / Aéroport, Lat: 31.6069, Lng: -8.0363)
10. **Diam's Café** (Café, Lat: 31.6310, Lng: -8.0090)
11. **Jardin Majorelle & Musée Yves Saint Laurent** (Jardin / Musée, Lat: 31.6417, Lng: -8.0033)
