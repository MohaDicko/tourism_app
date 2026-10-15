import AsyncStorage from '@react-native-async-storage/async-storage';

// URL par défaut du backend ASP.NET Core MVC (modifiable dans les réglages)
export const DEFAULT_API_BASE = "http://10.0.2.2:5240"; // 10.0.2.2 pour émulateur Android, ou http://localhost:5240 sur Web

const CACHE_KEY = "TOURISME_DATA_CACHE";
const FAVORIS_KEY = "TOURISME_FAVORIS";
const PARTICIPATIONS_KEY = "TOURISME_PARTICIPATIONS";

// Données initiales embarquées de secours (Marrakech)
const FALLBACK_DATA = {
  categories: [
    { id: 0, nom: "Tous", icone: "apps" },
    { id: 1, nom: "Place Publique", icone: "map-marker" },
    { id: 2, nom: "Monument", icone: "castle" },
    { id: 3, nom: "Musée", icone: "bank" },
    { id: 4, nom: "Jardin", icone: "flower" },
    { id: 5, nom: "Restaurant", icone: "silverware-fork-knife" },
    { id: 6, nom: "Café", icone: "coffee" },
    { id: 7, nom: "Hôtel", icone: "bed" },
    { id: 8, nom: "Hôpitale", icone: "hospital-box" },
    { id: 9, nom: "Aéroport", icone: "airplane" }
  ],
  endroits: [
    {
      id: 1,
      nom: "Place Jemaa el-Fna",
      description: "Au XIIe siècle, la place Jemaa el-Fna était un lieu de justice où les peines étaient publiquement appliquées. Dès la seconde moitié du XVIe siècle, les activités et fonctions de la place ont évolué en un lieu cosmopolite vibrant.",
      numero: "+212 524 380000",
      statut: 1,
      statutLibelle: "Ouvert",
      latitude: 31.6258,
      longitude: -7.9891,
      typeCategorie: "Place Publique",
      imageUrl: "https://images.unsplash.com/photo-1548013146-72479768bada?w=900&auto=format&fit=crop&q=80",
      audioUrl: null,
      noteMoyenne: 4.8
    },
    {
      id: 2,
      nom: "Médina de Marrakech",
      description: "Fondée en 1070 par les Almoravides, labyrinthe de souks parfumés, ruelles animées et riads historiques classés à l'UNESCO.",
      numero: "+212 524 384455",
      statut: 1,
      statutLibelle: "Ouvert",
      latitude: 31.6295,
      longitude: -7.9811,
      typeCategorie: "Place Publique",
      imageUrl: "https://images.unsplash.com/photo-1539020140153-e479b8c22e70?w=900&auto=format&fit=crop&q=80",
      noteMoyenne: 4.7
    },
    {
      id: 3,
      nom: "Palais El Badi",
      description: "Construit à la fin du XVIe siècle par Ahmed al-Mansur pour célébrer sa victoire, avec de vastes bassins et jardins d'orangers.",
      numero: "+212 524 378166",
      statut: 1,
      statutLibelle: "Ouvert",
      latitude: 31.6183,
      longitude: -7.9859,
      typeCategorie: "Monument",
      imageUrl: "https://images.unsplash.com/photo-1597212618440-806262de4f6b?w=900&auto=format&fit=crop&q=80",
      noteMoyenne: 4.6
    },
    {
      id: 4,
      nom: "Les tombeaux saadiens",
      description: "Nécropole dynastique découverte en 1917, ornée de marbre de Carrare et de stucs en bois de cèdre ciselé.",
      numero: "+212 524 444000",
      statut: 1,
      statutLibelle: "Ouvert",
      latitude: 31.6172,
      longitude: -7.9889,
      typeCategorie: "Monument",
      imageUrl: "https://images.unsplash.com/photo-1580618672591-eb180b1a973f?w=900&auto=format&fit=crop&q=80",
      noteMoyenne: 4.7
    },
    {
      id: 11,
      nom: "Jardin Majorelle",
      description: "Jardin botanique exotique et féerique au célèbre bleu outremer, restauré par Yves Saint Laurent et Pierre Bergé.",
      numero: "+212 524 313047",
      statut: 1,
      statutLibelle: "Ouvert",
      latitude: 31.6417,
      longitude: -8.0033,
      typeCategorie: "Jardin",
      imageUrl: "https://images.unsplash.com/photo-1589182373726-e4f658ab50f0?w=900&auto=format&fit=crop&q=80",
      noteMoyenne: 4.9
    },
    {
      id: 10,
      nom: "Diam's café",
      description: "Café lounge et terrasse ensoleillée à Guéliz pour déguster thé marocain à la menthe et pâtisseries fines.",
      numero: "+212 524 421200",
      statut: 1,
      statutLibelle: "Ouvert",
      latitude: 31.6310,
      longitude: -8.0090,
      typeCategorie: "Café",
      imageUrl: "https://images.unsplash.com/photo-1554118811-1e0d58224f24?w=900&auto=format&fit=crop&q=80",
      noteMoyenne: 4.4
    }
  ],
  parcours: [
    {
      id: 1,
      nom: "Circuit Trésors Historiques de la Médina",
      description: "Itinéraire traversant les joyaux séculaires, des Tombeaux Saadiens jusqu'à l'effervescente Place Jemaa el-Fna.",
      duree: "3h30",
      distance: "4.2 km",
      imageUrl: "https://images.unsplash.com/photo-1539020140153-e479b8c22e70?w=900&auto=format&fit=crop&q=80",
      etapes: [
        { rang: 1, nom: "Les tombeaux saadiens" },
        { rang: 2, nom: "Palais El Badi" },
        { rang: 3, nom: "Place Jemaa el-Fna" }
      ]
    },
    {
      id: 2,
      nom: "Flânerie Majorelle et Art Moderne à Guéliz",
      description: "Balade botanique et culturelle entre le Jardin Majorelle, les concept-stores et terrasses de Guéliz.",
      duree: "2h15",
      distance: "3.1 km",
      imageUrl: "https://images.unsplash.com/photo-1589182373726-e4f658ab50f0?w=900&auto=format&fit=crop&q=80",
      etapes: [
        { rang: 1, nom: "Jardin Majorelle" },
        { rang: 2, nom: "Diam's café" }
      ]
    }
  ],
  evenements: [
    {
      id: 1,
      nom: "Festival International du Film de Marrakech",
      description: "Projection de films du monde entier sur écran géant à ciel ouvert sur la place Jemaa el-Fna.",
      dateDebut: "2026-11-20",
      dateFin: "2026-11-28",
      contacte: "+212 524 398877",
      lieuNom: "Place Jemaa el-Fna",
      imageUrl: "https://images.unsplash.com/photo-1517604931442-7e0c8ed2963c?w=900&auto=format&fit=crop&q=80"
    },
    {
      id: 2,
      nom: "Festival National des Arts Populaires (FNAP)",
      description: "Danses et musiques folkloriques de toutes les régions du Maroc au cœur du Palais El Badi.",
      dateDebut: "2026-12-05",
      dateFin: "2026-12-10",
      contacte: "+212 524 380123",
      lieuNom: "Palais El Badi",
      imageUrl: "https://images.unsplash.com/photo-1492684223066-81342ee5ff30?w=900&auto=format&fit=crop&q=80"
    }
  ]
};

