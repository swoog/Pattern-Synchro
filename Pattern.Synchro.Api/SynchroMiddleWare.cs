using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Pattern.Synchro.Sample.Api
{
    public class SynchroMiddleWare  : IMiddleware
    {
        private readonly IPullSynchro pullSynchro;

        public SynchroMiddleWare(IPullSynchro pullSynchro)
        {
            this.pullSynchro = pullSynchro;
        }
        
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Path.Value.StartsWith("/synchro"))
            {
                var cars = this.pullSynchro.GetPull();

                context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(cars)));
                return;
            }

            await next(context);
        }    
    }
}