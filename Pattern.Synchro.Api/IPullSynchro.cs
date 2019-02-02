using System.Collections.Generic;

namespace Pattern.Synchro.Sample.Api
{
    public interface IPullSynchro
    {
        List<IEntity> GetPull();
    }

    public interface IEntity
    {
    }
}