import React, { useState, useEffect } from 'react';
import {
  StyleSheet,
  Text,
  View,
  ScrollView,
  TextInput,
  TouchableOpacity,
  Image,
  FlatList,
  Modal,
  SafeAreaView,
  StatusBar,
  Alert,
  Linking
} from 'react-native';
import { MaterialCommunityIcons } from '@expo/vector-icons';
import { translations } from './src/translations';
import { apiService, DEFAULT_API_BASE } from './src/services/api';
import FreeOpenStreetMap from './src/components/FreeOpenStreetMap';
import { audioService } from './src/services/audioPlayer';

export default function App() {
  const [lang, setLang] = useState('fr');
  const t = translations[lang] || translations.fr;

  const [activeTab, setActiveTab] = useState('mapp'); // 'mapp', 'audio', 'mappme', 'parcours', 'events', 'settings'
  const [viewMode, setViewMode] = useState('list'); // 'list' ou 'map'
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedCategory, setSelectedCategory] = useState('Tous');

  // Données
  const [endroits, setEndroits] = useState([]);
  const [categories, setCategories] = useState([]);
  const [parcours, setParcours] = useState([]);
  const [evenements, setEvenements] = useState([]);
  const [favoris, setFavoris] = useState([]);
  const [participations, setParticipations] = useState([]);
  const [isOnline, setIsOnline] = useState(false);
  const [loading, setLoading] = useState(true);

  // Modales
  const [selectedSite, setSelectedSite] = useState(null);
  const [activeAudioSite, setActiveAudioSite] = useState(null);
  const [isPlayingAudio, setIsPlayingAudio] = useState(false);
  const [audioProgress, setAudioProgress] = useState(0);
  const [audioCurrentTime, setAudioCurrentTime] = useState('0:00');
  const [audioTotalTime, setAudioTotalTime] = useState('0:00');
  const [authModalVisible, setAuthModalVisible] = useState(false);
  const [currentUser, setCurrentUser] = useState({ username: "Touriste_Marrakech", isLogged: true });

  const formatSec = (seconds) => {
    const s = Math.round(seconds || 0);
    const m = Math.floor(s / 60);
    const rem = s % 60;
    return `${m}:${rem < 10 ? '0' : ''}${rem}`;
  };

  const handlePlayAudio = (site) => {
    setActiveAudioSite(site);
    setIsPlayingAudio(true);
    setAudioProgress(0);
    audioService.playSite(
      site,
      lang,
      (prog, cur, dur) => {
        setAudioProgress(prog);
        setAudioCurrentTime(formatSec(cur));
        setAudioTotalTime(formatSec(dur));
      },
      () => {
        setIsPlayingAudio(false);
      }
    );
  };

  const handleToggleAudio = () => {
    const playing = audioService.togglePause();
    setIsPlayingAudio(playing);
  };

  const handleStopAudio = () => {
    audioService.stop();
    setActiveAudioSite(null);
    setIsPlayingAudio(false);
    setAudioProgress(0);
  };

  // Coordonnées simulées de l'utilisateur (Centre de Marrakech)
  const userLat = 31.6295;
  const userLng = -7.9811;

  // Initialisation et synchronisation
  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    setLoading(true);
    const result = await apiService.syncAll();
    setIsOnline(result.isOnline);
    if (result.data) {
      setEndroits(result.data.endroits || []);
      setParcours(result.data.parcours || []);
      setEvenements(result.data.evenements || []);
      setCategories(result.data.categories || []);
    }
    const favs = await apiService.getFavoris();
    setFavoris(favs);
    const parts = await apiService.getParticipations();
    setParticipations(parts);
    setLoading(false);
  };

  const handleToggleFavori = async (id) => {
    const updated = await apiService.toggleFavori(id);
    setFavoris(updated);
    const isFav = updated.includes(id);
    Alert.alert(
      isFav ? t.favoriteAdded : t.favoriteRemoved,
      isFav ? "Ce site a été ajouté à vos favoris." : "Ce site a été retiré de vos favoris."
    );
  };

  const handleToggleParticipation = async (eventId, eventName) => {
    const updated = await apiService.toggleParticipation(eventId);
    setParticipations(updated);
    const isPart = updated.includes(eventId);
    Alert.alert(
      isPart ? "Inscription confirmée !" : "Participation annulée",
      isPart ? `Rappel enregistré pour : ${eventName}` : `Vous ne participez plus à cet événement.`
    );
  };

  const calculateDistance = (lat, lon) => {
    const R = 6371; // km
    const dLat = (lat - userLat) * Math.PI / 180;
    const dLon = (lon - userLng) * Math.PI / 180;
    const a =
      Math.sin(dLat / 2) * Math.sin(dLat / 2) +
      Math.cos(userLat * Math.PI / 180) * Math.cos(lat * Math.PI / 180) *
      Math.sin(dLon / 2) * Math.sin(dLon / 2);
    const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
    return (R * c).toFixed(1);
  };

  // Filtrage des endroits
  const filteredEndroits = endroits.filter(e => {
    const matchCat = selectedCategory === 'Tous' || e.typeCategorie === selectedCategory;
    const matchSearch = !searchQuery || e.nom.toLowerCase().includes(searchQuery.toLowerCase()) ||
      (e.description && e.description.toLowerCase().includes(searchQuery.toLowerCase()));
    return matchCat && matchSearch;
  });

  return (
    <SafeAreaView style={styles.safeArea}>
      <StatusBar barStyle="light-content" backgroundColor="#0099cc" />

      {/* TOP HEADER */}
      <View style={styles.topHeader}>
        <View style={styles.headerLeft}>
          <MaterialCommunityIcons name="compass-outline" size={26} color="#ffffff" />
          <View>
            <Text style={styles.headerTitle}>Marrakech Audio-Guide</Text>
            <View style={styles.headerSubtitleRow}>
              <View style={[styles.statusDot, { backgroundColor: isOnline ? '#4ade80' : '#facc15' }]} />
              <Text style={styles.headerSubtitle}>
                {isOnline ? "En ligne (Connecté API)" : "Mode Hors-ligne (SQLite Cache)"}
              </Text>
            </View>
          </View>
        </View>

        <View style={styles.headerRight}>
          <TouchableOpacity
            style={styles.headerButton}
            onPress={() => {
              const next = lang === 'fr' ? 'en' : lang === 'en' ? 'es' : 'fr';
              setLang(next);
            }}
          >
            <Text style={styles.langText}>{lang.toUpperCase()}</Text>
          </TouchableOpacity>

          <TouchableOpacity style={styles.headerButton} onPress={loadData}>
            <MaterialCommunityIcons name="sync" size={20} color="#ffffff" />
          </TouchableOpacity>
        </View>
      </View>

      {/* CONTENU PRINCIPAL PAR ONGLET */}
      <View style={styles.mainContainer}>
        {/* ONGLET 1 : MAPP (Accueil & Découverte) */}
        {activeTab === 'mapp' && (
          <View style={styles.tabContent}>
            {/* Barre de Recherche */}
            <View style={styles.searchBarContainer}>
              <MaterialCommunityIcons name="magnify" size={22} color="#94a3b8" />
              <TextInput
                style={styles.searchInput}
                placeholder={t.searchPlaceholder}
                placeholderTextColor="#94a3b8"
                value={searchQuery}
                onChangeText={setSearchQuery}
              />
              {searchQuery.length > 0 && (
                <TouchableOpacity onPress={() => setSearchQuery('')}>
                  <MaterialCommunityIcons name="close-circle" size={18} color="#94a3b8" />
                </TouchableOpacity>
              )}
            </View>

            {/* Barre de Catégories Défilante (Figure 20 du cahier des charges) */}
            <View style={styles.categoriesContainer}>
              <ScrollView horizontal showsHorizontalScrollIndicator={false} contentContainerStyle={styles.categoriesScroll}>
                {categories.map((cat) => {
                  const isSelected = selectedCategory === cat.nom;
                  return (
                    <TouchableOpacity
                      key={cat.id}
                      style={[styles.categoryChip, isSelected && styles.categoryChipActive]}
                      onPress={() => setSelectedCategory(cat.nom)}
                    >
                      <MaterialCommunityIcons
                        name={cat.icone || "map-marker"}
                        size={16}
                        color={isSelected ? "#ffffff" : "#0099cc"}
                      />
                      <Text style={[styles.categoryChipText, isSelected && styles.categoryChipTextActive]}>
                        {cat.nom}
                      </Text>
                    </TouchableOpacity>
                  );
                })}
              </ScrollView>
            </View>

            {/* Bascule Vue Liste vs Vue Carte Gratuite OpenStreetMap */}
            <View style={styles.viewModeToggleContainer}>
              <TouchableOpacity
                style={[styles.viewModeBtn, viewMode === 'list' && styles.viewModeBtnActive]}
                onPress={() => setViewMode('list')}
              >
                <MaterialCommunityIcons name="format-list-bulleted" size={16} color={viewMode === 'list' ? "#ffffff" : "#0099cc"} />
                <Text style={[styles.viewModeText, viewMode === 'list' && styles.viewModeTextActive]}>
                  Liste ({filteredEndroits.length})
                </Text>
              </TouchableOpacity>

              <TouchableOpacity
                style={[styles.viewModeBtn, viewMode === 'map' && styles.viewModeBtnActive]}
                onPress={() => setViewMode('map')}
              >
                <MaterialCommunityIcons name="map" size={16} color={viewMode === 'map' ? "#ffffff" : "#0099cc"} />
                <Text style={[styles.viewModeText, viewMode === 'map' && styles.viewModeTextActive]}>
                  Carte OpenStreetMap
                </Text>
              </TouchableOpacity>
            </View>

            {/* Affichage selon le mode choisi */}
            {viewMode === 'map' ? (
              <ScrollView contentContainerStyle={styles.listPadding}>
                <Text style={styles.sectionTitle}>Carte Interactive de Marrakech (OpenStreetMap)</Text>
                <Text style={styles.mapInfoDesc}>
                  Carte mondiale ouverte et 100% gratuite. Zoomez, déplacez-vous et découvrez les monuments.
                </Text>

                <FreeOpenStreetMap
                  latitude={filteredEndroits.length > 0 ? filteredEndroits[0].latitude : 31.6258}
                  longitude={filteredEndroits.length > 0 ? filteredEndroits[0].longitude : -7.9891}
                  siteName="Marrakech"
                  height={340}
                />

                <Text style={[styles.sectionTitle, { marginTop: 12 }]}>Monuments sur la carte</Text>
                {filteredEndroits.map((site) => (
                  <TouchableOpacity
                    key={site.id}
                    style={styles.mapSiteRow}
                    onPress={() => setSelectedSite(site)}
                  >
                    <MaterialCommunityIcons name="map-marker" size={20} color="#0099cc" />
                    <View style={{ flex: 1, marginLeft: 8 }}>
                      <Text style={styles.mapSiteName}>{site.nom}</Text>
                      <Text style={styles.mapSiteType}>{site.typeCategorie} • {site.latitude}, {site.longitude}</Text>
                    </View>
                    <MaterialCommunityIcons name="chevron-right" size={20} color="#94a3b8" />
                  </TouchableOpacity>
                ))}
              </ScrollView>
            ) : (
            /* Liste des Sites */
            <FlatList
              data={filteredEndroits}
              keyExtractor={(item) => item.id.toString()}
              contentContainerStyle={styles.listPadding}
              renderItem={({ item }) => {
                const isFav = favoris.includes(item.id);
                const dist = calculateDistance(item.latitude, item.longitude);
                return (
                  <TouchableOpacity
                    style={styles.cardContainer}
                    activeOpacity={0.9}
                    onPress={() => setSelectedSite(item)}
                  >
                    <Image source={{ uri: item.imageUrl || "https://images.unsplash.com/photo-1548013146-72479768bada?w=800" }} style={styles.cardImage} />

                    <TouchableOpacity
                      style={styles.favBadge}
                      onPress={() => handleToggleFavori(item.id)}
                    >
                      <MaterialCommunityIcons
                        name={isFav ? "heart" : "heart-outline"}
                        size={22}
                        color={isFav ? "#ef4444" : "#ffffff"}
                      />
                    </TouchableOpacity>

                    <View style={styles.cardBody}>
                      <View style={styles.cardRow}>
                        <Text style={styles.cardTitle}>{item.nom}</Text>
                        <View style={styles.ratingBadge}>
                          <MaterialCommunityIcons name="star" size={14} color="#f59e0b" />
                          <Text style={styles.ratingText}>{item.noteMoyenne || 4.5}</Text>
                        </View>
                      </View>

                      <View style={styles.cardMetaRow}>
                        <View style={styles.tag}>
                          <Text style={styles.tagText}>{item.typeCategorie}</Text>
                        </View>
                        <Text style={styles.distanceText}>
                          <MaterialCommunityIcons name="map-marker-distance" size={13} color="#0099cc" /> {dist} km
                        </Text>
                        <Text style={[styles.statusText, { color: item.statut === 1 ? '#16a34a' : '#d97706' }]}>
                          ● {item.statut === 1 ? t.open : t.closed}
                        </Text>
                      </View>

                      <Text style={styles.cardDesc} numberOfLines={2}>
                        {item.description}
                      </Text>

                      {/* Actions rapides */}
                      <View style={styles.cardActions}>
                        <TouchableOpacity
                          style={styles.btnActionSecondary}
                          onPress={() => handlePlayAudio(item)}
                        >
                          <MaterialCommunityIcons name="headphones" size={16} color="#0099cc" />
                          <Text style={styles.btnActionSecondaryText}>{t.tabAudio}</Text>
                        </TouchableOpacity>

                        {item.numero && (
                          <TouchableOpacity
                            style={styles.btnActionIcon}
                            onPress={() => Linking.openURL(`tel:${item.numero}`)}
                          >
                            <MaterialCommunityIcons name="phone" size={16} color="#0284c7" />
                          </TouchableOpacity>
                        )}

                        <TouchableOpacity
                          style={styles.btnActionPrimary}
                          onPress={() => setSelectedSite(item)}
                        >
                          <Text style={styles.btnActionPrimaryText}>{t.details}</Text>
                          <MaterialCommunityIcons name="arrow-right" size={14} color="#ffffff" />
                        </TouchableOpacity>
                      </View>
                    </View>
                  </TouchableOpacity>
                );
              }}
            />
            )}
          </View>
        )}

        {/* ONGLET 2 : AUDIO-GUIDE */}
        {activeTab === 'audio' && (
          <ScrollView style={styles.tabContent} contentContainerStyle={styles.listPadding}>
            <View style={styles.bannerContainer}>
              <MaterialCommunityIcons name="podcast" size={32} color="#0099cc" />
              <View style={{ flex: 1, marginLeft: 12 }}>
                <Text style={styles.bannerTitle}>Audio-Guide de Marrakech</Text>
                <Text style={styles.bannerSubtitle}>
                  Écoutez les récits historiques et anecdotes captivantes à chaque pas.
                </Text>
              </View>
            </View>

            <Text style={styles.sectionTitle}>Lieux Incontournables & Récits Vocaux</Text>

            {endroits.map((site) => (
              <TouchableOpacity
                key={site.id}
                style={styles.audioRow}
                onPress={() => handlePlayAudio(site)}
              >
                <Image source={{ uri: site.imageUrl }} style={styles.audioThumbnail} />
                <View style={{ flex: 1, marginLeft: 12 }}>
                  <Text style={styles.audioSiteTitle}>{site.nom}</Text>
                  <Text style={styles.audioSiteSubtitle}>{site.typeCategorie} • Guide Vocal ({lang.toUpperCase()})</Text>
                  <Text style={styles.audioSnippet} numberOfLines={1}>{site.description}</Text>
                </View>
                <View style={styles.audioPlayBtn}>
                  <MaterialCommunityIcons
                    name={activeAudioSite && activeAudioSite.id === site.id && isPlayingAudio ? "pause" : "play"}
                    size={20}
                    color="#ffffff"
                  />
                </View>
              </TouchableOpacity>
            ))}
          </ScrollView>
        )}

        {/* ONGLET 3 : MAPP ME (Géolocalisation & Proximité) */}
        {activeTab === 'mappme' && (
          <ScrollView style={styles.tabContent} contentContainerStyle={styles.listPadding}>
            <View style={styles.radarCard}>
              <MaterialCommunityIcons name="crosshairs-gps" size={40} color="#0099cc" />
              <Text style={styles.radarTitle}>Fonctionnalité Mapp Me</Text>
              <Text style={styles.radarSubtitle}>
                Position actuelle : <Text style={{ fontWeight: 'bold' }}>Médina de Marrakech</Text> (Lat: {userLat}, Lng: {userLng})
              </Text>
              <View style={styles.radarBadge}>
                <Text style={styles.radarBadgeText}>Rayon de découverte : 100 km</Text>
              </View>
            </View>

            <Text style={styles.sectionTitle}>Sites les plus proches de vous</Text>

            {endroits
              .map(e => ({ ...e, distanceKm: calculateDistance(e.latitude, e.longitude) }))
              .sort((a, b) => parseFloat(a.distanceKm) - parseFloat(b.distanceKm))
              .map((item) => (
                <TouchableOpacity
                  key={item.id}
                  style={styles.proximityRow}
                  onPress={() => setSelectedSite(item)}
                >
                  <View style={styles.distanceBadge}>
                    <Text style={styles.distNumber}>{item.distanceKm}</Text>
                    <Text style={styles.distUnit}>km</Text>
                  </View>
                  <View style={{ flex: 1, marginLeft: 14 }}>
                    <Text style={styles.cardTitle}>{item.nom}</Text>
                    <Text style={styles.proximityDesc} numberOfLines={1}>{item.description}</Text>
                  </View>
                  <MaterialCommunityIcons name="chevron-right" size={24} color="#94a3b8" />
                </TouchableOpacity>
              ))}
          </ScrollView>
        )}

        {/* ONGLET 4 : PARCOURS */}
        {activeTab === 'parcours' && (
          <ScrollView style={styles.tabContent} contentContainerStyle={styles.listPadding}>
            <Text style={styles.sectionTitle}>Circuits & Parcours Recommandés</Text>

            {parcours.map((p) => (
              <View key={p.id} style={styles.parcoursCard}>
                <Image source={{ uri: p.imageUrl }} style={styles.parcoursImage} />
                <View style={styles.parcoursBody}>
                  <Text style={styles.cardTitle}>{p.nom}</Text>
                  <View style={styles.parcoursMeta}>
                    <Text style={styles.parcoursMetaText}><MaterialCommunityIcons name="clock-outline" size={14} /> {p.duree}</Text>
                    <Text style={styles.parcoursMetaText}><MaterialCommunityIcons name="map-marker-distance" size={14} /> {p.distance}</Text>
                    <Text style={styles.parcoursMetaText}><MaterialCommunityIcons name="format-list-numbered" size={14} /> {p.etapes ? p.etapes.length : 3} {t.stages}</Text>
                  </View>
                  <Text style={styles.cardDesc}>{p.description}</Text>

                  {/* Étapes du parcours */}
                  <View style={styles.etapesBox}>
                    <Text style={styles.etapesTitle}>Étapes ordonnées de l'itinéraire :</Text>
                    {p.etapes && p.etapes.map((et) => (
                      <View key={et.rang} style={styles.etapeItem}>
                        <View style={styles.etapeIndex}>
                          <Text style={styles.etapeIndexText}>{et.rang}</Text>
                        </View>
                        <Text style={styles.etapeName}>{et.nom}</Text>
                      </View>
                    ))}
                  </View>
                </View>
              </View>
            ))}
          </ScrollView>
        )}

        {/* ONGLET 5 : ÉVÉNEMENTS */}
        {activeTab === 'events' && (
          <ScrollView style={styles.tabContent} contentContainerStyle={styles.listPadding}>
            <Text style={styles.sectionTitle}>Agenda Culturel & Manifestations</Text>

            {evenements.map((ev) => {
              const isParticipating = participations.includes(ev.id);
              return (
                <View key={ev.id} style={styles.eventCard}>
                  <Image source={{ uri: ev.imageUrl }} style={styles.eventImage} />
                  <View style={styles.eventBody}>
                    <View style={styles.cardRow}>
                      <Text style={[styles.cardTitle, { flex: 1 }]}>{ev.nom}</Text>
                      <View style={styles.eventDateBadge}>
                        <Text style={styles.eventDateText}>{ev.dateDebut?.substring(5, 10)}</Text>
                      </View>
                    </View>

                    <Text style={styles.eventLocation}>
                      <MaterialCommunityIcons name="map-marker" size={14} color="#ef4444" /> {ev.lieuNom || "Marrakech"}
                    </Text>

                    <Text style={styles.cardDesc}>{ev.description}</Text>

                    <TouchableOpacity
                      style={[styles.btnParticipate, isParticipating && styles.btnParticipateActive]}
                      onPress={() => handleToggleParticipation(ev.id, ev.nom)}
                    >
                      <MaterialCommunityIcons
                        name={isParticipating ? "check-circle" : "calendar-plus"}
                        size={18}
                        color="#ffffff"
                      />
                      <Text style={styles.btnParticipateText}>
                        {isParticipating ? t.participating : t.participate}
                      </Text>
                    </TouchableOpacity>
                  </View>
                </View>
              );
            })}
          </ScrollView>
        )}

        {/* ONGLET 6 : PARAMÈTRES */}
        {activeTab === 'settings' && (
          <ScrollView style={styles.tabContent} contentContainerStyle={styles.listPadding}>
            <Text style={styles.sectionTitle}>{t.settingsTitle}</Text>

            {/* Choix de langue (Tableau 2 du PFE) */}
            <View style={styles.settingCard}>
              <Text style={styles.settingHeader}>
                <MaterialCommunityIcons name="translate" size={18} color="#0099cc" /> {t.language}
              </Text>
              <View style={styles.langSelectorRow}>
                {['fr', 'en', 'es'].map((code) => (
                  <TouchableOpacity
                    key={code}
                    style={[styles.langOption, lang === code && styles.langOptionActive]}
                    onPress={() => setLang(code)}
                  >
                    <Text style={[styles.langOptionText, lang === code && styles.langOptionTextActive]}>
                      {code === 'fr' ? 'Français' : code === 'en' ? 'English' : 'Español'}
                    </Text>
                  </TouchableOpacity>
                ))}
              </View>
            </View>

            {/* Filtres de notifications (Tableau 4 du PFE) */}
            <View style={styles.settingCard}>
              <Text style={styles.settingHeader}>
                <MaterialCommunityIcons name="bell-ring-outline" size={18} color="#0099cc" /> {t.notifications}
              </Text>
              {['Hébergement', 'Café & Restaurant', 'Monuments', 'Shopping', 'Santé', 'Transport'].map((cat, i) => (
                <View key={i} style={styles.notificationRow}>
                  <Text style={styles.notificationLabel}>{cat}</Text>
                  <MaterialCommunityIcons name="toggle-switch" size={32} color="#0099cc" />
                </View>
              ))}
            </View>

            {/* À Propos (Tableau 4 du PFE) */}
            <View style={styles.settingCard}>
              <Text style={styles.settingHeader}>
                <MaterialCommunityIcons name="information-outline" size={18} color="#0099cc" /> {t.about}
              </Text>
              <Text style={styles.aboutText}>{t.aboutText}</Text>
              <Text style={styles.authorText}>Auteurs : Mohamed Dicko & Khardi Sanae</Text>
              <Text style={styles.authorText}>Encadrement : M. Anass El Hamdi & M. Mounir Omari</Text>
              <Text style={styles.authorText}>Version 2.0 (React Native + ASP.NET Core)</Text>
            </View>
          </ScrollView>
        )}
      </View>

      {/* MODALE DÉTAILS D'UN SITE */}
      {selectedSite && (
        <Modal visible={true} animationType="slide" transparent={false}>
          <SafeAreaView style={styles.modalSafeArea}>
            <ScrollView contentContainerStyle={styles.modalContent}>
              <Image source={{ uri: selectedSite.imageUrl }} style={styles.modalHeroImage} />

              <TouchableOpacity style={styles.modalCloseBtn} onPress={() => setSelectedSite(null)}>
                <MaterialCommunityIcons name="close" size={24} color="#ffffff" />
              </TouchableOpacity>

              <View style={styles.modalBody}>
                <View style={styles.cardRow}>
                  <Text style={styles.modalTitle}>{selectedSite.nom}</Text>
                  <TouchableOpacity onPress={() => handleToggleFavori(selectedSite.id)}>
                    <MaterialCommunityIcons
                      name={favoris.includes(selectedSite.id) ? "heart" : "heart-outline"}
                      size={28}
                      color={favoris.includes(selectedSite.id) ? "#ef4444" : "#64748b"}
                    />
                  </TouchableOpacity>
                </View>

                <View style={styles.cardMetaRow}>
                  <View style={styles.tag}><Text style={styles.tagText}>{selectedSite.typeCategorie}</Text></View>
                  <Text style={[styles.statusText, { color: selectedSite.statut === 1 ? '#16a34a' : '#d97706' }]}>
                    ● {selectedSite.statut === 1 ? t.open : t.closed}
                  </Text>
                </View>

                {/* Carte 100% Gratuite OpenStreetMap dans la modale */}
                <Text style={styles.modalSectionTitle}>Emplacement sur la carte (OpenStreetMap Gratuit)</Text>
                <FreeOpenStreetMap
                  latitude={selectedSite.latitude}
                  longitude={selectedSite.longitude}
                  siteName={selectedSite.nom}
                  height={240}
                />

                <Text style={styles.modalSectionTitle}>Description</Text>
                <Text style={styles.modalDesc}>{selectedSite.description}</Text>

                {/* Bouton Audio-guide */}
                <TouchableOpacity
                  style={styles.modalAudioButton}
                  onPress={() => handlePlayAudio(selectedSite)}
                >
                  <MaterialCommunityIcons name="headphones" size={22} color="#ffffff" />
                  <Text style={styles.modalAudioButtonText}>{t.listenAudio}</Text>
                </TouchableOpacity>

                {/* Bouton Téléphone */}
                {selectedSite.numero && (
                  <TouchableOpacity
                    style={styles.modalCallButton}
                    onPress={() => Linking.openURL(`tel:${selectedSite.numero}`)}
                  >
                    <MaterialCommunityIcons name="phone" size={20} color="#0099cc" />
                    <Text style={styles.modalCallButtonText}>{t.call} ({selectedSite.numero})</Text>
                  </TouchableOpacity>
                )}
              </View>
            </ScrollView>
          </SafeAreaView>
        </Modal>
      )}

      {/* LECTEUR AUDIO-GUIDE FLOTTANT (Figure 20 du PFE) */}
      {activeAudioSite && (
        <View style={styles.miniAudioPlayer}>
          <TouchableOpacity onPress={handleToggleAudio}>
            <MaterialCommunityIcons
              name={isPlayingAudio ? "pause-circle" : "play-circle"}
              size={44}
              color="#0099cc"
            />
          </TouchableOpacity>

          <View style={styles.miniAudioInfo}>
            <Text style={styles.miniAudioTitle} numberOfLines={1}>{activeAudioSite.nom}</Text>
            <View style={styles.progressBar}>
              <View style={[styles.progressFill, { width: `${audioProgress * 100}%` }]} />
            </View>
            <Text style={styles.miniAudioTime}>
              {audioCurrentTime} / {audioTotalTime} • Guide Vocal ({lang.toUpperCase()})
            </Text>
          </View>

          <TouchableOpacity onPress={handleStopAudio}>
            <MaterialCommunityIcons name="close" size={24} color="#64748b" />
          </TouchableOpacity>
        </View>
      )}

      {/* BOTTOM TAB NAVIGATION (Mapp, Audio, Mapp Me, Parcours, Événements, Paramètres) */}
      <View style={styles.bottomTabBar}>
        <TouchableOpacity
          style={styles.tabItem}
          onPress={() => setActiveTab('mapp')}
        >
          <MaterialCommunityIcons
            name="map-outline"
            size={22}
            color={activeTab === 'mapp' ? "#0099cc" : "#94a3b8"}
          />
          <Text style={[styles.tabLabel, activeTab === 'mapp' && styles.tabLabelActive]}>
            {t.tabMap}
          </Text>
        </TouchableOpacity>

        <TouchableOpacity
          style={styles.tabItem}
          onPress={() => setActiveTab('audio')}
        >
          <MaterialCommunityIcons
            name="headphones"
            size={22}
            color={activeTab === 'audio' ? "#0099cc" : "#94a3b8"}
          />
          <Text style={[styles.tabLabel, activeTab === 'audio' && styles.tabLabelActive]}>
            {t.tabAudio}
          </Text>
        </TouchableOpacity>

        <TouchableOpacity
          style={styles.tabItem}
          onPress={() => setActiveTab('mappme')}
        >
          <MaterialCommunityIcons
            name="crosshairs-gps"
            size={22}
            color={activeTab === 'mappme' ? "#0099cc" : "#94a3b8"}
          />
          <Text style={[styles.tabLabel, activeTab === 'mappme' && styles.tabLabelActive]}>
            {t.tabMappMe}
          </Text>
        </TouchableOpacity>

        <TouchableOpacity
          style={styles.tabItem}
          onPress={() => setActiveTab('parcours')}
        >
          <MaterialCommunityIcons
            name="signpost-split"
            size={22}
            color={activeTab === 'parcours' ? "#0099cc" : "#94a3b8"}
          />
          <Text style={[styles.tabLabel, activeTab === 'parcours' && styles.tabLabelActive]}>
            {t.tabParcours}
          </Text>
        </TouchableOpacity>

        <TouchableOpacity
          style={styles.tabItem}
          onPress={() => setActiveTab('events')}
        >
          <MaterialCommunityIcons
            name="calendar-month"
            size={22}
            color={activeTab === 'events' ? "#0099cc" : "#94a3b8"}
          />
          <Text style={[styles.tabLabel, activeTab === 'events' && styles.tabLabelActive]}>
            {t.tabEvents}
          </Text>
        </TouchableOpacity>

        <TouchableOpacity
          style={styles.tabItem}
          onPress={() => setActiveTab('settings')}
        >
          <MaterialCommunityIcons
            name="cog-outline"
            size={22}
            color={activeTab === 'settings' ? "#0099cc" : "#94a3b8"}
          />
          <Text style={[styles.tabLabel, activeTab === 'settings' && styles.tabLabelActive]}>
            {t.tabSettings}
          </Text>
        </TouchableOpacity>
      </View>
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  safeArea: {
    flex: 1,
    backgroundColor: '#0099cc'
  },
  topHeader: {
    backgroundColor: '#0099cc',
    paddingHorizontal: 16,
    paddingVertical: 12,
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between'
  },
  headerLeft: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: 10
  },
  headerTitle: {
    color: '#ffffff',
    fontSize: 16,
    fontWeight: '700'
  },
  headerSubtitleRow: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: 6
  },
  statusDot: {
    width: 7,
    height: 7,
    borderRadius: 4
  },
  headerSubtitle: {
    color: 'rgba(255,255,255,0.85)',
    fontSize: 11
  },
  headerRight: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: 8
  },
  headerButton: {
    backgroundColor: 'rgba(255,255,255,0.2)',
    paddingHorizontal: 8,
    paddingVertical: 4,
    borderRadius: 6
  },
  langText: {
    color: '#ffffff',
    fontWeight: '700',
    fontSize: 12
  },
  mainContainer: {
    flex: 1,
    backgroundColor: '#f8fafc'
  },
  tabContent: {
    flex: 1
  },
  listPadding: {
    padding: 16,
    paddingBottom: 90
  },
  searchBarContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#ffffff',
    marginHorizontal: 16,
    marginTop: 12,
    marginBottom: 8,
    paddingHorizontal: 12,
    height: 44,
    borderRadius: 22,
    borderWidth: 1,
    borderColor: '#e2e8f0',
    gap: 8
  },
  searchInput: {
    flex: 1,
    color: '#1e293b',
    fontSize: 14
  },
  categoriesContainer: {
    marginBottom: 10
  },
  categoriesScroll: {
    paddingHorizontal: 16,
    gap: 8
  },
  categoryChip: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#ffffff',
    paddingHorizontal: 12,
    paddingVertical: 7,
    borderRadius: 20,
    borderWidth: 1,
    borderColor: '#cbd5e1',
    gap: 6
  },
  categoryChipActive: {
    backgroundColor: '#0099cc',
    borderColor: '#0099cc'
  },
  categoryChipText: {
    color: '#475569',
    fontSize: 12,
    fontWeight: '600'
  },
  categoryChipTextActive: {
    color: '#ffffff'
  },
  cardContainer: {
    backgroundColor: '#ffffff',
    borderRadius: 14,
    overflow: 'hidden',
    marginBottom: 16,
    borderWidth: 1,
    borderColor: '#e2e8f0',
    shadowColor: '#000',
    shadowOpacity: 0.05,
    shadowRadius: 5,
    elevation: 2
  },
  cardImage: {
    width: '100%',
    height: 160
  },
  favBadge: {
    position: 'absolute',
    top: 12,
    right: 12,
    backgroundColor: 'rgba(0,0,0,0.4)',
    padding: 6,
    borderRadius: 20
  },
  cardBody: {
    padding: 14
  },
  cardRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center'
  },
  cardTitle: {
    fontSize: 16,
    fontWeight: '700',
    color: '#0f172a'
  },
  ratingBadge: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#fef3c7',
    paddingHorizontal: 6,
    paddingVertical: 2,
    borderRadius: 6,
    gap: 3
  },
  ratingText: {
    fontSize: 12,
    fontWeight: '700',
    color: '#b45309'
  },
  cardMetaRow: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: 10,
    marginVertical: 6
  },
  tag: {
    backgroundColor: '#e0f2fe',
    paddingHorizontal: 8,
    paddingVertical: 3,
    borderRadius: 4
  },
  tagText: {
    fontSize: 11,
    color: '#0284c7',
    fontWeight: '600'
  },
  distanceText: {
    fontSize: 12,
    color: '#64748b'
  },
  statusText: {
    fontSize: 12,
    fontWeight: '600'
  },
  cardDesc: {
    color: '#64748b',
    fontSize: 13,
    lineHeight: 18,
    marginBottom: 12
  },
  cardActions: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: 8,
    borderTopWidth: 1,
    borderTopColor: '#f1f5f9',
    paddingTop: 10
  },
  btnActionSecondary: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#f0f9ff',
    paddingHorizontal: 10,
    paddingVertical: 6,
    borderRadius: 6,
    gap: 4
  },
  btnActionSecondaryText: {
    color: '#0099cc',
    fontSize: 12,
    fontWeight: '600'
  },
  btnActionIcon: {
    backgroundColor: '#f0f9ff',
    padding: 8,
    borderRadius: 6
  },
  btnActionPrimary: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#0099cc',
    paddingHorizontal: 12,
    paddingVertical: 6,
    borderRadius: 6,
    marginLeft: 'auto',
    gap: 4
  },
  btnActionPrimaryText: {
    color: '#ffffff',
    fontSize: 12,
    fontWeight: '600'
  },
  sectionTitle: {
    fontSize: 18,
    fontWeight: '700',
    color: '#0f172a',
    marginBottom: 12
  },
  bannerContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#e0f2fe',
    padding: 16,
    borderRadius: 12,
    marginBottom: 16
  },
  bannerTitle: {
    fontSize: 16,
    fontWeight: '700',
    color: '#0369a1'
  },
  bannerSubtitle: {
    fontSize: 12,
    color: '#0c4a6e',
    marginTop: 2
  },
  audioRow: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#ffffff',
    padding: 12,
    borderRadius: 10,
    marginBottom: 10,
    borderWidth: 1,
    borderColor: '#e2e8f0'
  },
  audioThumbnail: {
    width: 60,
    height: 60,
    borderRadius: 8
  },
  audioSiteTitle: {
    fontSize: 14,
    fontWeight: '700',
    color: '#0f172a'
  },
  audioSiteSubtitle: {
    fontSize: 11,
    color: '#0099cc',
    marginTop: 2
  },
  audioSnippet: {
    fontSize: 11,
    color: '#64748b',
    marginTop: 2
  },
  audioPlayBtn: {
    backgroundColor: '#0099cc',
    width: 36,
    height: 36,
    borderRadius: 18,
    alignItems: 'center',
    justifyContent: 'center'
  },
  radarCard: {
    backgroundColor: '#ffffff',
    borderRadius: 14,
    padding: 20,
    alignItems: 'center',
    borderWidth: 1,
    borderColor: '#e2e8f0',
    marginBottom: 16
  },
  radarTitle: {
    fontSize: 18,
    fontWeight: '700',
    color: '#0f172a',
    marginTop: 8
  },
  radarSubtitle: {
    fontSize: 12,
    color: '#64748b',
    marginTop: 4,
    textAlign: 'center'
  },
  radarBadge: {
    backgroundColor: '#f0fdf4',
    borderColor: '#bbf7d0',
    borderWidth: 1,
    paddingHorizontal: 12,
    paddingVertical: 4,
    borderRadius: 20,
    marginTop: 10
  },
  radarBadgeText: {
    fontSize: 12,
    color: '#16a34a',
    fontWeight: '600'
  },
  proximityRow: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#ffffff',
    padding: 14,
    borderRadius: 10,
    marginBottom: 10,
    borderWidth: 1,
    borderColor: '#e2e8f0'
  },
  distanceBadge: {
    backgroundColor: '#f0f9ff',
    padding: 8,
    borderRadius: 8,
    alignItems: 'center',
    minWidth: 50
  },
  distNumber: {
    fontSize: 16,
    fontWeight: '700',
    color: '#0099cc'
  },
  distUnit: {
    fontSize: 10,
    color: '#0284c7'
  },
  proximityDesc: {
    fontSize: 12,
    color: '#64748b',
    marginTop: 2
  },
  parcoursCard: {
    backgroundColor: '#ffffff',
    borderRadius: 12,
    overflow: 'hidden',
    marginBottom: 16,
    borderWidth: 1,
    borderColor: '#e2e8f0'
  },
  parcoursImage: {
    width: '100%',
    height: 140
  },
  parcoursBody: {
    padding: 14
  },
  parcoursMeta: {
    flexDirection: 'row',
    gap: 14,
    marginVertical: 6
  },
  parcoursMetaText: {
    fontSize: 12,
    color: '#0284c7',
    fontWeight: '600'
  },
  etapesBox: {
    backgroundColor: '#f8fafc',
    padding: 10,
    borderRadius: 8,
    marginTop: 8
  },
  etapesTitle: {
    fontSize: 12,
    fontWeight: '700',
    color: '#334155',
    marginBottom: 6
  },
  etapeItem: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: 8,
    marginBottom: 4
  },
  etapeIndex: {
    backgroundColor: '#0099cc',
    width: 20,
    height: 20,
    borderRadius: 10,
    alignItems: 'center',
    justifyContent: 'center'
  },
  etapeIndexText: {
    color: '#ffffff',
    fontSize: 10,
    fontWeight: '700'
  },
  etapeName: {
    fontSize: 12,
    color: '#1e293b'
  },
  eventCard: {
    backgroundColor: '#ffffff',
    borderRadius: 12,
    overflow: 'hidden',
    marginBottom: 16,
    borderWidth: 1,
    borderColor: '#e2e8f0'
  },
  eventImage: {
    width: '100%',
    height: 140
  },
  eventBody: {
    padding: 14
  },
  eventDateBadge: {
    backgroundColor: '#fee2e2',
    paddingHorizontal: 8,
    paddingVertical: 2,
    borderRadius: 4
  },
  eventDateText: {
    color: '#b91c1c',
    fontSize: 11,
    fontWeight: '700'
  },
  eventLocation: {
    color: '#64748b',
    fontSize: 12,
    marginVertical: 4
  },
  btnParticipate: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    backgroundColor: '#0099cc',
    paddingVertical: 8,
    borderRadius: 8,
    gap: 6,
    marginTop: 8
  },
  btnParticipateActive: {
    backgroundColor: '#16a34a'
  },
  btnParticipateText: {
    color: '#ffffff',
    fontWeight: '600',
    fontSize: 13
  },
  settingCard: {
    backgroundColor: '#ffffff',
    padding: 16,
    borderRadius: 12,
    borderWidth: 1,
    borderColor: '#e2e8f0',
    marginBottom: 14
  },
  settingHeader: {
    fontSize: 14,
    fontWeight: '700',
    color: '#0f172a',
    marginBottom: 10
  },
  langSelectorRow: {
    flexDirection: 'row',
    gap: 8
  },
  langOption: {
    flex: 1,
    paddingVertical: 8,
    borderRadius: 8,
    borderWidth: 1,
    borderColor: '#cbd5e1',
    alignItems: 'center'
  },
  langOptionActive: {
    backgroundColor: '#0099cc',
    borderColor: '#0099cc'
  },
  langOptionText: {
    color: '#475569',
    fontSize: 12,
    fontWeight: '600'
  },
  langOptionTextActive: {
    color: '#ffffff'
  },
  notificationRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    paddingVertical: 6,
    borderBottomWidth: 1,
    borderBottomColor: '#f1f5f9'
  },
  notificationLabel: {
    fontSize: 13,
    color: '#334155'
  },
  aboutText: {
    fontSize: 12,
    color: '#64748b',
    lineHeight: 18,
    marginBottom: 8
  },
  authorText: {
    fontSize: 11,
    color: '#475569',
    marginBottom: 2
  },
  modalSafeArea: {
    flex: 1,
    backgroundColor: '#ffffff'
  },
  modalContent: {
    paddingBottom: 40
  },
  modalHeroImage: {
    width: '100%',
    height: 240
  },
  modalCloseBtn: {
    position: 'absolute',
    top: 16,
    right: 16,
    backgroundColor: 'rgba(0,0,0,0.5)',
    padding: 8,
    borderRadius: 20
  },
  modalBody: {
    padding: 20
  },
  modalTitle: {
    fontSize: 22,
    fontWeight: '800',
    color: '#0f172a',
    flex: 1
  },
  modalSectionTitle: {
    fontSize: 16,
    fontWeight: '700',
    color: '#0f172a',
    marginTop: 16,
    marginBottom: 6
  },
  modalDesc: {
    fontSize: 14,
    color: '#475569',
    lineHeight: 22
  },
  modalMapPreview: {
    backgroundColor: '#f0f9ff',
    padding: 16,
    borderRadius: 10,
    alignItems: 'center',
    marginTop: 14
  },
  modalMapText: {
    fontSize: 12,
    color: '#0369a1',
    marginTop: 4
  },
  btnOpenMaps: {
    backgroundColor: '#0099cc',
    paddingHorizontal: 14,
    paddingVertical: 6,
    borderRadius: 6,
    marginTop: 8
  },
  btnOpenMapsText: {
    color: '#ffffff',
    fontSize: 12,
    fontWeight: '600'
  },
  modalAudioButton: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    backgroundColor: '#0099cc',
    paddingVertical: 12,
    borderRadius: 10,
    gap: 8,
    marginTop: 20
  },
  modalAudioButtonText: {
    color: '#ffffff',
    fontSize: 15,
    fontWeight: '700'
  },
  modalCallButton: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    backgroundColor: '#f0f9ff',
    paddingVertical: 10,
    borderRadius: 10,
    gap: 8,
    marginTop: 10,
    borderWidth: 1,
    borderColor: '#bae6fd'
  },
  modalCallButtonText: {
    color: '#0099cc',
    fontSize: 14,
    fontWeight: '600'
  },
  miniAudioPlayer: {
    position: 'absolute',
    bottom: 60,
    left: 12,
    right: 12,
    backgroundColor: '#ffffff',
    borderRadius: 12,
    padding: 10,
    flexDirection: 'row',
    alignItems: 'center',
    borderWidth: 1,
    borderColor: '#cbd5e1',
    shadowColor: '#000',
    shadowOpacity: 0.1,
    shadowRadius: 10,
    elevation: 8,
    gap: 10
  },
  miniAudioInfo: {
    flex: 1
  },
  miniAudioTitle: {
    fontSize: 13,
    fontWeight: '700',
    color: '#0f172a'
  },
  progressBar: {
    height: 4,
    backgroundColor: '#e2e8f0',
    borderRadius: 2,
    marginVertical: 4
  },
  progressFill: {
    height: '100%',
    backgroundColor: '#0099cc',
    borderRadius: 2
  },
  miniAudioTime: {
    fontSize: 10,
    color: '#94a3b8'
  },
  bottomTabBar: {
    flexDirection: 'row',
    height: 60,
    backgroundColor: '#ffffff',
    borderTopWidth: 1,
    borderTopColor: '#e2e8f0'
  },
  tabItem: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center'
  },
  tabLabel: {
    fontSize: 10,
    color: '#94a3b8',
    marginTop: 2,
    fontWeight: '500'
  },
  tabLabelActive: {
    color: '#0099cc',
    fontWeight: '700'
  },
  viewModeToggleContainer: {
    flexDirection: 'row',
    marginHorizontal: 16,
    marginBottom: 10,
    backgroundColor: '#e2e8f0',
    borderRadius: 8,
    padding: 3,
    gap: 4
  },
  viewModeBtn: {
    flex: 1,
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: 7,
    borderRadius: 6,
    gap: 6
  },
  viewModeBtnActive: {
    backgroundColor: '#0099cc'
  },
  viewModeText: {
    fontSize: 12,
    fontWeight: '600',
    color: '#475569'
  },
  viewModeTextActive: {
    color: '#ffffff'
  },
  mapInfoDesc: {
    fontSize: 12,
    color: '#64748b',
    marginBottom: 8
  },
  mapSiteRow: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#ffffff',
    padding: 12,
    borderRadius: 8,
    borderWidth: 1,
    borderColor: '#e2e8f0',
    marginBottom: 8
  },
  mapSiteName: {
    fontSize: 13,
    fontWeight: '700',
    color: '#0f172a'
  },
  mapSiteType: {
    fontSize: 11,
    color: '#64748b',
    marginTop: 2
  }
});
