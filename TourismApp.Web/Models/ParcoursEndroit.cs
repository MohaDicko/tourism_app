using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourismApp.Web.Models
{
    // Correspond à l'entité associative "Ordre" du diagramme UML avec attribut "Rang"
    public class ParcoursEndroit
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ParcoursId { get; set; }

        [ForeignKey(nameof(ParcoursId))]
        public Parcours Parcours { get; set; } = null!;

        [Required]
        public int EndroitId { get; set; }

        [ForeignKey(nameof(EndroitId))]
        public Endroit Endroit { get; set; } = null!;

        [Display(Name = "Rang")]
        public int Rang { get; set; } = 1;
    }
}
