using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Pattern.Synchro.Sample.Api;
using Xunit;

namespace Pattern.Synchro.Tests
{
    public class DeviceTests : BaseTests
    {
        public DeviceTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }
        
        [Fact]
        public async Task Should_Device_Is_Updated_On_Server_When_Synchro_End()
        {
            await this.client.Run();

            await this.AssertServer<Device>(c => c.LastUpdated == new DateTime(2019,2,2,10,00,45));
        }        
    }
}