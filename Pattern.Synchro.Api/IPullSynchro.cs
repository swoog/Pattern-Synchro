using System;
using System.Collections.Generic;
using Pattern.Synchro.Client;

namespace Pattern.Synchro.Api
{
    public interface IPullSynchro
    {
        List<IEntity> GetPull(DateTime lastSynchro);
    }
}