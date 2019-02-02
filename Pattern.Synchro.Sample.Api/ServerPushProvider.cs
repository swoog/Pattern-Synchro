using System.Threading.Tasks;
using Pattern.Synchro.Api;

namespace Pattern.Synchro.Sample.Api
{
    public class ServerPushProvider : ServerPushProviderBase<Client.Car>
    {
        private readonly SampleDbContext sampleDbContext;

        public ServerPushProvider(SampleDbContext sampleDbContext)
        {
            this.sampleDbContext = sampleDbContext;
        }

        protected override async Task Push(Client.Car entity)
        {
            var car = await this.sampleDbContext.Cars.FindAsync(entity.Id);

            if (car == null)
            {
                car = new Car
                {
                    Id = entity.Id,
                    Name = entity.Name
                };
            
                await this.sampleDbContext.Cars.AddAsync(car);                
            }
            else
            {
                car.Name = entity.Name;
            }

            await this.sampleDbContext.SaveChangesAsync();
        }
    }
}