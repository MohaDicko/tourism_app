using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using TourismApp.Web.Data;

var builder = WebApplication.CreateBuilder(args);

// Configuration de la base de données (SQLite par défaut pour portabilité immédiate, ou SQL Server)
bool useSqlServer = builder.Configuration.GetValue<bool>("UseSqlServer");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (useSqlServer)
    {
        var sqlServerConn = builder.Configuration.GetConnectionString("SqlServerConnection");
        options.UseSqlServer(sqlServerConn);
    }
    else
    {
        var sqliteConn = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=tourism_app.db";
        options.UseSqlite(sqliteConn);
    }
});

// Authentification par Cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllMobile", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Initialisation et amorçage des données de démonstration (Marrakech)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Erreur survenue lors de l'initialisation de la base de données.");
    }
}

// Assurer l'existence du dossier uploads
var uploadsPath = Path.Combine(app.Environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");
Directory.CreateDirectory(Path.Combine(uploadsPath, "images"));
Directory.CreateDirectory(Path.Combine(uploadsPath, "audio"));

// Pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error404");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Home/Error404");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowAllMobile");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
