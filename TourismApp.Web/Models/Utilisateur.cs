using System.ComponentModel.DataAnnotations;

namespace TourismApp.Web.Models
{
    public class Utilisateur
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom d'utilisateur est obligatoire")]
        [StringLength(100)]
        [Display(Name = "Nom d'utilisateur")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Prénom")]
        public string First_Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Nom")]
        public string Last_Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        [StringLength(50)]
        [Display(Name = "Téléphone")]
        public string? Telephone { get; set; }

        [StringLength(100)]
        [Display(Name = "Nationalité")]
        public string? Nationalite { get; set; }

        [StringLength(50)]
        public string Role { get; set; } = "User"; // "Admin" ou "User"

        public DateTime DateInscription { get; set; } = DateTime.UtcNow;

        public ICollection<Favori> Favoris { get; set; } = new List<Favori>();
        public ICollection<ParticiperEvenement> Participations { get; set; } = new List<ParticiperEvenement>();
        public ICollection<NoteEndroit> Notes { get; set; } = new List<NoteEndroit>();
    }
}
