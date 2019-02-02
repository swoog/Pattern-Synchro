using System;
using Pattern.Synchro.Client;

namespace Pattern.Synchro.Sample.Client
{
    public class Car : IEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}