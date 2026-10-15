using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourismApp.Web.Models
{
    public class Favori
    {
        [Key]
        public int Id { get; set; }

        public DateTime Date_ajout { get; set; } = DateTime.UtcNow;

        [Required]
        public int UtilisateurId { get; set; }

        [ForeignKey(nameof(UtilisateurId))]
        public Utilisateur Utilisateur { get; set; } = null!;

        [Required]
        public int EndroitId { get; set; }

        [ForeignKey(nameof(EndroitId))]
        public Endroit Endroit { get; set; } = null!;
    }

    public class ParticiperEvenement
    {
        [Key]
        public int Id { get; set; }

        public DateTime Date_part { get; set; } = DateTime.UtcNow;

        [Required]
        public int UtilisateurId { get; set; }

        [ForeignKey(nameof(UtilisateurId))]
        public Utilisateur Utilisateur { get; set; } = null!;

        [Required]
        public int EvenementId { get; set; }

        [ForeignKey(nameof(EvenementId))]
        public Evenement Evenement { get; set; } = null!;
    }

    public class NoteEndroit
    {
        [Key]
        public int Id { get; set; }

        [Range(1, 5)]
        public int Note { get; set; } = 5;

        public string? Commentaire { get; set; }

        public DateTime DateNote { get; set; } = DateTime.UtcNow;

        [Required]
        public int UtilisateurId { get; set; }

        [ForeignKey(nameof(UtilisateurId))]
        public Utilisateur Utilisateur { get; set; } = null!;

        [Required]
        public int EndroitId { get; set; }

        [ForeignKey(nameof(EndroitId))]
        public Endroit Endroit { get; set; } = null!;
    }
}
