using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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

        public async Task<DateTime?> GetLastSynchro(HttpContext context, Guid deviceId)
        {
            var device = await this.db.Devices.FindAsync(deviceId);

            this.Verify(context, device);

            return device?.LastSynchro;
        }
        
        public async Task<DateTime?> GetLastLocalSynchro(HttpContext context, Guid deviceId)
        {
            var device = await this.db.Devices.FindAsync(deviceId);

            this.Verify(context, device);

            var deviceLastLocalSynchro = device?.LastLocalSynchro;

            if (deviceLastLocalSynchro.HasValue)
            {
                return new DateTime(deviceLastLocalSynchro.Value.Ticks, DateTimeKind.Utc);
            }

            return null;
        }

        public async Task<int?> GetVersion(HttpContext context, Guid deviceId)
        {
            var device = await this.db.Devices.FindAsync(deviceId);

            this.Verify(context, device);

            return device?.Version;
        }

        public async Task SaveLastSynchro(HttpContext context, Guid deviceId, DateTime dateTime,
            DateTime lastLocalSyncDateTime, int version)
        {
            var device = await this.db.Devices.FindAsync(deviceId);

            this.Verify(context, device);

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

            this.SaveLastSynchro(context, device);

            await this.db.SaveChangesAsync();
        }

        protected virtual void SaveLastSynchro(HttpContext context, Device device)
        {
            
        }
        
        protected virtual void Verify(HttpContext context, Device device)
        {
            
        }
    }

    public interface IDeviceDbContext
    {
        DbSet<Device> Devices { get; set; }
    }
}