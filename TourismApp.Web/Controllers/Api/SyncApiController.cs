using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourismApp.Web.Data;

namespace TourismApp.Web.Controllers.Api
{
    [ApiController]
    [Route("api/sync")]
    public class SyncApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SyncApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/sync/all (Utilisé par le service de synchronisation de l'application mobile)
        [HttpGet("all")]
        public async Task<IActionResult> GetAllData()
        {
            var endroits = await _context.Endroits
                .AsNoTracking()
                .Select(e => new
                {
                    e.Id,
                    e.Nom,
                    e.Description,
                    e.Numero,
                    Statut = (int)e.Statut,
                    StatutLibelle = e.Statut.ToString(),
                    e.Latitude,
                    e.Longitude,
                    e.TypeCategorie,
                    e.ImageUrl,
                    e.AudioUrl,
                    e.JardinType,
                    e.MonumentDateCreation,
                    e.RestaurantMenu,
                    e.RestaurantType,
                    e.HebergementNombreEtoile,
                    e.HebergementType,
                    e.CafeDateOuvert,
                    e.CafeDateFermer,
                    NoteMoyenne = e.Notes.Any() ? Math.Round(e.Notes.Average(n => n.Note), 1) : 4.5,
                    TotalAvis = e.Notes.Count()
                })
                .ToListAsync();

            var parcours = await _context.Parcours
                .AsNoTracking()
                .Select(p => new
                {
                    p.Id,
                    p.Nom,
                    p.Description,
                    p.Duree,
                    p.Distance,
                    p.ImageUrl,
                    Etapes = p.Etapes.OrderBy(pe => pe.Rang).Select(pe => new
                    {
                        pe.Rang,
                        EndroitId = pe.Endroit.Id,
                        Nom = pe.Endroit.Nom,
                        pe.Endroit.Latitude,
                        pe.Endroit.Longitude,
                        pe.Endroit.ImageUrl
                    })
                })
                .ToListAsync();

            var evenements = await _context.Evenements
                .AsNoTracking()
                .Select(ev => new
                {
                    ev.Id,
                    ev.Nom,
                    ev.Description,
                    ev.DateDebut,
                    ev.DateFin,
                    ev.Contacte,
                    ev.ImageUrl,
                    ev.EndroitId,
                    LieuNom = ev.Endroit != null ? ev.Endroit.Nom : "Marrakech",
                    NombreParticipants = ev.Participations.Count()
                })
                .ToListAsync();

            var categories = new[]
            {
                new { Id = 1, Nom = "Place Publique", Icone = "map-marker-radius" },
                new { Id = 2, Nom = "Monument", Icone = "castle" },
                new { Id = 3, Nom = "Musée", Icone = "bank" },
                new { Id = 4, Nom = "Jardin", Icone = "flower" },
                new { Id = 5, Nom = "Restaurant", Icone = "silverware-fork-knife" },
                new { Id = 6, Nom = "Café", Icone = "coffee" },
                new { Id = 7, Nom = "Hôtel", Icone = "bed" },
                new { Id = 8, Nom = "Hôpitale", Icone = "hospital-box" },
                new { Id = 9, Nom = "Aéroport", Icone = "airplane" }
            };

            return Ok(new
            {
                Timestamp = DateTime.UtcNow,
                Version = "1.0",
                Categories = categories,
                Endroits = endroits,
                Parcours = parcours,
                Evenements = evenements
            });
        }
    }
}
