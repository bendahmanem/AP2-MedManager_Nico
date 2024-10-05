using Microsoft.EntityFrameworkCore;
using MedManager.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MedManager.Data;

public class ApplicationDbContext : IdentityDbContext<Medecin>
{

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Allergie> Allergies => Set<Allergie>();
    public DbSet<Ordonnance> Ordonnances => Set<Ordonnance>();
    public DbSet<Medicament> Medicaments => Set<Medicament>();
    public DbSet<Antecedent> Antecedents => Set<Antecedent>();

    // Constructeur
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Patient>()
            .HasMany(p => p.Allergies)
            .WithMany(a => a.Patients);


        modelBuilder.Entity<Patient>()
            .HasMany(p => p.Antecedents)
            .WithMany(a => a.Patients);

        modelBuilder.Entity<Allergie>()
            .HasMany(a => a.Medicaments)
            .WithMany(m => m.Allergies);

        modelBuilder.Entity<Antecedent>()
            .HasMany(a => a.Medicaments)
            .WithMany(m => m.Antecedents);

        modelBuilder.Entity<Ordonnance>()
            .HasOne(o => o.Patient)
            .WithMany(p => p.Ordonnances)
            .HasForeignKey(o => o.PatientId);

        modelBuilder.Entity<Medecin>()
            .HasMany(m => m.Patients)
            .WithOne(p => p.medecin)
            .HasForeignKey(p => p.MedecinID);

        modelBuilder.Entity<Medecin>()
            .HasMany(m => m.Ordonnances)
            .WithOne(o => o.Medecin)
            .HasForeignKey(o => o.MedecinId);

        modelBuilder.Entity<Ordonnance>()
            .HasMany(o => o.Medicaments)
            .WithMany(m => m.Ordonnances);





    }
}
