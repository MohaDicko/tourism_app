-- ==================================================================================
-- PROJET DE FIN D'ETUDES : APPLICATION TOURISTIQUE ET AUDIO-GUIDE MARRAKECH
-- SCHEMA RELATIONNEL ET DONNEES INITIALES POUR MICROSOFT SQL SERVER
-- Auteurs : Mohamed Dicko & Khardi Sanae
-- Encadrants : M. Anass El Hamdi (Algo Consulting Group) & M. Mounir Omari (SUP-MTI)
-- ==================================================================================

-- 1. CREATION DE LA BASE DE DONNEES
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'TourismAppDb')
BEGIN
    CREATE DATABASE TourismAppDb;
END
GO

USE TourismAppDb;
GO

-- 2. NETTOYAGE PREALABLE (SI LES TABLES EXISTENT DEJA)
IF OBJECT_ID('NoteEndroits', 'U') IS NOT NULL DROP TABLE NoteEndroits;
IF OBJECT_ID('ParticiperEvenements', 'U') IS NOT NULL DROP TABLE ParticiperEvenements;
IF OBJECT_ID('Favoris', 'U') IS NOT NULL DROP TABLE Favoris;
IF OBJECT_ID('Evenements', 'U') IS NOT NULL DROP TABLE Evenements;
IF OBJECT_ID('ParcoursEndroits', 'U') IS NOT NULL DROP TABLE ParcoursEndroits;
IF OBJECT_ID('Parcours', 'U') IS NOT NULL DROP TABLE Parcours;
IF OBJECT_ID('Endroits', 'U') IS NOT NULL DROP TABLE Endroits;
IF OBJECT_ID('Utilisateurs', 'U') IS NOT NULL DROP TABLE Utilisateurs;
GO

-- ==================================================================================
-- 3. CREATION DES TABLES DU SYSTEME
-- ==================================================================================

