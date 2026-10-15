using System.Security.Cryptography;
using System.Text;
using TourismApp.Web.Models;

namespace TourismApp.Web.Data
{
    public static class DbInitializer
    {
        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Vérification si des données existent déjà
            if (context.Utilisateurs.Any())
            {
                return; // Base déjà initialisée
            }

            // 1. Initialisation des Utilisateurs (Administrateur et visiteurs de la Figure 16)
            var adminUser = new Utilisateur
            {
                Username = "admin",
                First_Name = "Mohamed",
                Last_Name = "Dicko",
                Email = "admin@tourismapp.ma",
                PasswordHash = HashPassword("admin123"),
                Telephone = "+212 600 000000",
                Nationalite = "Marocaine",
                Role = "Admin",
                DateInscription = DateTime.UtcNow.AddMonths(-6)
            };

            var user1 = new Utilisateur
            {
                Username = "Jmilk",
                First_Name = "James",
                Last_Name = "Milner",
                Email = "james.milner@example.com",
                PasswordHash = HashPassword("user123"),
                Telephone = "+44 7911 123456",
                Nationalite = "Britannique",
                Role = "User",
                DateInscription = DateTime.UtcNow.AddMonths(-4)
            };

            var user2 = new Utilisateur
            {
                Username = "Skatmou",
                First_Name = "Khardi",
                Last_Name = "Sanae",
                Email = "sanae.khardi@example.com",
                PasswordHash = HashPassword("user123"),
                Telephone = "+212 611 223344",
                Nationalite = "Marocaine",
                Role = "User",
                DateInscription = DateTime.UtcNow.AddMonths(-3)
            };

            var user3 = new Utilisateur
            {
                Username = "Govinda",
                First_Name = "Sanghani",
                Last_Name = "Gusikowski",
                Email = "sanghani@example.com",
                PasswordHash = HashPassword("user123"),
                Telephone = "+33 612 345678",
                Nationalite = "Française",
                Role = "User",
                DateInscription = DateTime.UtcNow.AddMonths(-2)
            };

            var user4 = new Utilisateur
            {
                Username = "Hritik",
                First_Name = "Roshan",
                Last_Name = "Rogahin",
                Email = "roshan@example.com",
                PasswordHash = HashPassword("user123"),
                Telephone = "+91 98765 43210",
                Nationalite = "Indienne",
                Role = "User",
                DateInscription = DateTime.UtcNow.AddMonths(-1)
            };

            context.Utilisateurs.AddRange(adminUser, user1, user2, user3, user4);
            context.SaveChanges();

