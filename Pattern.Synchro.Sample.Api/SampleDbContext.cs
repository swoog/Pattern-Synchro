using Microsoft.EntityFrameworkCore;

namespace Pattern.Synchro.Sample.Api
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

        public DbSet<Device> Devices { get; set; }
        
        public DbSet<Car> Cars { get; set; }
    }
}