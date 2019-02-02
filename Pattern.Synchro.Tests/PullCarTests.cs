using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Pattern.Synchro.Sample.Api;
using Xunit;
using Car = Pattern.Synchro.Sample.Client.Car;

namespace Pattern.Synchro.Tests
{
    public class PullCarTests : BaseTests
    {
        public PullCarTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task Should_Have_Car_In_Local_Database_When_Pull_From_Server()
        {
            await this.AddServer(new Sample.Api.Car
            {
                Id = Guid.NewGuid(),
                Name = "Megane IV"
            });

            await this.client.Run();

            await this.AssertLocal<Car>(c => "Megane IV" == c.Name);
        }
        
        [Fact]
        public async Task Should_Car_In_Local_Is_Updated_When_Pull_From_Server()
        {
            var newGuid = Guid.NewGuid();
            await this.AddServer(new Sample.Api.Car
            {
                Id = newGuid,
                Name = "Megane IV"
            });

            await this.AddLocal(new Car
            {
                Id = newGuid,
                Name = "Megane 4"
            });

            await this.client.Run();

            await this.AssertLocal<Car>(c => "Megane IV" == c.Name);
        }       
    }
}