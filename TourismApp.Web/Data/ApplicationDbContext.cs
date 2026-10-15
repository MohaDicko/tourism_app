using Microsoft.EntityFrameworkCore;
using TourismApp.Web.Models;

namespace TourismApp.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Endroit> Endroits => Set<Endroit>();
        public DbSet<Evenement> Evenements => Set<Evenement>();
        public DbSet<Parcours> Parcours => Set<Parcours>();
        public DbSet<ParcoursEndroit> ParcoursEndroits => Set<ParcoursEndroit>();
        public DbSet<Utilisateur> Utilisateurs => Set<Utilisateur>();
        public DbSet<Favori> Favoris => Set<Favori>();
        public DbSet<ParticiperEvenement> ParticiperEvenements => Set<ParticiperEvenement>();
        public DbSet<NoteEndroit> NoteEndroits => Set<NoteEndroit>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Index unique pour Username et Email
            modelBuilder.Entity<Utilisateur>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Utilisateur>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Relation Parcours - Endroit (Ordre / Rang)
            modelBuilder.Entity<ParcoursEndroit>()
                .HasOne(pe => pe.Parcours)
                .WithMany(p => p.Etapes)
                .HasForeignKey(pe => pe.ParcoursId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ParcoursEndroit>()
                .HasOne(pe => pe.Endroit)
                .WithMany(e => e.ParcoursEndroits)
                .HasForeignKey(pe => pe.EndroitId)
                .OnDelete(DeleteBehavior.Cascade);

            // Favoris
            modelBuilder.Entity<Favori>()
                .HasOne(f => f.Utilisateur)
                .WithMany(u => u.Favoris)
                .HasForeignKey(f => f.UtilisateurId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Favori>()
                .HasOne(f => f.Endroit)
                .WithMany(e => e.Favoris)
                .HasForeignKey(f => f.EndroitId)
                .OnDelete(DeleteBehavior.Cascade);

            // Participer Evenement
            modelBuilder.Entity<ParticiperEvenement>()
                .HasOne(pe => pe.Utilisateur)
                .WithMany(u => u.Participations)
                .HasForeignKey(pe => pe.UtilisateurId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ParticiperEvenement>()
                .HasOne(pe => pe.Evenement)
                .WithMany(e => e.Participations)
                .HasForeignKey(pe => pe.EvenementId)
                .OnDelete(DeleteBehavior.Cascade);

            // Note Endroit
            modelBuilder.Entity<NoteEndroit>()
                .HasOne(n => n.Utilisateur)
                .WithMany(u => u.Notes)
                .HasForeignKey(n => n.UtilisateurId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NoteEndroit>()
                .HasOne(n => n.Endroit)
                .WithMany(e => e.Notes)
                .HasForeignKey(n => n.EndroitId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
