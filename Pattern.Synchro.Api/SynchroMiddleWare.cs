using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Pattern.Synchro.Api.Pull;
using Pattern.Synchro.Api.Push;
using Pattern.Synchro.Client;

namespace Pattern.Synchro.Api
{
    public class SynchroMiddleWare  : IMiddleware
    {
        private readonly IPullSynchro pullSynchro;
        private readonly IServerPushSynchro serverPushSynchro;
        private readonly IDeviceInformation deviceInformation;
        private readonly IDateTimeService dateTimeService;

        public SynchroMiddleWare(
            IPullSynchro pullSynchro, 
            IServerPushSynchro serverPushSynchro, 
            IDeviceInformation deviceInformation,
            IDateTimeService dateTimeService)
        {
            this.pullSynchro = pullSynchro;
            this.serverPushSynchro = serverPushSynchro;
            this.deviceInformation = deviceInformation;
            this.dateTimeService = dateTimeService;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Path.Value.StartsWith("/synchro/end"))
            {
                var deviceId = Guid.Parse(context.Request.Query["deviceId"]);
                var streamReader = new StreamReader(context.Request.Body);

                var entities = JsonConvert.DeserializeObject<SynchroDevice>(streamReader.ReadToEnd(),
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All,
                        PreserveReferencesHandling = PreserveReferencesHandling.All
                    });
                await this.deviceInformation.SaveLastSynchro(deviceId, entities.BeginServerDateTime);
                return;
            }

            if (context.Request.Path.Value.StartsWith("/synchro/begin"))
            {
                var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new SynchroDevice
                    {
                        BeginServerDateTime = this.dateTimeService. DateTimeNow()
                    },
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All,
                        PreserveReferencesHandling = PreserveReferencesHandling.All
                    }));
                await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                return;
            }

            if (context.Request.Path.Value.StartsWith("/synchro"))
            {
                switch (context.Request.Method.ToUpperInvariant())
                {
                    case "GET":
                        var deviceId = Guid.Parse(context.Request.Query["deviceId"]);
                        var lastSynchro = await this.deviceInformation.GetLastSynchro(deviceId) ?? DateTime.MinValue;

                        var cars = this.pullSynchro.GetPull(context, lastSynchro);

                        var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(cars,
                            new JsonSerializerSettings
                            {
                                TypeNameHandling = TypeNameHandling.All,
                                PreserveReferencesHandling = PreserveReferencesHandling.All
                            }));
                        await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                        return;
                    case "POST":
                        var streamReader = new StreamReader(context.Request.Body);

                        var entities = JsonConvert.DeserializeObject<List<IEntity>>(streamReader.ReadToEnd(),
                            new JsonSerializerSettings
                            {
                                TypeNameHandling = TypeNameHandling.All,
                                PreserveReferencesHandling = PreserveReferencesHandling.All
                            });
                        await this.serverPushSynchro.Push(context, entities);
                        return;
                }
            }

            await next(context);
        }
    }
}