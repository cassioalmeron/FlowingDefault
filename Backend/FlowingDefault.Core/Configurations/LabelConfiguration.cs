using Microsoft.EntityFrameworkCore;
using FlowingDefault.Core.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowingDefault.Core.Configurations
{
    internal class LabelConfiguration : IEntityTypeConfiguration<Label>
    {
        public void Configure(EntityTypeBuilder<Label> builder)
        {
            builder.ToTable("Label");
            
            builder.HasIndex(u => u.Name)
                .IsUnique();
        }
    }
}