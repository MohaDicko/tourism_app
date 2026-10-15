using System.ComponentModel.DataAnnotations;

namespace TourismApp.Web.Models
{
    public class Parcours
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom du parcours est obligatoire")]
        [StringLength(150)]
        [Display(Name = "Nom du parcours")]
        public string Nom { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [StringLength(50)]
        [Display(Name = "Durée")]
        public string? Duree { get; set; } // ex: "3 heures"

        [StringLength(50)]
        [Display(Name = "Distance")]
        public string? Distance { get; set; } // ex: "4.5 km"

        [StringLength(255)]
        [Display(Name = "Image d'illustration")]
        public string? ImageUrl { get; set; }

        public DateTime DateCreation { get; set; } = DateTime.UtcNow;

        // Liste ordonnée d'endroits composants ce parcours
        public ICollection<ParcoursEndroit> Etapes { get; set; } = new List<ParcoursEndroit>();
    }
}
