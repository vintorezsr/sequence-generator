using Microsoft.EntityFrameworkCore;

namespace SequenceGenerator.EntityFramework
{
    public class SequenceDbContext : DbContext
    {
        public SequenceDbContext(DbContextOptions<SequenceDbContext> dbContextOptions)
            : base(dbContextOptions)
        {
        }

        public virtual DbSet<SequenceTemplate> SequenceTemplates { get; set; }
        public virtual DbSet<SequenceSeed> SequenceSeeds { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SequenceDbContext).Assembly);
        }
    }
}