            // 2. Initialisation des Endroits / Sites (Figure 13 et 18 du cahier des charges)
            var sites = new List<Endroit>
            {
                new Endroit
                {
                    Nom = "Place Jemaa el-Fna",
                    Description = "Au XIIe siècle, la place Jemaa el-Fna était un lieu de justice où les peines étaient publiquement appliquées. Dès la seconde moitié du XVIe siècle, les activités et fonctions de la place ont évolué. Ainsi, elle est décrite par l'auteur espagnol Carvajal Marmol comme un lieu cosmopolite où règne une forte activité commerciale, entourée de charmeurs de serpents, conteurs, musiciens et marchands d'épices.",
                    Numero = "+212 524 380000",
                    Statut = EndroitStatut.Ouvert,
                    Latitude = 31.6258,
                    Longitude = -7.9891,
                    TypeCategorie = "Place Publique",
                    ImageUrl = "https://images.unsplash.com/photo-1548013146-72479768bada?w=900&auto=format&fit=crop&q=80",
                    AudioUrl = null,
                    DateCreation = DateTime.UtcNow
                },
                new Endroit
                {
                    Nom = "Médina de Marrakech",
                    Description = "Fondée en 1070-1072 par les Almoravides, la Médina de Marrakech est un dédale envoûtant de ruelles animées, de souks parfumés, d'ateliers d'artisans et de riads historiques, classée au patrimoine mondial de l'UNESCO.",
                    Numero = "+212 524 384455",
                    Statut = EndroitStatut.Ouvert,
                    Latitude = 31.6295,
                    Longitude = -7.9811,
                    TypeCategorie = "Place Publique",
                    ImageUrl = "https://images.unsplash.com/photo-1539020140153-e479b8c22e70?w=900&auto=format&fit=crop&q=80",
                    DateCreation = DateTime.UtcNow
                },
                new Endroit
                {
                    Nom = "Palais El Badi",
                    Description = "Édifié à la fin du XVIe siècle par le sultan saadien Ahmed al-Mansur Dhahbi pour célébrer la victoire contre l'armée portugaise, ce palais monumental présentait de somptueux marbres d'Italie et de l'or du Soudan.",
                    Numero = "+212 524 378166",
                    Statut = EndroitStatut.Ouvert,
                    Latitude = 31.6183,
                    Longitude = -7.9859,
                    TypeCategorie = "Palais",
                    MonumentDateCreation = "1578",
                    ImageUrl = "https://images.unsplash.com/photo-1597212618440-806262de4f6b?w=900&auto=format&fit=crop&q=80",
                    DateCreation = DateTime.UtcNow
                },
                new Endroit
                {
                    Nom = "Les tombeaux saadiens",
                    Description = "Nécropole historique de Marrakech abritant les sépultures d'une soixantaine de membres de la dynastie saadienne. Découverts en 1917, ils se distinguent par leurs plafonds en cèdre sculpté et stucs minutieusement ouvragés.",
                    Numero = "+212 524 444000",
                    Statut = EndroitStatut.Ouvert,
                    Latitude = 31.6172,
                    Longitude = -7.9889,
                    TypeCategorie = "Tombeau",
                    MonumentDateCreation = "1557",
                    ImageUrl = "https://images.unsplash.com/photo-1580618672591-eb180b1a973f?w=900&auto=format&fit=crop&q=80",
                    DateCreation = DateTime.UtcNow
                },
                new Endroit
                {
                    Nom = "Musée Dar Si Saïd",
                    Description = "Ancienne demeure palatiale du XIXe siècle transformée en Musée National du Tissage et du Tapis. Témoin d'un art artisanal marocain raffiné avec patios fleuris et plafonds d'inspiration hispano-mauresque.",
                    Numero = "+212 524 389564",
                    Statut = EndroitStatut.Ouvert,
                    Latitude = 31.6231,
                    Longitude = -7.9836,
                    TypeCategorie = "Musée",
                    ImageUrl = "https://images.unsplash.com/photo-1579783902614-a3fb3927b675?w=900&auto=format&fit=crop&q=80",
                    DateCreation = DateTime.UtcNow
                },
                new Endroit
                {
                    Nom = "Domaine Rosaroum",
                    Description = "Havre de paix niché dans la palmeraie de Marrakech, proposant hébergement de luxe, suites raffinées et vue imprenable sur la chaîne de l'Atlas.",
                    Numero = "+212 524 329000",
                    Statut = EndroitStatut.Ouvert,
                    Latitude = 31.6350,
                    Longitude = -8.0050,
                    TypeCategorie = "Hôtel",
                    HebergementNombreEtoile = 5,
                    HebergementType = "Hôtel de Charme / Riad",
                    ImageUrl = "https://images.unsplash.com/photo-1566073771259-6a8506099945?w=900&auto=format&fit=crop&q=80",
                    DateCreation = DateTime.UtcNow
                },
                new Endroit
                {
                    Nom = "La palette",
                    Description = "Restaurant gastronomique convivial situé au quartier Guéliz, combinant cuisine marocaine revisitée et inspirations méditerranéennes dans un cadre artistique branché.",
                    Numero = "+212 524 430000",
                    Statut = EndroitStatut.Ouvert,
                    Latitude = 31.6380,
                    Longitude = -8.0120,
                    TypeCategorie = "Restaurant",
                    RestaurantType = "Bistronomique & Marocain",
                    RestaurantMenu = "Tajines raffinés, Pastilla au pigeon, Poissons du jour, Desserts à la fleur d'oranger",
                    ImageUrl = "https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=900&auto=format&fit=crop&q=80",
                    DateCreation = DateTime.UtcNow
                },
                new Endroit
                {
                    Nom = "Hôpitale Ibn Nafis",
                    Description = "Centre hospitalier universitaire et pôle médical spécialisé, offrant des soins d'urgence de qualité pour résidents et voyageurs.",
                    Numero = "+212 524 353535",
                    Statut = EndroitStatut.Ouvert,
                    Latitude = 31.6420,
                    Longitude = -8.0200,
                    TypeCategorie = "Hôpitale",
                    ImageUrl = "https://images.unsplash.com/photo-1586773860418-d37222d8fce3?w=900&auto=format&fit=crop&q=80",
                    DateCreation = DateTime.UtcNow
                },
                new Endroit
                {
                    Nom = "Aéroport de Marrakech-Ménara",
                    Description = "Aéroport international moderne, porte d'entrée majeure du Royaume, réputé pour son architecture avant-gardiste mêlant motifs traditionnels islamiques et modernisme verrier.",
                    Numero = "+212 524 447910",
                    Statut = EndroitStatut.Ouvert,
                    Latitude = 31.6069,
                    Longitude = -8.0363,
                    TypeCategorie = "Aéroport",
                    ImageUrl = "https://images.unsplash.com/photo-1520437358207-323b43b50729?w=900&auto=format&fit=crop&q=80",
                    DateCreation = DateTime.UtcNow
                },
                new Endroit
                {
                    Nom = "Diam's café",
                    Description = "Salon de café branché avec terrasse ombragée, idéal pour savourer un thé à la menthe marocain ou un espresso d'origine tout en profitant du soleil de Marrakech.",
                    Numero = "+212 524 421200",
                    Statut = EndroitStatut.Ouvert,
                    Latitude = 31.6310,
                    Longitude = -8.0090,
                    TypeCategorie = "Café",
                    CafeDateOuvert = "07:30",
                    CafeDateFermer = "23:30",
                    ImageUrl = "https://images.unsplash.com/photo-1554118811-1e0d58224f24?w=900&auto=format&fit=crop&q=80",
                    DateCreation = DateTime.UtcNow
                },
                new Endroit
                {
                    Nom = "Jardin Majorelle",
                    Description = "Jardin botanique enchanteur créé par Jacques Majorelle puis restauré par Yves Saint Laurent et Pierre Bergé. Célèbre pour son bleu Majorelle éclatant et sa collection mondiale de cactées et plantes exotiques.",
                    Numero = "+212 524 313047",
                    Statut = EndroitStatut.Ouvert,
                    Latitude = 31.6417,
                    Longitude = -8.0033,
                    TypeCategorie = "Jardin",
                    JardinType = "Botanique / Exotique",
                    ImageUrl = "https://images.unsplash.com/photo-1589182373726-e4f658ab50f0?w=900&auto=format&fit=crop&q=80",
                    DateCreation = DateTime.UtcNow
                },
                new Endroit
                {
                    Nom = "Mosquée Koutoubia",
                    Description = "Chef-d'œuvre de l'architecture almohade du XIIe siècle, son minaret de 77 mètres domine la cité ocre et a servi de modèle à la Giralda de Séville.",
                    Numero = "+212 524 389900",
                    Statut = EndroitStatut.Ouvert,
                    Latitude = 31.6238,
                    Longitude = -7.9936,
                    TypeCategorie = "Monument",
                    MonumentDateCreation = "1158",
                    ImageUrl = "https://images.unsplash.com/photo-1548013146-72479768bada?w=900&auto=format&fit=crop&q=80",
                    DateCreation = DateTime.UtcNow
                }
            };

