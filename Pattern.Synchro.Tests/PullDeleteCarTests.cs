using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Pattern.Synchro.Sample.Api;
using Xunit;

namespace Pattern.Synchro.Tests
{
    public class PullDeleteCarTests : BaseTests
    {
        public PullDeleteCarTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task Should_Delete_Car_On_Mobile_When_Pull_Deleted_Car()
        {
            var newGuid = Guid.NewGuid();
            await this.AddLocal(new Sample.Client.Car
            {
                Id = newGuid,
                Name = "Megane IV"
            }).ConfigureAwait(false);
            
            await this.AddServer(new Car
            {
                Id = newGuid,
                Name = "Megane IV",
                UserId = "1",
                IsDeleted = true,
            }).ConfigureAwait(false);

            await this.client.Run().ConfigureAwait(false);

            await this.AssertHave<Car>(0).ConfigureAwait(false);
        }
    }
}