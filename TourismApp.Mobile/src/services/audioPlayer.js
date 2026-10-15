import { Platform } from 'react-native';

class AudioGuideService {
  constructor() {
    this.currentAudio = null;
    this.speechUtterance = null;
    this.isPlaying = false;
    this.currentSite = null;
    this.onProgressCallback = null;
    this.progressInterval = null;
    this.progress = 0;
    this.duration = 100;
  }

  playSite(site, lang = 'fr', onProgress, onEnd) {
    this.stop();
    this.currentSite = site;
    this.onProgressCallback = onProgress;
    this.isPlaying = true;

    // Cas 1 : Le site possède un fichier MP3 (uploadé par l'admin ou URL)
    if (site.audioUrl && (site.audioUrl.startsWith('http') || site.audioUrl.startsWith('/uploads'))) {
      const fullUrl = site.audioUrl.startsWith('http') ? site.audioUrl : `http://localhost:5240${site.audioUrl}`;
      try {
        if (typeof Audio !== 'undefined') {
          this.currentAudio = new Audio(fullUrl);
          this.currentAudio.play().catch(e => console.log("Audio play error, fallback to TTS:", e));
          
          this.currentAudio.ontimeupdate = () => {
            if (this.currentAudio && this.currentAudio.duration) {
              const progress = this.currentAudio.currentTime / this.currentAudio.duration;
              this.progress = progress;
              if (this.onProgressCallback) this.onProgressCallback(progress, this.currentAudio.currentTime, this.currentAudio.duration);
            }
          };

          this.currentAudio.onended = () => {
            this.stop();
            if (onEnd) onEnd();
          };
          return;
        }
      } catch (err) {
        console.log("Erreur lecture MP3, bascule sur la synthèse vocale :", err);
      }
    }

    // Cas 2 : Synthèse Vocale (Text-To-Speech) naturelle et gratuite
    // Lit le nom du monument et sa description historique complète à haute voix !
    if (typeof window !== 'undefined' && 'speechSynthesis' in window) {
      window.speechSynthesis.cancel();

      const textToSpeak = `${site.nom}. ${site.description || "Aucune description vocale disponible."}`;
      this.speechUtterance = new SpeechSynthesisUtterance(textToSpeak);

      // Adaptation de la langue vocale (Français, Anglais, Espagnol)
      if (lang === 'fr') this.speechUtterance.lang = 'fr-FR';
      else if (lang === 'en') this.speechUtterance.lang = 'en-US';
      else if (lang === 'es') this.speechUtterance.lang = 'es-ES';

      this.speechUtterance.rate = 0.95; // Rythme de diction posé pour un guide touristique
      this.speechUtterance.pitch = 1.0;

      // Estimation de durée pour la barre de progression (mots / minute)
      const wordsCount = textToSpeak.split(' ').length;
      const estimatedDurationSec = Math.max(10, Math.round(wordsCount / 2.2));
      let elapsedSec = 0;

      this.progressInterval = setInterval(() => {
        elapsedSec += 0.5;
        const progress = Math.min(1, elapsedSec / estimatedDurationSec);
        this.progress = progress;
        if (this.onProgressCallback) {
          this.onProgressCallback(progress, elapsedSec, estimatedDurationSec);
        }
      }, 500);

      this.speechUtterance.onend = () => {
        this.stop();
        if (onEnd) onEnd();
      };

      this.speechUtterance.onerror = () => {
        this.stop();
      };

      window.speechSynthesis.speak(this.speechUtterance);
    }
  }

  togglePause() {
    if (!this.isPlaying) {
      this.resume();
    } else {
      this.pause();
    }
    return this.isPlaying;
  }

  pause() {
    this.isPlaying = false;
    if (this.currentAudio) {
      this.currentAudio.pause();
    } else if (typeof window !== 'undefined' && window.speechSynthesis) {
      window.speechSynthesis.pause();
    }
    if (this.progressInterval) clearInterval(this.progressInterval);
  }

  resume() {
    this.isPlaying = true;
    if (this.currentAudio) {
      this.currentAudio.play();
    } else if (typeof window !== 'undefined' && window.speechSynthesis) {
      window.speechSynthesis.resume();
    }
  }

  stop() {
    this.isPlaying = false;
    this.progress = 0;
    if (this.currentAudio) {
      this.currentAudio.pause();
      this.currentAudio = null;
    }
    if (typeof window !== 'undefined' && window.speechSynthesis) {
      window.speechSynthesis.cancel();
    }
    if (this.progressInterval) {
      clearInterval(this.progressInterval);
      this.progressInterval = null;
    }
    this.currentSite = null;
    if (this.onProgressCallback) {
      this.onProgressCallback(0, 0, 0);
    }
  }
}

export const audioService = new AudioGuideService();
