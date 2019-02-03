using System.Collections.Generic;
using System.Threading.Tasks;
using Pattern.Synchro.Client;

namespace Pattern.Synchro.Api.Push
{
    public interface IServerPushSynchro
    {
        Task Push(List<IEntity> entities);
    }
}