            context.Endroits.AddRange(sites);
            context.SaveChanges();

            // 3. Initialisation des Parcours conseillés (avec étapes ordonnées)
            var p1 = new Parcours
            {
                Nom = "Circuit Trésors Historiques de la Médina",
                Description = "Un itinéraire pédestre inoubliable traversant les monuments emblématiques de Marrakech, des Tombeaux Saadiens jusqu'à l'incontournable Place Jemaa el-Fna.",
                Duree = "3 heures 30 min",
                Distance = "4.2 km",
                ImageUrl = "https://images.unsplash.com/photo-1539020140153-e479b8c22e70?w=900&auto=format&fit=crop&q=80"
            };

            var p2 = new Parcours
            {
                Nom = "Flânerie Majorelle et Art Moderne à Guéliz",
                Description = "Découverte des jardins artistiques, de la haute couture, pause café gourmande et galeries contemporaines de la ville nouvelle.",
                Duree = "2 heures 15 min",
                Distance = "3.1 km",
                ImageUrl = "https://images.unsplash.com/photo-1589182373726-e4f658ab50f0?w=900&auto=format&fit=crop&q=80"
            };

            context.Parcours.AddRange(p1, p2);
            context.SaveChanges();

            // Association des étapes aux parcours
            var jemaa = sites.First(s => s.Nom == "Place Jemaa el-Fna");
            var palais = sites.First(s => s.Nom == "Palais El Badi");
            var tombeaux = sites.First(s => s.Nom == "Les tombeaux saadiens");
            var musee = sites.First(s => s.Nom == "Musée Dar Si Saïd");
            var majorelle = sites.First(s => s.Nom == "Jardin Majorelle");
            var diams = sites.First(s => s.Nom == "Diam's café");

