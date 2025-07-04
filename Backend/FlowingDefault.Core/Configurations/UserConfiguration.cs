using Microsoft.EntityFrameworkCore;
using FlowingDefault.Core.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowingDefault.Core.Configurations
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("User");
            
            builder.HasIndex(u => u.Username)
                .IsUnique();
        }
    }
}