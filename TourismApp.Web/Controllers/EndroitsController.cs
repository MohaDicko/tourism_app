using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourismApp.Web.Data;
using TourismApp.Web.Models;
using TourismApp.Web.ViewModels;

namespace TourismApp.Web.Controllers
{
    [Authorize]
    public class EndroitsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public EndroitsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Endroits (Figure 13 & 18)
        [HttpGet]
        public async Task<IActionResult> Index(string? search)
        {
            ViewData["CurrentSearch"] = search;
            var query = _context.Endroits.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(e => e.Nom.Contains(search) || 
                                         e.TypeCategorie.Contains(search) || 
                                         (e.Description != null && e.Description.Contains(search)));
            }

            var list = await query.OrderBy(e => e.Id).ToListAsync();
            return View(list);
        }

        // GET: Endroits/Details/5 (Figure 15)
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var endroit = await _context.Endroits
                .Include(e => e.Notes)
                .ThenInclude(n => n.Utilisateur)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (endroit == null) return NotFound();

            return View(endroit);
        }

        // GET: Endroits/Create (Figure 14)
        [HttpGet]
        public IActionResult Create()
        {
            return View(new EndroitFormViewModel());
        }

        // POST: Endroits/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EndroitFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var endroit = new Endroit
            {
                Nom = model.Nom,
                Description = model.Description,
                Numero = model.Numero,
                Statut = model.Statut,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                TypeCategorie = model.TypeCategorie,
                JardinType = model.JardinType,
                MonumentDateCreation = model.MonumentDateCreation,
                RestaurantMenu = model.RestaurantMenu,
                RestaurantType = model.RestaurantType,
                HebergementNombreEtoile = model.HebergementNombreEtoile,
                HebergementType = model.HebergementType,
                CafeDateOuvert = model.CafeDateOuvert,
                CafeDateFermer = model.CafeDateFermer,
                DateCreation = DateTime.UtcNow
            };

            // Traitement upload Image
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                endroit.ImageUrl = await SaveFileAsync(model.ImageFile, "images");
            }

            // Traitement upload Audio
            if (model.AudioFile != null && model.AudioFile.Length > 0)
            {
                endroit.AudioUrl = await SaveFileAsync(model.AudioFile, "audio");
            }

            _context.Add(endroit);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Le site « {endroit.Nom} » a été ajouté avec succès.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Endroits/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var endroit = await _context.Endroits.FindAsync(id);
            if (endroit == null) return NotFound();

            var model = new EndroitFormViewModel
            {
                Id = endroit.Id,
                Nom = endroit.Nom,
                Description = endroit.Description,
                Numero = endroit.Numero,
                Statut = endroit.Statut,
                Latitude = endroit.Latitude,
                Longitude = endroit.Longitude,
                TypeCategorie = endroit.TypeCategorie,
                ExistingImageUrl = endroit.ImageUrl,
                ExistingAudioUrl = endroit.AudioUrl,
                JardinType = endroit.JardinType,
                MonumentDateCreation = endroit.MonumentDateCreation,
                RestaurantMenu = endroit.RestaurantMenu,
                RestaurantType = endroit.RestaurantType,
                HebergementNombreEtoile = endroit.HebergementNombreEtoile,
                HebergementType = endroit.HebergementType,
                CafeDateOuvert = endroit.CafeDateOuvert,
                CafeDateFermer = endroit.CafeDateFermer
            };

            return View(model);
        }

        // POST: Endroits/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EndroitFormViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var endroit = await _context.Endroits.FindAsync(id);
            if (endroit == null) return NotFound();

            endroit.Nom = model.Nom;
            endroit.Description = model.Description;
            endroit.Numero = model.Numero;
            endroit.Statut = model.Statut;
            endroit.Latitude = model.Latitude;
            endroit.Longitude = model.Longitude;
            endroit.TypeCategorie = model.TypeCategorie;
            endroit.JardinType = model.JardinType;
            endroit.MonumentDateCreation = model.MonumentDateCreation;
            endroit.RestaurantMenu = model.RestaurantMenu;
            endroit.RestaurantType = model.RestaurantType;
            endroit.HebergementNombreEtoile = model.HebergementNombreEtoile;
            endroit.HebergementType = model.HebergementType;
            endroit.CafeDateOuvert = model.CafeDateOuvert;
            endroit.CafeDateFermer = model.CafeDateFermer;
            endroit.DateModification = DateTime.UtcNow;

            // Upload nouvelle image si fournie
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                endroit.ImageUrl = await SaveFileAsync(model.ImageFile, "images");
            }

            // Upload nouvel audio si fourni
            if (model.AudioFile != null && model.AudioFile.Length > 0)
            {
                endroit.AudioUrl = await SaveFileAsync(model.AudioFile, "audio");
            }

            _context.Update(endroit);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Le site « {endroit.Nom} » a été mis à jour avec succès.";
            return RedirectToAction(nameof(Details), new { id = endroit.Id });
        }

        // POST: Endroits/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var endroit = await _context.Endroits.FindAsync(id);
            if (endroit != null)
            {
                _context.Endroits.Remove(endroit);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Le site « {endroit.Nom} » a été supprimé.";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<string> SaveFileAsync(IFormFile file, string folder)
        {
            var uploadDir = Path.Combine(_environment.WebRootPath, "uploads", folder);
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(uploadDir, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/{folder}/{uniqueFileName}";
        }
    }
}
