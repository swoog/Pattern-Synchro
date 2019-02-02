using System.Threading.Tasks;
using Pattern.Synchro.Client;

namespace Pattern.Synchro.Sample.Api
{
    public interface IServerPushProvider
    {
        Task<bool> CanPush(IEntity entity);

        Task Push(IEntity entity);
    }
}