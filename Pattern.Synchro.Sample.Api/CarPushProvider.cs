using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Pattern.Synchro.Api;
using Pattern.Synchro.Api.Push;

namespace Pattern.Synchro.Sample.Api
{
    public class CarPushProvider : DbSetPushProvider<SampleDbContext, Car, Client.Car>
    {
        public CarPushProvider(SampleDbContext sampleDbContext, IDateTimeService dateTimeService) 
            : base(sampleDbContext, dateTimeService)
        {
        }

        protected override DbSet<Car> GetDbSet(SampleDbContext db)
        {
            return db.Cars;
        }

        protected override bool UpdateProperties(HttpContext context, Client.Car entity, Car car)
        {
            car.UserId = context.User.Identity.Name;
            if (car.Name != entity.Name)
            {
                car.Name = entity.Name;
                return true;
            }

            return false;
        }
    }
}