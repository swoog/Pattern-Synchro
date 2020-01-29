using System.Collections.Generic;

namespace Pattern.Synchro.Client
{
    public interface ISyncCallback
    {
        void SyncEvents(SyncEvent @event, List<IEntity> entities);
    }
}