using System;
using System.Threading.Tasks;

namespace Pattern.Synchro.Api
{
    public interface IDeviceInformation
    {
        Task<DateTime?> GetLastSynchro(Guid deviceId);
        Task SaveLastSynchro(Guid deviceId, DateTime dateTime);
    }
}