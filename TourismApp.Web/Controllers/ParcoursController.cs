using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TourismApp.Web.Data;
using TourismApp.Web.Models;
using TourismApp.Web.ViewModels;

namespace TourismApp.Web.Controllers
{
    [Authorize]
    public class ParcoursController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ParcoursController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Parcours
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var parcours = await _context.Parcours
                .Include(p => p.Etapes)
                .ThenInclude(e => e.Endroit)
                .OrderBy(p => p.Id)
                .ToListAsync();

            return View(parcours);
        }

        // GET: Parcours/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var parcours = await _context.Parcours
                .Include(p => p.Etapes.OrderBy(e => e.Rang))
                .ThenInclude(e => e.Endroit)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (parcours == null) return NotFound();

            return View(parcours);
        }

        // GET: Parcours/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Endroits = await _context.Endroits.OrderBy(e => e.Nom).ToListAsync();
            return View(new ParcoursFormViewModel());
        }

        // POST: Parcours/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ParcoursFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Endroits = await _context.Endroits.OrderBy(e => e.Nom).ToListAsync();
                return View(model);
            }

            var parcours = new Parcours
            {
                Nom = model.Nom,
                Description = model.Description,
                Duree = model.Duree,
                Distance = model.Distance,
                DateCreation = DateTime.UtcNow
            };

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                parcours.ImageUrl = await SaveFileAsync(model.ImageFile);
            }

            _context.Parcours.Add(parcours);
            await _context.SaveChangesAsync();

            // Ajout des étapes
            if (model.SelectedEndroitIds != null && model.SelectedEndroitIds.Any())
            {
                int rang = 1;
                foreach (var endroitId in model.SelectedEndroitIds)
                {
                    _context.ParcoursEndroits.Add(new ParcoursEndroit
                    {
                        ParcoursId = parcours.Id,
                        EndroitId = endroitId,
                        Rang = rang++
                    });
                }
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = $"Le parcours « {parcours.Nom} » a été créé.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Parcours/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var parcours = await _context.Parcours.FindAsync(id);
            if (parcours != null)
            {
                _context.Parcours.Remove(parcours);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Le parcours « {parcours.Nom} » a été supprimé.";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<string> SaveFileAsync(IFormFile file)
        {
            var uploadDir = Path.Combine(_environment.WebRootPath, "uploads", "images");
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

            return $"/uploads/images/{uniqueFileName}";
        }
    }
}
