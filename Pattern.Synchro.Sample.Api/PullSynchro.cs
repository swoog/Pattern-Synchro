using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Pattern.Synchro.Api;
using Pattern.Synchro.Client;

namespace Pattern.Synchro.Sample.Api
{
    public class PullSynchro : IPullSynchro
    {
        private readonly SampleDbContext sampleDbContext;

        public PullSynchro(SampleDbContext sampleDbContext)
        {
            this.sampleDbContext = sampleDbContext;
        }

        public List<IEntity> GetPull(DateTime lastSynchro)
        {
            return this.sampleDbContext.Cars
                .Where(c => c.LastUpdated >= lastSynchro)
                .Select(c => new Client.Car
                {
                    Id = c.Id,
                    Name = c.Name
                }).Cast<IEntity>().ToList();
        }
    }
}