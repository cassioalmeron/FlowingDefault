using FlowingDefault.Core;
using Microsoft.EntityFrameworkCore;

namespace FlowingDefault.Tests.Mocks
{
    internal class TestDbContext : FlowingDefaultDbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var rnd = new Random();

            // Here it configured the Memory Database
            optionsBuilder.UseInMemoryDatabase($"Virtual-Database-Name-{rnd.Next(1, 1000)}")
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();
        }
    }
}