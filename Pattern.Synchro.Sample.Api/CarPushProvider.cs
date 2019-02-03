using Microsoft.EntityFrameworkCore;
using Pattern.Synchro.Api;
using Pattern.Synchro.Api.Push;
using Remotion.Linq.Clauses;

namespace Pattern.Synchro.Sample.Api
{
    public class CarPushProvider : DbSetPushProvider<SampleDbContext, Car, Client.Car>
    {
        public CarPushProvider(SampleDbContext sampleDbContext) : base(sampleDbContext)
        {
        }

        protected override DbSet<Car> GetDbSet(SampleDbContext db)
        {
            return db.Cars;
        }

        protected override void UpdateProperties(Client.Car entity, Car car)
        {
            car.Name = entity.Name;
        }
    }
}