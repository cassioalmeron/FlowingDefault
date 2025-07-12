using System.Reflection;
using FlowingDefault.Core.Models;
using FlowingDefault.Core.Utils;
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

    /// <summary>
    /// Ensures that the database exists and the admin user is created (synchronous version)
    /// </summary>
    public void EnsureDatabaseAndAdminUser()
    {
        // Ensure database is created
        Database.EnsureCreated();
        
        // Check if admin user exists
        var adminUser = Users.Find(1);
        
        if (adminUser == null)
        {
            adminUser = new User
            {
                Id = 1,
                Name = "Administrator",
                Username = "admin",
                Password = HashUtils.GenerateMd5Hash("admin")
            };

            Users.Add(adminUser);
            SaveChanges();
        }
    }
}