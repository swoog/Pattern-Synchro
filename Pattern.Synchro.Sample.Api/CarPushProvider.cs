using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Pattern.Synchro.Api.Push;

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

        protected override void UpdateProperties(HttpContext context, Client.Car entity, Car car)
        {
            car.UserId = context.User.Identity.Name;
            car.Name = entity.Name;
        }
    }
}