using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Pattern.Synchro.Sample.Api;
using Xunit;
using Car = Pattern.Synchro.Sample.Client.Car;

namespace Pattern.Synchro.Tests
{
    public class PushCarTests : BaseTests
    {
        public PushCarTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }
        
        [Fact]
        public async Task Should_Car_In_Local_Is_Updated_When_Pull_From_Server()
        {
            var newGuid = Guid.NewGuid();
            await this.AddLocal(new Car
            {
                Id = newGuid,
                Name = "Megane IV"
            });
            
            await this.client.Run();

            await this.AssertServer<Car>(c => "Megane IV" == c.Name);
        }       
    }
}