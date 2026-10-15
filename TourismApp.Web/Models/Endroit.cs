using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourismApp.Web.Models
{
    public enum EndroitStatut
    {
        [Display(Name = "Ouvert")]
        Ouvert = 1,
        [Display(Name = "Fermé")]
        Ferme = 0,
        [Display(Name = "En travaux / Rénovation")]
        Travaux = 2
    }

    public class Endroit
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire")]
        [StringLength(150)]
        [Display(Name = "Nom du site")]
        public string Nom { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [StringLength(50)]
        [Display(Name = "GSM / Téléphone")]
        public string? Numero { get; set; }

        [Display(Name = "Statut")]
        public EndroitStatut Statut { get; set; } = EndroitStatut.Ouvert;

        [Required]
        [Display(Name = "Latitude")]
        public double Latitude { get; set; }

        [Required]
        [Display(Name = "Longitude")]
        public double Longitude { get; set; }

        [Required]
        [StringLength(60)]
        [Display(Name = "Type")]
        public string TypeCategorie { get; set; } = "Place Publique";

        [StringLength(255)]
        [Display(Name = "Image de couverture")]
        public string? ImageUrl { get; set; }

        [StringLength(255)]
        [Display(Name = "Fichier Audio")]
        public string? AudioUrl { get; set; }

        // Spécialisations selon le diagramme de classe PFE
        [StringLength(100)]
        [Display(Name = "Type de jardin")]
        public string? JardinType { get; set; }

        [StringLength(100)]
        [Display(Name = "Date de création du monument")]
        public string? MonumentDateCreation { get; set; }

        [Display(Name = "Menu du restaurant")]
        public string? RestaurantMenu { get; set; }

        [StringLength(100)]
        [Display(Name = "Type de restaurant / cuisine")]
        public string? RestaurantType { get; set; }

        [Display(Name = "Nombre d'étoiles (Hébergement)")]
        [Range(1, 7)]
        public int? HebergementNombreEtoile { get; set; }

        [StringLength(100)]
        [Display(Name = "Type d'hébergement")]
        public string? HebergementType { get; set; }

        [StringLength(50)]
        [Display(Name = "Heure d'ouverture")]
        public string? CafeDateOuvert { get; set; }

        [StringLength(50)]
        [Display(Name = "Heure de fermeture")]
        public string? CafeDateFermer { get; set; }

        public DateTime DateCreation { get; set; } = DateTime.UtcNow;
        public DateTime? DateModification { get; set; }

        // Relations
        public ICollection<ParcoursEndroit> ParcoursEndroits { get; set; } = new List<ParcoursEndroit>();
        public ICollection<Evenement> Evenements { get; set; } = new List<Evenement>();
        public ICollection<Favori> Favoris { get; set; } = new List<Favori>();
        public ICollection<NoteEndroit> Notes { get; set; } = new List<NoteEndroit>();
    }
}
