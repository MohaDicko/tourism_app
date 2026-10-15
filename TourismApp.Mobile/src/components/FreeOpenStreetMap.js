import React from 'react';
import { View, StyleSheet, Text, TouchableOpacity, Linking, Platform } from 'react-native';
import { MaterialCommunityIcons } from '@expo/vector-icons';

/**
 * Composant de carte 100% GRATUITE utilisant OpenStreetMap.
 * Aucune clé API, aucune carte bancaire requise, données ouvertes mondiales.
 */
export default function FreeOpenStreetMap({ latitude, longitude, siteName, height = 260, interactive = true }) {
  const lat = latitude || 31.6258;
  const lng = longitude || -7.9891;
  const delta = 0.008;

  // URL officielle d'intégration OpenStreetMap (100% gratuite, libre de droits)
  const osmEmbedUrl = `https://www.openstreetmap.org/export/embed.html?bbox=${lng - delta}%2C${lat - delta}%2C${lng + delta}%2C${lat + delta}&layer=mapnik&marker=${lat}%2C${lng}`;
  const osmFullUrl = `https://www.openstreetmap.org/?mlat=${lat}&mlon=${lng}#map=16/${lat}/${lng}`;

  return (
    <View style={[styles.container, { height }]}>
      {/* Intégration iframe OpenStreetMap (Web et WebView compatible) */}
      {Platform.OS === 'web' ? (
        <iframe
          src={osmEmbedUrl}
          style={{
            width: '100%',
            height: '100%',
            border: 'none',
            borderRadius: 12
          }}
          title={siteName || "Carte OpenStreetMap"}
          loading="lazy"
        />
      ) : (
        <View style={styles.fallbackBox}>
          <MaterialCommunityIcons name="map-marker-radius" size={40} color="#0099cc" />
          <Text style={styles.fallbackTitle}>{siteName || "Emplacement OpenStreetMap"}</Text>
          <Text style={styles.fallbackCoords}>GPS : {lat.toFixed(4)}, {lng.toFixed(4)}</Text>
        </View>
      )}

      {/* Barre d'outils gratuite OpenStreetMap en bas de carte */}
      <View style={styles.footerRow}>
        <View style={styles.providerBadge}>
          <MaterialCommunityIcons name="earth" size={14} color="#16a34a" />
          <Text style={styles.providerText}>OpenStreetMap (API 100% Gratuite)</Text>
        </View>

        <TouchableOpacity
          style={styles.btnOpenOsm}
          onPress={() => Linking.openURL(osmFullUrl)}
        >
          <Text style={styles.btnOpenOsmText}>Agrandir</Text>
          <MaterialCommunityIcons name="open-in-new" size={12} color="#0099cc" />
        </TouchableOpacity>
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    width: '100%',
    borderRadius: 12,
    overflow: 'hidden',
    backgroundColor: '#e2e8f0',
    borderWidth: 1,
    borderColor: '#cbd5e1',
    position: 'relative',
    marginVertical: 12
  },
  footerRow: {
    position: 'absolute',
    bottom: 6,
    left: 8,
    right: 8,
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    backgroundColor: 'rgba(255, 255, 255, 0.92)',
    paddingHorizontal: 10,
    paddingVertical: 5,
    borderRadius: 8,
    borderWidth: 1,
    borderColor: '#e2e8f0'
  },
  providerBadge: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: 5
  },
  providerText: {
    fontSize: 10,
    fontWeight: '700',
    color: '#334155'
  },
  btnOpenOsm: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: 3,
    backgroundColor: '#f0f9ff',
    paddingHorizontal: 8,
    paddingVertical: 3,
    borderRadius: 4
  },
  btnOpenOsmText: {
    fontSize: 10,
    fontWeight: '600',
    color: '#0099cc'
  },
  fallbackBox: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
    backgroundColor: '#f0f9ff'
  },
  fallbackTitle: {
    fontSize: 14,
    fontWeight: '700',
    color: '#0369a1',
    marginTop: 4
  },
  fallbackCoords: {
    fontSize: 11,
    color: '#64748b',
    marginTop: 2
  }
});
