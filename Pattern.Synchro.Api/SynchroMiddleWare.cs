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
        private readonly IServerCallback serverCallback;

        public SynchroMiddleWare(
            IPullSynchro pullSynchro, 
            IServerPushSynchro serverPushSynchro, 
            IDeviceInformation deviceInformation,
            IDateTimeService dateTimeService,
            IServerCallback serverCallback)
        {
            this.pullSynchro = pullSynchro;
            this.serverPushSynchro = serverPushSynchro;
            this.deviceInformation = deviceInformation;
            this.dateTimeService = dateTimeService;
            this.serverCallback = serverCallback;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Path.Value.StartsWith("/synchro/end"))
            {
                var deviceId = Guid.Parse(context.Request.Query["deviceId"]);
                var streamReader = new StreamReader(context.Request.Body);

                var entities = JsonConvert.DeserializeObject<SynchroDevice>(await streamReader.ReadToEndAsync().ConfigureAwait(false),
                    new JsonSerializerSettings
                    {
                        PreserveReferencesHandling = PreserveReferencesHandling.All
                    });
                await this.deviceInformation.SaveLastSynchro(deviceId, entities.BeginServerDateTime, entities.LastLocalSyncDateTime).ConfigureAwait(false);
                return;
            }

            if (context.Request.Path.Value.StartsWith("/synchro/begin"))
            {
                await this.serverCallback.Begin(context.Request.Headers).ConfigureAwait(false);
                
                var deviceId = Guid.Parse(context.Request.Query["deviceId"]);
                var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new SynchroDevice
                    {
                        BeginServerDateTime = this.dateTimeService.DateTimeNow(),
                        LastLocalSyncDateTime = (await this.deviceInformation.GetLastLocalSynchro(deviceId)) ?? DateTime.MinValue
                    },
                    new JsonSerializerSettings
                    {
                        PreserveReferencesHandling = PreserveReferencesHandling.All
                    }));
                await context.Response.Body.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
                return;
            }

            if (context.Request.Path.Value.StartsWith("/synchro"))
            {
                switch (context.Request.Method.ToUpperInvariant())
                {
                    case "GET":
                        var deviceId = Guid.Parse(context.Request.Query["deviceId"]);
                        var lastSynchro = await this.deviceInformation.GetLastSynchro(deviceId).ConfigureAwait(false) ?? DateTime.MinValue;

                        var cars = this.pullSynchro.GetPull(context, lastSynchro);

                        var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(cars,
                            new JsonSerializerSettings
                            {
                                PreserveReferencesHandling = PreserveReferencesHandling.All
                            }));
                        await context.Response.Body.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
                        return;
                    case "POST":
                        var streamReader = new StreamReader(context.Request.Body);

                        var entities = JsonConvert.DeserializeObject<List<IEntity>>(await streamReader.ReadToEndAsync().ConfigureAwait(false),
                            new JsonSerializerSettings
                            {
                                PreserveReferencesHandling = PreserveReferencesHandling.All
                            });
                        await this.serverPushSynchro.Push(context, entities).ConfigureAwait(false);
                        return;
                }
            }

            await next(context).ConfigureAwait(false);
        }
    }
}