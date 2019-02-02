using System;

namespace Pattern.Synchro.Api
{
    public interface IDeviceInformation
    {
        DateTime? GetLastSynchro(Guid deviceId);
    }
}