using System;
using System.Collections.Generic;
using Pattern.Synchro.Client;

namespace Pattern.Synchro.Api.Pull
{
    public class PullSynchro : IPullSynchro
    {
        private readonly IEnumerable<IServerPullProvider> serverPullProviders;

        public PullSynchro(IEnumerable<IServerPullProvider> serverPullProviders)
        {
            this.serverPullProviders = serverPullProviders;
        }

        public List<IEntity> GetPull(DateTime lastSynchro)
        {
            var entities = new List<IEntity>();
            foreach (var serverPullProvider in this.serverPullProviders)
            {
                entities.AddRange(serverPullProvider.GetPull(lastSynchro));
            }

            return entities;
        }
    }
}