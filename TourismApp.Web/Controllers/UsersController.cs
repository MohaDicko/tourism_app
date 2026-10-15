using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourismApp.Web.Data;

namespace TourismApp.Web.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Users (Figure 16: Profiles des utilisateurs)
        [HttpGet]
        public async Task<IActionResult> Index(string? search)
        {
            ViewData["CurrentSearch"] = search;
            var query = _context.Utilisateurs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u => u.First_Name.Contains(search) || 
                                         u.Last_Name.Contains(search) || 
                                         u.Username.Contains(search) ||
                                         u.Email.Contains(search));
            }

            var users = await query.OrderBy(u => u.Id).ToListAsync();
            return View(users);
        }
    }
}
