using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Pattern.Synchro.Api;
using Pattern.Synchro.Sample.Api;
using Xunit;

namespace Pattern.Synchro.Tests
{
    public class PushCarTests : BaseTests
    {
        public PushCarTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }
        
        [Fact]
        public async Task Should_Car_In_Server_Is_Add_When_Push_From_Mobile()
        {
            var newGuid = Guid.NewGuid();
            await this.AddLocal(new Sample.Client.Car
            {
                Id = newGuid,
                Name = "Megane IV"
            });
            
            await this.client.Run();

            await this.AssertServer<Car>(c => "Megane IV" == c.Name);
        }
        
        [Fact]
        public async Task Should_Car_Has_UserId_When_Push_From_Mobile()
        {
            var newGuid = Guid.NewGuid();
            await this.AddLocal(new Sample.Client.Car
            {
                Id = newGuid,
                Name = "Megane IV"
            });
            
            await this.client.Run();

            await this.AssertServer<Car>(c => "1" == c.UserId);
        }
        
        [Fact]
        public async Task Should_Car_In_Local_Is_Updated_When_Pull_From_Server()
        {
            await this.AddServer(new Device
            {
                Id = this.deviceId,
                LastSynchro = new DateTime(2019, 2, 2)
            });
            
            var newGuid = Guid.NewGuid();
            await this.AddServer(new Car
            {
                Id = newGuid,
                Name = "Megane 4",
                LastUpdated = new DateTime(2019, 2, 1)
            });
            await this.AddLocal(new Sample.Client.Car
            {
                Id = newGuid,
                Name = "Megane IV"
            });
            
            await this.client.Run();

            await this.AssertServer<Car>(c => "Megane IV" == c.Name);
        }
    }
}