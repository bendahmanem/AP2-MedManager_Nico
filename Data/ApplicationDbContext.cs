using System;
using Microsoft.EntityFrameworkCore;
using MedManager.Models;

namespace newEmpty.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<Student> Student { get; set; }
    public DbSet<Instructor> Instructor { get; set; }


    // Constructeur
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}