-- Table : Utilisateurs (Profiles Visiteurs & Administrateurs)
CREATE TABLE Utilisateurs (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nom NVARCHAR(100) NOT NULL,
    Prenom NVARCHAR(100) NOT NULL,
    Username NVARCHAR(100) NOT NULL UNIQUE,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Telephone NVARCHAR(50) NULL,
    Nationalite NVARCHAR(100) NULL,
    Role NVARCHAR(50) NOT NULL DEFAULT 'User', -- 'Admin' ou 'User'
    DateInscription DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO

-- Table : Endroits (Monuments, Sites, Cafes, Jardins, Hebergements...)
CREATE TABLE Endroits (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nom NVARCHAR(150) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    Numero NVARCHAR(50) NULL,
    Statut INT NOT NULL DEFAULT 1, -- 1=Ouvert, 0=Ferme, 2=Travaux
    Latitude FLOAT NOT NULL,
    Longitude FLOAT NOT NULL,
    TypeCategorie NVARCHAR(60) NOT NULL, -- Place Publique, Monument, Musee, Jardin, Restaurant, Cafe, Hotel, Hopitale, Aeroport...
    ImageUrl NVARCHAR(255) NULL,
    AudioUrl NVARCHAR(255) NULL,
    
    -- Specialisations selon Diagramme de Classe (Figure 11 du PFE)
    JardinType NVARCHAR(100) NULL,
    MonumentDateCreation NVARCHAR(100) NULL,
    RestaurantMenu NVARCHAR(MAX) NULL,
    RestaurantType NVARCHAR(100) NULL,
    HebergementNombreEtoile INT NULL,
    HebergementType NVARCHAR(100) NULL,
    CafeDateOuvert NVARCHAR(50) NULL,
    CafeDateFermer NVARCHAR(50) NULL,
    
    DateCreation DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    DateModification DATETIME2 NULL
);
GO

-- Table : Parcours (Destinations Conseillees)
CREATE TABLE Parcours (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nom NVARCHAR(150) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    Duree NVARCHAR(50) NULL,
    Distance NVARCHAR(50) NULL,
    ImageUrl NVARCHAR(255) NULL,
    DateCreation DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO

-- Table associative : ParcoursEndroits (Entite 'Ordre' avec attribut 'Rang')
CREATE TABLE ParcoursEndroits (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ParcoursId INT NOT NULL,
    EndroitId INT NOT NULL,
    Rang INT NOT NULL DEFAULT 1,
    CONSTRAINT FK_ParcoursEndroits_Parcours FOREIGN KEY (ParcoursId) REFERENCES Parcours(Id) ON DELETE CASCADE,
    CONSTRAINT FK_ParcoursEndroits_Endroits FOREIGN KEY (EndroitId) REFERENCES Endroits(Id) ON DELETE CASCADE
);
GO

-- Table : Evenements Culturels
CREATE TABLE Evenements (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nom NVARCHAR(150) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    DateDebut DATETIME2 NOT NULL,
    DateFin DATETIME2 NOT NULL,
    Contacte NVARCHAR(50) NULL,
    ImageUrl NVARCHAR(255) NULL,
    EndroitId INT NULL,
    DateCreation DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Evenements_Endroit FOREIGN KEY (EndroitId) REFERENCES Endroits(Id) ON DELETE SET NULL
);
GO

-- Table : Favoris
CREATE TABLE Favoris (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Date_ajout DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UtilisateurId INT NOT NULL,
    EndroitId INT NOT NULL,
    CONSTRAINT FK_Favoris_Utilisateur FOREIGN KEY (UtilisateurId) REFERENCES Utilisateurs(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Favoris_Endroit FOREIGN KEY (EndroitId) REFERENCES Endroits(Id) ON DELETE CASCADE
);
GO

-- Table : ParticiperEvenements
CREATE TABLE ParticiperEvenements (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Date_part DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UtilisateurId INT NOT NULL,
    EvenementId INT NOT NULL,
    CONSTRAINT FK_Participer_Utilisateur FOREIGN KEY (UtilisateurId) REFERENCES Utilisateurs(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Participer_Evenement FOREIGN KEY (EvenementId) REFERENCES Evenements(Id) ON DELETE CASCADE
);
GO

-- Table : NoteEndroits (Avis et Notes de 1 a 5 etoiles)
CREATE TABLE NoteEndroits (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Note INT NOT NULL CHECK (Note >= 1 AND Note <= 5),
    Commentaire NVARCHAR(MAX) NULL,
    DateNote DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UtilisateurId INT NOT NULL,
    EndroitId INT NOT NULL,
    CONSTRAINT FK_NoteEndroits_Utilisateur FOREIGN KEY (UtilisateurId) REFERENCES Utilisateurs(Id) ON DELETE CASCADE,
    CONSTRAINT FK_NoteEndroits_Endroit FOREIGN KEY (EndroitId) REFERENCES Endroits(Id) ON DELETE CASCADE
);
GO

-- ==================================================================================
-- 4. INSERTION DES DONNEES DE DEMO DU PFE (MARRAKECH)
-- ==================================================================================

-- Utilisateurs (Figure 16 du Cahier des Charges)
-- Mot de passe hashé SHA-256 pour 'admin123' : jZae727K08KaOmKSgOaGzww/XVqGr/hbm9TZyZfCXbA=
INSERT INTO Utilisateurs (Prenom, Nom, Username, Email, PasswordHash, Telephone, Nationalite, Role)
VALUES
('Mohamed', 'Dicko', 'admin', 'admin@tourismapp.ma', 'jZae727K08KaOmKSgOaGzww/XVqGr/hbm9TZyZfCXbA=', '+212 600 000000', 'Marocaine', 'Admin'),
('James', 'Milner', 'Jmilk', 'james.milner@example.com', 'jZae727K08KaOmKSgOaGzww/XVqGr/hbm9TZyZfCXbA=', '+44 7911 123456', 'Britannique', 'User'),
('Sanae', 'Khardi', 'Skatmou', 'sanae.khardi@example.com', 'jZae727K08KaOmKSgOaGzww/XVqGr/hbm9TZyZfCXbA=', '+212 611 223344', 'Marocaine', 'User'),
('Sanghani', 'Gusikowski', 'Govinda', 'sanghani@example.com', 'jZae727K08KaOmKSgOaGzww/XVqGr/hbm9TZyZfCXbA=', '+33 612 345678', 'Française', 'User'),
('Roshan', 'Rogahin', 'Hritik', 'roshan@example.com', 'jZae727K08KaOmKSgOaGzww/XVqGr/hbm9TZyZfCXbA=', '+91 98765 43210', 'Indienne', 'User');
GO

-- Endroits & Sites Touristiques (Figures 13, 15, 18 du PFE)
INSERT INTO Endroits (Nom, Description, Numero, Statut, Latitude, Longitude, TypeCategorie, ImageUrl, MonumentDateCreation, JardinType, RestaurantMenu, RestaurantType, HebergementNombreEtoile, HebergementType, CafeDateOuvert, CafeDateFermer)
VALUES
('Place Jemaa el-Fna', 'Au XIIe siècle, la place Jemaa el-Fna était un lieu de justice où les peines étaient publiquement appliquées. Dès la seconde moitié du XVIe siècle, les activités et fonctions de la place ont évolué. Elle est décrite comme un lieu cosmopolite où règne une forte activité commerciale, entourée de charmeurs de serpents, conteurs et marchands.', '+212 524 380000', 1, 31.6258, -7.9891, 'Place Publique', 'https://images.unsplash.com/photo-1548013146-72479768bada?w=900&auto=format&fit=crop&q=80', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('Médina de Marrakech', 'Fondée en 1070 par les Almoravides, la Médina de Marrakech est un dédale envoûtant de ruelles animées, de souks parfumés, d''ateliers d''artisans et de riads historiques classés au patrimoine mondial de l''UNESCO.', '+212 524 384455', 1, 31.6295, -7.9811, 'Place Publique', 'https://images.unsplash.com/photo-1539020140153-e479b8c22e70?w=900&auto=format&fit=crop&q=80', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('Palais El Badi', 'Édifié à la fin du XVIe siècle par le sultan saadien Ahmed al-Mansur pour célébrer sa victoire, ce palais monumental présentait de somptueux marbres d''Italie et de l''or du Soudan.', '+212 524 378166', 1, 31.6183, -7.9859, 'Palais', 'https://images.unsplash.com/photo-1597212618440-806262de4f6b?w=900&auto=format&fit=crop&q=80', '1578', NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('Les tombeaux saadiens', 'Nécropole historique de Marrakech abritant les sépultures d''une soixantaine de membres de la dynastie saadienne, avec de splendides plafonds en cèdre sculpté et stucs minutieux.', '+212 524 444000', 1, 31.6172, -7.9889, 'Tombeau', 'https://images.unsplash.com/photo-1580618672591-eb180b1a973f?w=900&auto=format&fit=crop&q=80', '1557', NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('Musée Dar Si Saïd', 'Ancienne demeure palatiale du XIXe siècle transformée en Musée National du Tissage et du Tapis avec patios fleuris et plafonds hispano-mauresques.', '+212 524 389564', 1, 31.6231, -7.9836, 'Musée', 'https://images.unsplash.com/photo-1579783902614-a3fb3927b675?w=900&auto=format&fit=crop&q=80', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('Domaine Rosaroum', 'Havre de paix niché dans la palmeraie de Marrakech, proposant suites raffinées et vue imprenable sur l''Atlas.', '+212 524 329000', 1, 31.6350, -8.0050, 'Hôtel', 'https://images.unsplash.com/photo-1566073771259-6a8506099945?w=900&auto=format&fit=crop&q=80', NULL, NULL, NULL, NULL, 5, 'Hôtel de Charme / Riad', NULL, NULL),
('La palette', 'Restaurant gastronomique convivial situé à Guéliz, combinant cuisine marocaine revisitée et inspirations méditerranéennes.', '+212 524 430000', 1, 31.6380, -8.0120, 'Restaurant', 'https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=900&auto=format&fit=crop&q=80', NULL, NULL, 'Tajines réinventés, Pastilla, Desserts à la fleur d''oranger', 'Bistronomique & Marocain', NULL, NULL, NULL, NULL),
('Hôpitale Ibn Nafis', 'Pôle hospitalier universitaire de référence pour résidents et voyageurs.', '+212 524 353535', 1, 31.6420, -8.0200, 'Hôpitale', 'https://images.unsplash.com/photo-1586773860418-d37222d8fce3?w=900&auto=format&fit=crop&q=80', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('Aéroport de Marrakech-Ménara', 'Aéroport international réputé pour son architecture avant-gardiste mêlant motifs traditionnels islamiques et modernisme verrier.', '+212 524 447910', 1, 31.6069, -8.0363, 'Aéroport', 'https://images.unsplash.com/photo-1520437358207-323b43b50729?w=900&auto=format&fit=crop&q=80', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('Diam''s café', 'Salon de café branché avec terrasse ombragée, idéal pour savourer un thé à la menthe marocain ou un espresso d''origine.', '+212 524 421200', 1, 31.6310, -8.0090, 'Café', 'https://images.unsplash.com/photo-1554118811-1e0d58224f24?w=900&auto=format&fit=crop&q=80', NULL, NULL, NULL, NULL, NULL, NULL, '07:30', '23:30'),
('Jardin Majorelle', 'Jardin botanique enchanteur créé par Jacques Majorelle puis restauré par Yves Saint Laurent et Pierre Bergé, célèbre pour son bleu outremer iconique.', '+212 524 313047', 1, 31.6417, -8.0033, 'Jardin', 'https://images.unsplash.com/photo-1589182373726-e4f658ab50f0?w=900&auto=format&fit=crop&q=80', NULL, 'Botanique & Exotique', NULL, NULL, NULL, NULL, NULL, NULL),
('Mosquée Koutoubia', 'Chef-d''œuvre de l''architecture almohade du XIIe siècle, son minaret de 77 mètres domine la cité ocre.', '+212 524 389900', 1, 31.6238, -7.9936, 'Monument', 'https://images.unsplash.com/photo-1548013146-72479768bada?w=900&auto=format&fit=crop&q=80', '1158', NULL, NULL, NULL, NULL, NULL, NULL, NULL);
GO

-- Parcours
INSERT INTO Parcours (Nom, Description, Duree, Distance, ImageUrl)
VALUES
('Circuit Trésors Historiques de la Médina', 'Itinéraire traversant les joyaux séculaires, des Tombeaux Saadiens jusqu''à la Place Jemaa el-Fna.', '3h30', '4.2 km', 'https://images.unsplash.com/photo-1539020140153-e479b8c22e70?w=900&auto=format&fit=crop&q=80'),
('Flânerie Majorelle et Art Moderne à Guéliz', 'Balade botanique et culturelle entre le Jardin Majorelle, galeries contemporaines et terrasses branchées.', '2h15', '3.1 km', 'https://images.unsplash.com/photo-1589182373726-e4f658ab50f0?w=900&auto=format&fit=crop&q=80');
GO

-- Etapes des Parcours (Ordre / Rang)
INSERT INTO ParcoursEndroits (ParcoursId, EndroitId, Rang)
VALUES
(1, 4, 1), -- Tombeaux Saadiens
(1, 3, 2), -- Palais El Badi
(1, 5, 3), -- Musée Dar Si Saïd
(1, 1, 4), -- Place Jemaa el-Fna
(2, 11, 1), -- Jardin Majorelle
(2, 10, 2); -- Diam's Café
GO

-- Événements
INSERT INTO Evenements (Nom, Description, DateDebut, DateFin, Contacte, EndroitId, ImageUrl)
VALUES
('Festival International du Film de Marrakech (FIFM)', 'Projection de films du monde entier en plein air sur la Place Jemaa el-Fna et au Palais des Congrès.', DATEADD(day, 10, GETUTCDATE()), DATEADD(day, 18, GETUTCDATE()), '+212 524 398877', 1, 'https://images.unsplash.com/photo-1517604931442-7e0c8ed2963c?w=900&auto=format&fit=crop&q=80'),
('Festival National des Arts Populaires (FNAP)', 'Célébration du patrimoine immatériel et danses folkloriques au cœur du Palais El Badi.', DATEADD(day, 25, GETUTCDATE()), DATEADD(day, 30, GETUTCDATE()), '+212 524 380123', 3, 'https://images.unsplash.com/photo-1492684223066-81342ee5ff30?w=900&auto=format&fit=crop&q=80');
GO

-- Favoris et Notes
INSERT INTO Favoris (UtilisateurId, EndroitId, Date_ajout)
VALUES (2, 1, GETUTCDATE());

INSERT INTO NoteEndroits (UtilisateurId, EndroitId, Note, Commentaire, DateNote)
VALUES (2, 1, 5, 'Une atmosphère féerique incontournable le soir !', GETUTCDATE());
GO

PRINT '=======================================================';
PRINT 'BASE DE DONNEES TOURISMAPDB INITIALISEE AVEC SUCCES !';
PRINT '=======================================================';
GO
