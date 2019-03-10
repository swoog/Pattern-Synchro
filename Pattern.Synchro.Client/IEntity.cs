using System;
using Newtonsoft.Json;

namespace Pattern.Synchro.Client
{
    [JsonConverter(typeof(SynchroConverter))]
    public interface IEntity
    {
        Guid Id { get; set; }

        DateTime LastUpdated { get; set; }
    }
}