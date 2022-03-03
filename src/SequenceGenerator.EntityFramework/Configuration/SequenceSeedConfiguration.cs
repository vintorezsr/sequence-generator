using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace SequenceGenerator.EntityFramework.Configuration
{
    public class SequenceSeedConfiguration : IEntityTypeConfiguration<SequenceSeed>
    {
        public void Configure(EntityTypeBuilder<SequenceSeed> builder)
        {
            builder.Property(x => x.Id)
                .HasDefaultValueSql("NEWSEQUENTIALID()");
            builder.Property(x => x.PartitionStrategy)
                .HasMaxLength(150);
        }
    }
}
