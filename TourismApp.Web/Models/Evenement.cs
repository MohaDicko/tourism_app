using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourismApp.Web.Models
{
    public class Evenement
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom de l'événement est obligatoire")]
        [StringLength(150)]
        [Display(Name = "Nom de l'événement")]
        public string Nom { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Date de début")]
        public DateTime DateDebut { get; set; } = DateTime.Today;

        [Required]
        [Display(Name = "Date de fin")]
        public DateTime DateFin { get; set; } = DateTime.Today.AddDays(1);

        [StringLength(50)]
        [Display(Name = "Contact")]
        public string? Contacte { get; set; }

        [StringLength(255)]
        [Display(Name = "Image")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Lieu / Endroit associé")]
        public int? EndroitId { get; set; }

        [ForeignKey(nameof(EndroitId))]
        public Endroit? Endroit { get; set; }

        public DateTime DateCreation { get; set; } = DateTime.UtcNow;

        public ICollection<ParticiperEvenement> Participations { get; set; } = new List<ParticiperEvenement>();
    }
}
