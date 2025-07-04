using Microsoft.EntityFrameworkCore;
using FlowingDefault.Core.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowingDefault.Core.Configurations
{
    internal class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable("Project");
            
            builder.HasIndex(p => new { p.UserId, p.Name })
                .IsUnique();
        }
    }
}