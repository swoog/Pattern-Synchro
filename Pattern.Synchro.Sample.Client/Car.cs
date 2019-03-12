using System;
using Pattern.Synchro.Client;
using SQLite;

namespace Pattern.Synchro.Sample.Client
{
    public class Car : IEntity
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        
        public DateTime LastUpdated { get; set; }

        public string Name { get; set; }
    }
}