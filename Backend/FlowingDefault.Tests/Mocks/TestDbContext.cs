using System.Net.NetworkInformation;
using FlowingDefault.Core;
using FlowingDefault.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowingDefault.Tests.Mocks
{
    internal class TestDbContext : FlowingDefaultDbContext
    {
        private static int _count;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Here it configured the Memory Database
            optionsBuilder.UseInMemoryDatabase($"Virtual-Database-Name-{_count++}")
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();
        }

        /// <summary>
        /// Initialize the test database with admin user
        /// </summary>
        public void InitializeTestDatabase()
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
                    Password = "admin" // Use plain text for tests
                };

                Users.Add(adminUser);
                SaveChanges();
            }
        }
    }
}