using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Pattern.Synchro.Client;
using SQLite;

namespace Pattern.Synchro.Sample.Client
{
    public class SynchroClient
    {
        private readonly HttpClient httpClient;
        private readonly SQLiteAsyncConnection db;

        public SynchroClient(HttpClient httpClient, SQLiteAsyncConnection db)
        {
            this.httpClient = httpClient;
            this.db = db;
        }

        public async Task Run()
        {
            var response = await this.httpClient.GetStringAsync("/synchro");

            var cars = JsonConvert.DeserializeObject<List<IEntity>>(response, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            });

            foreach (var car in cars)
            {
                await this.db.InsertOrReplaceAsync(car);
            }
        }
    }
}