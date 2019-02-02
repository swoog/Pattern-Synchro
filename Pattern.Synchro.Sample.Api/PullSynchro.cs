using System.Collections.Generic;
using System.Linq;

namespace Pattern.Synchro.Sample.Api
{
    public class PullSynchro : IPullSynchro
    {
        private readonly SampleDbContext sampleDbContext;

        public PullSynchro(SampleDbContext sampleDbContext)
        {
            this.sampleDbContext = sampleDbContext;
        }

        public List<Car> GetPull()
        {
            return this.sampleDbContext.Cars.ToList();
        }
    }
}