            context.ParcoursEndroits.AddRange(
                new ParcoursEndroit { ParcoursId = p1.Id, EndroitId = tombeaux.Id, Rang = 1 },
                new ParcoursEndroit { ParcoursId = p1.Id, EndroitId = palais.Id, Rang = 2 },
                new ParcoursEndroit { ParcoursId = p1.Id, EndroitId = musee.Id, Rang = 3 },
                new ParcoursEndroit { ParcoursId = p1.Id, EndroitId = jemaa.Id, Rang = 4 },

                new ParcoursEndroit { ParcoursId = p2.Id, EndroitId = majorelle.Id, Rang = 1 },
                new ParcoursEndroit { ParcoursId = p2.Id, EndroitId = diams.Id, Rang = 2 }
            );

            // 4. Initialisation des Événements culturels
            var ev1 = new Evenement
            {
                Nom = "Festival International du Film de Marrakech (FIFM)",
                Description = "L'un des rendez-vous cinématographiques les plus prestigieux du continent africain réunissant les plus grandes stars internationales sur la Place Jemaa el-Fna et au Palais des Congrès.",
                DateDebut = DateTime.Today.AddDays(10),
                DateFin = DateTime.Today.AddDays(18),
                Contacte = "+212 524 398877",
                EndroitId = jemaa.Id,
                ImageUrl = "https://images.unsplash.com/photo-1517604931442-7e0c8ed2963c?w=900&auto=format&fit=crop&q=80"
            };

            var ev2 = new Evenement
            {
                Nom = "Festival National des Arts Populaires (FNAP)",
                Description = "Célébration festive du patrimoine immatériel marocain avec troupes folkloriques, chants et danses traditionnelles dans le somptueux décor du Palais El Badi.",
                DateDebut = DateTime.Today.AddDays(25),
                DateFin = DateTime.Today.AddDays(30),
                Contacte = "+212 524 380123",
                EndroitId = palais.Id,
                ImageUrl = "https://images.unsplash.com/photo-1492684223066-81342ee5ff30?w=900&auto=format&fit=crop&q=80"
            };

            context.Evenements.AddRange(ev1, ev2);

            // 5. Quelques avis / favoris de test
            context.Favoris.Add(new Favori
            {
                UtilisateurId = user1.Id,
                EndroitId = jemaa.Id,
                Date_ajout = DateTime.UtcNow.AddDays(-5)
            });

            context.NoteEndroits.Add(new NoteEndroit
            {
                UtilisateurId = user1.Id,
                EndroitId = jemaa.Id,
                Note = 5,
                Commentaire = "Ambiance magique le soir au coucher du soleil !",
                DateNote = DateTime.UtcNow.AddDays(-4)
            });

            context.SaveChanges();
        }
    }
}
