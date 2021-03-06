using System;
using System.Threading.Tasks;
using Pattern.Synchro.Api;

namespace Pattern.Synchro.Sample.Api
{
    public class DeviceInformation : IDeviceInformation
    {
        private readonly SampleDbContext db;

        public DeviceInformation(SampleDbContext db)
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
                device = new Device()
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
}