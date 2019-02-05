using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Pattern.Synchro.Api
{
    public class DeviceInformation<TDbContext> : IDeviceInformation
        where TDbContext : DbContext, IDeviceDbContext
    {
        private readonly TDbContext db;

        public DeviceInformation(TDbContext db)
        {
            this.db = db;
        }

        public async Task<DateTime?> GetLastSynchro(Guid deviceId)
        {
            var device = await this.db.Devices.FindAsync(deviceId);

            return device?.LastSynchro;
        }

        public async Task SaveLastSynchro(Guid deviceId, DateTime dateTime)
        {
            var device = await this.db.Devices.FindAsync(deviceId);

            if (device == null)
            {
                device = new Device
                {
                    Id = deviceId,
                    LastSynchro = dateTime
                };

                await this.db.Devices.AddAsync(device);
            }
            else
            {
                device.LastSynchro = dateTime;                
            }

            await this.db.SaveChangesAsync();
        }
    }

    public interface IDeviceDbContext
    {
        DbSet<Device> Devices { get; set; }
    }
}