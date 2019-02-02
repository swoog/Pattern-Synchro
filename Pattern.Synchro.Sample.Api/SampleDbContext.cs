using Microsoft.EntityFrameworkCore;

namespace Pattern.Synchro.Tests
{
    public class SampleDbContext : DbContext
    {
        public SampleDbContext()
        {
        }

        public SampleDbContext(DbContextOptions<SampleDbContext> options)
            : base(options)
        {
        }

        public DbSet<Model.Car> Cars { get; set; }
    }
}