export const apiService = {
  // Synchronisation avec le backend ASP.NET Core MVC (/api/sync/all)
  async syncAll(apiBase = DEFAULT_API_BASE) {
    const candidates = [
      apiBase,
      "http://localhost:5240",
      "http://10.0.2.2:5240",
      "http://172.20.10.2:5240"
    ];

    // Supprimer les doublons
    const uniqueCandidates = [...new Set(candidates)];

    for (const host of uniqueCandidates) {
      try {
        const controller = new AbortController();
        const timeoutId = setTimeout(() => controller.abort(), 2000);

        const response = await fetch(`${host}/api/sync/all`, {
          signal: controller.signal
        });
        clearTimeout(timeoutId);

        if (response.ok) {
          const data = await response.json();
          await AsyncStorage.setItem(CACHE_KEY, JSON.stringify(data));
          console.log(`Connecté avec succès au backend ASP.NET : ${host}`);
          return { success: true, data, isOnline: true, connectedHost: host };
        }
      } catch (e) {
        // Essayer le candidat suivant
      }
    }

    // Récupération depuis le cache local si hors-ligne
    try {
      const cached = await AsyncStorage.getItem(CACHE_KEY);
      if (cached) {
        return { success: true, data: JSON.parse(cached), isOnline: false };
      }
    } catch (e) {
      // Ignorer
    }

    return { success: true, data: FALLBACK_DATA, isOnline: false };
  },

  // Gestion des Favoris
  async getFavoris() {
    try {
      const favs = await AsyncStorage.getItem(FAVORIS_KEY);
      return favs ? JSON.parse(favs) : [1]; // Place Jemaa el-Fna par défaut
    } catch {
      return [1];
    }
  },

  async toggleFavori(id) {
    try {
      const favs = await this.getFavoris();
      let updated;
      if (favs.includes(id)) {
        updated = favs.filter(f => f !== id);
      } else {
        updated = [...favs, id];
      }
      await AsyncStorage.setItem(FAVORIS_KEY, JSON.stringify(updated));
      return updated;
    } catch {
      return [];
    }
  },

  // Inscription aux événements
  async getParticipations() {
    try {
      const parts = await AsyncStorage.getItem(PARTICIPATIONS_KEY);
      return parts ? JSON.parse(parts) : [];
    } catch {
      return [];
    }
  },

  async toggleParticipation(eventId) {
    try {
      const parts = await this.getParticipations();
      let updated;
      if (parts.includes(eventId)) {
        updated = parts.filter(id => id !== eventId);
      } else {
        updated = [...parts, eventId];
      }
      await AsyncStorage.setItem(PARTICIPATIONS_KEY, JSON.stringify(updated));
      return updated;
    } catch {
      return [];
    }
  }
};
