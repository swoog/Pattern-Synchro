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

            var deviceLastLocalSynchro = device?.LastLocalSynchro;

            if (deviceLastLocalSynchro.HasValue)
            {
                return new DateTime(deviceLastLocalSynchro.Value.Ticks, DateTimeKind.Utc);
            }

            return null;
        }

        public async Task<int?> GetVersion(Guid deviceId)
        {
            var device = await this.db.Devices.FindAsync(deviceId);

            return device?.Version;
        }

        public async Task SaveLastSynchro(Guid deviceId, DateTime dateTime, DateTime lastLocalSyncDateTime, int version)
        {
            var device = await this.db.Devices.FindAsync(deviceId);

            if (device == null)
            {
                device = new Device
                {
                    Id = deviceId,
                    LastSynchro = dateTime,
                    LastLocalSynchro = lastLocalSyncDateTime,
                    Version = version
                };

                await this.db.Devices.AddAsync(device);
            }
            else
            {
                device.LastSynchro = dateTime;
                device.LastLocalSynchro = lastLocalSyncDateTime;
                device.Version = version;
            }

            await this.db.SaveChangesAsync();
        }
    }

    public interface IDeviceDbContext
    {
        DbSet<Device> Devices { get; set; }
    }
}