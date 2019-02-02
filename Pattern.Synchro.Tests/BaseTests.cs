using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pattern.Synchro.Sample.Api;
using Pattern.Synchro.Sample.Client;
using SQLite;
using Xunit;
using Car = Pattern.Synchro.Sample.Client.Car;

namespace Pattern.Synchro.Tests
{
    public class BaseTests : IClassFixture<WebApplicationFactory<Startup>>, IDisposable
    {
        private readonly HttpClient httpClient;
        private readonly SampleDbContext entity;
        private readonly string databaseName;
        private SQLiteAsyncConnection localDb;
        protected SynchroClient client;

        public BaseTests(WebApplicationFactory<Startup> factory)
        {
            this.databaseName = this.GetType().Name;
            this.httpClient = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    EntityFrameworkServiceCollectionExtensions.AddDbContext<SampleDbContext>(services, opt =>
                        SqliteDbContextOptionsBuilderExtensions.UseSqlite(opt, $"Data Source={this.databaseName}"));
                });
            }).CreateClient();

            var options = new DbContextOptionsBuilder<SampleDbContext>()
                .UseSqlite($"Data Source={this.databaseName}")
                .Options;

            this.entity = new SampleDbContext(options);
            this.entity.Database.EnsureCreated();

            this.localDb = new SQLiteAsyncConnection(":memory:");
            Task.WaitAll(this.localDb.CreateTableAsync<Car>());

            this.client = new SynchroClient(this.httpClient, this.localDb);
        }
        
        protected async Task AddServer<T>(T obj) where T : class
        {
            await this.entity.Set<T>().AddAsync(obj);
            await this.entity.SaveChangesAsync();
        }

        protected async Task AssertLocal<T>(Predicate<T> predicate) where T : new()
        {
            var car = await this.localDb.Table<T>().ToListAsync();

            Xunit.Assert.NotNull(car);
            Xunit.Assert.Contains(car, predicate);
        }
        
        public void Dispose()
        {
            File.Delete(this.databaseName);
        }
    }
}