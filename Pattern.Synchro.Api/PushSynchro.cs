using System.Collections.Generic;
using System.Threading.Tasks;
using Pattern.Synchro.Client;

namespace Pattern.Synchro.Api
{
    public class PushSynchro : IServerPushSynchro
    {
        private readonly IEnumerable<IServerPushProvider> pushProviders;

        public PushSynchro(IEnumerable<IServerPushProvider> pushProviders)
        {
            this.pushProviders = pushProviders;
        }
        
        public async Task Push(List<IEntity> entities)
        {
            foreach (var entity in entities)
            {
                foreach (var pushProvider in this.pushProviders)
                {
                    if (await pushProvider.CanPush(entity))
                    {
                        await pushProvider.Push(entity);
                    }   
                }
            }
        }
    }
}