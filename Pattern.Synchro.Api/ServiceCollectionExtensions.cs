using Microsoft.Extensions.DependencyInjection;

namespace Pattern.Synchro.Api
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSynchro(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IDateTimeService, DateTimeService>();
            serviceCollection.AddTransient<SynchroMiddleWare>();
            serviceCollection.AddTransient<IServerPushSynchro, PushSynchro>();
        }
    }
}