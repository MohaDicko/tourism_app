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
    public class EvenementsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public EvenementsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Evenements
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var events = await _context.Evenements
                .Include(e => e.Endroit)
                .Include(e => e.Participations)
                .OrderByDescending(e => e.DateDebut)
                .ToListAsync();

            return View(events);
        }

        // GET: Evenements/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.EndroitList = new SelectList(await _context.Endroits.OrderBy(e => e.Nom).ToListAsync(), "Id", "Nom");
            return View(new EvenementFormViewModel());
        }

        // POST: Evenements/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EvenementFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.EndroitList = new SelectList(await _context.Endroits.OrderBy(e => e.Nom).ToListAsync(), "Id", "Nom", model.EndroitId);
                return View(model);
            }

            var evt = new Evenement
            {
                Nom = model.Nom,
                Description = model.Description,
                DateDebut = model.DateDebut,
                DateFin = model.DateFin,
                Contacte = model.Contacte,
                EndroitId = model.EndroitId,
                DateCreation = DateTime.UtcNow
            };

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var uploadDir = Path.Combine(_environment.WebRootPath, "uploads", "images");
                if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);
                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(model.ImageFile.FileName)}";
                using (var stream = new FileStream(Path.Combine(uploadDir, fileName), FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }
                evt.ImageUrl = $"/uploads/images/{fileName}";
            }

            _context.Evenements.Add(evt);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"L'événement « {evt.Nom} » a été planifié avec succès.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Evenements/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var evt = await _context.Evenements.FindAsync(id);
            if (evt != null)
            {
                _context.Evenements.Remove(evt);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"L'événement « {evt.Nom} » a été supprimé.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
