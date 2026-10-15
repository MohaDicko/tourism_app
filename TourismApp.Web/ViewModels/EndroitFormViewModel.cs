using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using TourismApp.Web.Models;

namespace TourismApp.Web.ViewModels
{
    public class EndroitFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom du site est obligatoire")]
        [StringLength(150)]
        [Display(Name = "Nom")]
        public string Nom { get; set; } = string.Empty;

        [Required(ErrorMessage = "La longitude est obligatoire")]
        [Display(Name = "Longitude")]
        public double Longitude { get; set; } = -7.9891;

        [Required(ErrorMessage = "La latitude est obligatoire")]
        [Display(Name = "Latitude")]
        public double Latitude { get; set; } = 31.6258;

        [StringLength(50)]
        [Display(Name = "GSM / Téléphone")]
        public string? Numero { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Statut")]
        public EndroitStatut Statut { get; set; } = EndroitStatut.Ouvert;

        [Required]
        [Display(Name = "Type")]
        public string TypeCategorie { get; set; } = "Place Publique";

        [Display(Name = "Fichier Image")]
        public IFormFile? ImageFile { get; set; }

        public string? ExistingImageUrl { get; set; }

        [Display(Name = "Fichier Audio (Audio-guide)")]
        public IFormFile? AudioFile { get; set; }

        public string? ExistingAudioUrl { get; set; }

        // Spécialisations selon catégorie
        [Display(Name = "Type de jardin")]
        public string? JardinType { get; set; }

        [Display(Name = "Date de création du monument")]
        public string? MonumentDateCreation { get; set; }

        [Display(Name = "Menu du restaurant")]
        public string? RestaurantMenu { get; set; }

        [Display(Name = "Type de restaurant")]
        public string? RestaurantType { get; set; }

        [Display(Name = "Nombre d'étoiles")]
        public int? HebergementNombreEtoile { get; set; }

        [Display(Name = "Type d'hébergement")]
        public string? HebergementType { get; set; }

        [Display(Name = "Heure d'ouverture")]
        public string? CafeDateOuvert { get; set; }

        [Display(Name = "Heure de fermeture")]
        public string? CafeDateFermer { get; set; }
    }
}
