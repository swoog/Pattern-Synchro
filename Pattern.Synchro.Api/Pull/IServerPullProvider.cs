using System;
using System.Collections.Generic;
using Pattern.Synchro.Client;

namespace Pattern.Synchro.Api.Pull
{
    public interface IServerPullProvider
    {
        List<IEntity> GetPull(DateTime lastSynchro);
    }
}