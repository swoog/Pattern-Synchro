using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SQLite;

namespace Pattern.Synchro.Client
{
    public class SynchroClient
    {
        private readonly HttpClient httpClient;
        private readonly SQLiteAsyncConnection db;
        private readonly IEnumerable<IClientPushSynchro> clientPushSynchro;
        private ISyncCallback syncCallback;

        public SynchroClient(HttpClient httpClient, SQLiteAsyncConnection db, IEnumerable<IClientPushSynchro> clientPushSynchro)
        {
            this.httpClient = httpClient;
            this.db = db;
            this.clientPushSynchro = clientPushSynchro;
        }

        public Guid DeviceId { get; set; }

        public async Task Run(int version = 0, Dictionary<string, string> headers = null)
        {
            await this.SyncEvents(SyncEvent.Begin, null);

            try
            {
                var beginLocalDateTime = DateTime.Now;
                var synchroDevice = await this.Begin(headers);

                var pullEntities = await this.Pull(version);

                await this.Push(synchroDevice, version);

                synchroDevice.LastLocalSyncDateTime = beginLocalDateTime;
                synchroDevice.Version = version;
                await this.End(synchroDevice);

                await this.SyncEvents(SyncEvent.End, pullEntities);
            }
            catch (HttpRequestException requestException)
            {
                await this.SyncEvents(SyncEvent.End, new List<IEntity>());
            }
        }

        private async Task SyncEvents(SyncEvent @event, List<IEntity> entities)
        {
            await (this.syncCallback?.SyncEvents(@event, entities) ?? Task.CompletedTask);
        }

        private async Task End(SynchroDevice synchroDevice)
        {
            var json = JsonConvert.SerializeObject(synchroDevice, new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.All
            });

            await this.httpClient.PostAsync($"/synchro/end?deviceId={this.DeviceId}", new StringContent(json, Encoding.UTF8, "application/json"));
        }

        private async Task<SynchroDevice> Begin(Dictionary<string, string> headers)
        {
            using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get,
                $"/synchro/begin?deviceId={this.DeviceId}"))
            {
                if (headers != null)
                {
                    foreach (var key in headers.Keys)
                    {
                        httpRequestMessage.Headers.Add(key, headers[key]);
                    }
                }
                
                var httpResponseMessage = await this.httpClient.SendAsync(httpRequestMessage);
                
                var response = await httpResponseMessage.Content.ReadAsStringAsync();

                var synchroDevice = JsonConvert.DeserializeObject<SynchroDevice>(response, new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.All
                });

                return synchroDevice;               
            }
        }

        private async Task Push(SynchroDevice synchroDevice, int version)
        {
            var lastUpdated = synchroDevice.LastLocalSyncDateTime;
            var entities = await this.GetPushEntities(lastUpdated);

            var json = JsonConvert.SerializeObject(entities, new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.All
            });

            await this.httpClient.PostAsync($"/synchro?deviceId={this.DeviceId}&version={version}", new StringContent(json, Encoding.UTF8, "application/json"));
        }

        private async Task<List<IEntity>> GetPushEntities(DateTime lastUpdated)
        {
            var entities = new List<IEntity>();
            
            foreach (var pushSynchro in this.clientPushSynchro)
            {
                entities.AddRange(await pushSynchro.GetEntities(lastUpdated));
            }

            return entities;
        }

        private async Task<List<IEntity>> Pull(int version)
        {
            var response = await this.httpClient.GetStringAsync($"/synchro?deviceId={this.DeviceId}&version={version}");

            var cars = JsonConvert.DeserializeObject<List<IEntity>>(response, new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.All
            });

            foreach (var car in cars)
            {
                if (car.IsDeleted)
                {
                    await this.db.DeleteAsync(car);
                }
                else
                {
                    await this.db.InsertOrReplaceAsync(car);
                }
            }

            return cars;
        }

        public async Task SetCallback(ISyncCallback syncCallback)
        {
            this.syncCallback = syncCallback;
        }
    }
}