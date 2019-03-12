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

        public async Task Run()
        {
            this.syncCallback?.SyncEvents(SyncEvent.Begin, null);
            var synchroDevice = await this.Begin();

            var pullEntities = await this.Pull();

            await this.Push();

            await this.End(synchroDevice);

            this.syncCallback?.SyncEvents(SyncEvent.End, pullEntities);
        }

        private async Task End(SynchroDevice synchroDevice)
        {
            var json = JsonConvert.SerializeObject(synchroDevice, new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.All
            });

            await this.httpClient.PostAsync($"/synchro/end?deviceId={this.DeviceId}", new StringContent(json, Encoding.UTF8, "application/json"));
        }

        private async Task<SynchroDevice> Begin()
        {
            var response =  await this.httpClient.GetStringAsync($"/synchro/begin?deviceId={this.DeviceId}");
            
            var synchroDevice = JsonConvert.DeserializeObject<SynchroDevice>(response, new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.All
            });

            return synchroDevice;
        }

        private async Task Push()
        {
            var entities = await this.GetPushEntities();

            var json = JsonConvert.SerializeObject(entities, new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.All
            });

            await this.httpClient.PostAsync($"/synchro?deviceId={this.DeviceId}", new StringContent(json, Encoding.UTF8, "application/json"));
        }

        private async Task<List<IEntity>> GetPushEntities()
        {
            var entities = new List<IEntity>();
            
            foreach (var pushSynchro in this.clientPushSynchro)
            {
                entities.AddRange(await pushSynchro.GetEntities());
            }

            return entities;
        }

        private async Task<List<IEntity>> Pull()
        {
            var response = await this.httpClient.GetStringAsync($"/synchro?deviceId={this.DeviceId}");

            var cars = JsonConvert.DeserializeObject<List<IEntity>>(response, new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.All
            });

            foreach (var car in cars)
            {
                await this.db.InsertOrReplaceAsync(car);
            }

            return cars;
        }

        public async Task SetCallback(ISyncCallback syncCallback)
        {
            this.syncCallback = syncCallback;
        }
    }
}