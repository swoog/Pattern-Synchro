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
        
        public async Task<DateTime?> GetLastLocalSynchro(Guid deviceId)
        {
            var device = await this.db.Devices.FindAsync(deviceId);

            return device?.LastLocalSynchro;
        }

        public async Task SaveLastSynchro(Guid deviceId, DateTime dateTime, DateTime lastLocalSyncDateTime)
        {
            var device = await this.db.Devices.FindAsync(deviceId);

            if (device == null)
            {
                device = new Device
                {
                    Id = deviceId,
                    LastSynchro = dateTime,
                    LastLocalSynchro = lastLocalSyncDateTime
                };

                await this.db.Devices.AddAsync(device);
            }
            else
            {
                device.LastSynchro = dateTime;
                device.LastLocalSynchro = lastLocalSyncDateTime;
            }

            await this.db.SaveChangesAsync();
        }
    }

    public interface IDeviceDbContext
    {
        DbSet<Device> Devices { get; set; }
    }
}