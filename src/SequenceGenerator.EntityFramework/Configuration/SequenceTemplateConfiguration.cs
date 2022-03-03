using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace SequenceGenerator.EntityFramework.Configuration
{
    public class SequenceTemplateConfiguration : IEntityTypeConfiguration<SequenceTemplate>
    {
        public void Configure(EntityTypeBuilder<SequenceTemplate> builder)
        {
            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd()
                .HasValueGenerator<SequentialGuidValueGenerator>();
            builder.HasAlternateKey(x => x.Key);
            builder.Property(x => x.Template)
                .HasMaxLength(50);
            builder.Property(x => x.PartitionStrategy)
                .HasMaxLength(50);
        }
    }
}
