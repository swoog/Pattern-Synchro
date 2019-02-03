using System.Threading.Tasks;
using Pattern.Synchro.Client;

namespace Pattern.Synchro.Api.Push
{
    public abstract class ServerPushProviderBase<T> : IServerPushProvider
    {
        public Task<bool> CanPush(IEntity entity)
        {
            return Task.FromResult(entity is T);
        }

        public Task Push(IEntity entity)
        {
            return this.Push((T) entity);
        }

        protected abstract Task Push(T entity);
    }
}