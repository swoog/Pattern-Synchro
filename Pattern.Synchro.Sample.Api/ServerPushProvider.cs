using System.Threading.Tasks;
using Pattern.Synchro.Client;

namespace Pattern.Synchro.Sample.Api
{
    public class ServerPushProvider : IServerPushProvider
    {
        private readonly SampleDbContext sampleDbContext;

        public ServerPushProvider(SampleDbContext sampleDbContext)
        {
            this.sampleDbContext = sampleDbContext;
        }

        public Task<bool> CanPush(IEntity entity)
        {
            return Task.FromResult(entity is Client.Car);
        }
        
        public async Task Push(IEntity entity)
        {
            var carDto = (Client.Car) entity;
            var car = new Car
            {
                Id = carDto.Id,
                Name = carDto.Name
            };
            
            await this.sampleDbContext.Cars.AddAsync(car);

            await this.sampleDbContext.SaveChangesAsync();
        }
    }
}