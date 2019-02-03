using Microsoft.EntityFrameworkCore;
using Pattern.Synchro.Api;
using Pattern.Synchro.Api.Pull;

namespace Pattern.Synchro.Sample.Api
{
    public class CarPullProvider : DbSetPullProvider<SampleDbContext, Car, Client.Car>
    {
        public CarPullProvider(SampleDbContext sampleDbContext) 
            : base(sampleDbContext)
        {
        }

        protected override DbSet<Car> GetDbSet(SampleDbContext db)
        {
            return db.Cars;
        }

        protected override void UpdateProperties(Client.Car entity, Car car)
        {
            entity.Name = car.Name;
        }
    }
}