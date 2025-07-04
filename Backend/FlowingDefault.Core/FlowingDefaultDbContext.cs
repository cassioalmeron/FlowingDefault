using System.Reflection;
using FlowingDefault.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowingDefault.Core;

public class FlowingDefaultDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Project> Projects { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            path = Path.Combine(path, "FlowingDefault", "Database");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var dbPath = Path.Combine(path, "FlowingDefault.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}