using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourismApp.Web.Data;
using TourismApp.Web.Models;

namespace TourismApp.Web.Controllers.Api
{
    [ApiController]
    [Route("api/endroits")]
    public class EndroitsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EndroitsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/endroits
        [HttpGet]
        public async Task<IActionResult> GetEndroits([FromQuery] string? categorie, [FromQuery] string? search, [FromQuery] double? lat, [FromQuery] double? lng, [FromQuery] double? radiusKm)
        {
            var query = _context.Endroits.AsQueryable();

            if (!string.IsNullOrWhiteSpace(categorie) && categorie != "Tous")
            {
                query = query.Where(e => e.TypeCategorie == categorie);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(e => e.Nom.Contains(search) || (e.Description != null && e.Description.Contains(search)));
            }

            var list = await query.ToListAsync();

            // Filtrage par rayon GPS si demandé ("Mapp me")
            if (lat.HasValue && lng.HasValue)
            {
                double maxKm = radiusKm ?? 50.0;
                var filtered = list.Select(e => new
                {
                    Endroit = e,
                    DistanceKm = CalculateDistance(lat.Value, lng.Value, e.Latitude, e.Longitude)
                })
                .Where(x => x.DistanceKm <= maxKm)
                .OrderBy(x => x.DistanceKm)
                .Select(x => new
                {
                    x.Endroit.Id,
                    x.Endroit.Nom,
                    x.Endroit.Description,
                    x.Endroit.Numero,
                    Statut = (int)x.Endroit.Statut,
                    x.Endroit.Latitude,
                    x.Endroit.Longitude,
                    x.Endroit.TypeCategorie,
                    x.Endroit.ImageUrl,
                    x.Endroit.AudioUrl,
                    DistanceKm = Math.Round(x.DistanceKm, 2)
                });

                return Ok(filtered);
            }

            return Ok(list.Select(e => new
            {
                e.Id,
                e.Nom,
                e.Description,
                e.Numero,
                Statut = (int)e.Statut,
                e.Latitude,
                e.Longitude,
                e.TypeCategorie,
                e.ImageUrl,
                e.AudioUrl
            }));
        }

        // GET: api/endroits/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEndroit(int id)
        {
            var e = await _context.Endroits
                .Include(x => x.Notes)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (e == null) return NotFound(new { message = "Site introuvable" });

            return Ok(new
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
                TotalNotes = e.Notes.Count
            });
        }

        private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Rayon de la Terre en km
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private static double ToRadians(double angle) => Math.PI * angle / 180.0;
    }
}
