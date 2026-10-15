using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace TourismApp.Web.ViewModels
{
    public class ParcoursFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom du parcours est obligatoire")]
        [StringLength(150)]
        [Display(Name = "Nom du parcours")]
        public string Nom { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [StringLength(50)]
        [Display(Name = "Durée estimée")]
        public string? Duree { get; set; }

        [StringLength(50)]
        [Display(Name = "Distance estimée")]
        public string? Distance { get; set; }

        [Display(Name = "Image d'illustration")]
        public IFormFile? ImageFile { get; set; }

        public string? ExistingImageUrl { get; set; }

        [Display(Name = "Étapes du parcours (sélectionner les sites dans l'ordre)")]
        public List<int> SelectedEndroitIds { get; set; } = new();
    }

    public class EvenementFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom de l'événement est obligatoire")]
        [StringLength(150)]
        [Display(Name = "Nom de l'événement")]
        public string Nom { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "La date de début est obligatoire")]
        [Display(Name = "Date de début")]
        [DataType(DataType.Date)]
        public DateTime DateDebut { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "La date de fin est obligatoire")]
        [Display(Name = "Date de fin")]
        [DataType(DataType.Date)]
        public DateTime DateFin { get; set; } = DateTime.Today.AddDays(1);

        [StringLength(50)]
        [Display(Name = "Contact téléphonique")]
        public string? Contacte { get; set; }

        [Display(Name = "Lieu / Site associé")]
        public int? EndroitId { get; set; }

        [Display(Name = "Affiche / Image")]
        public IFormFile? ImageFile { get; set; }

        public string? ExistingImageUrl { get; set; }
    }
}
