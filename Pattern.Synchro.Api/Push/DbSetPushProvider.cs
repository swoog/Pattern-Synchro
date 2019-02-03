using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pattern.Synchro.Client;

namespace Pattern.Synchro.Api.Push
{
    public abstract class DbSetPushProvider<T, TModel, TDto> : ServerPushProviderBase<TDto> 
        where T : DbContext
        where TModel : class, IEntity, new()
        where TDto : IEntity
    {
        private readonly T db;

        protected DbSetPushProvider(T db)
        {
            this.db = db;
        }

        protected abstract DbSet<TModel> GetDbSet(T db);
        
        protected override async Task Push(TDto entity)
        {
            var car = await this.GetDbSet(this.db).FindAsync(entity.Id);

            if (car == null)
            {
                car = new TModel
                {
                    Id = entity.Id,
                };

                this.UpdateProperties(entity, car);
                
                await this.GetDbSet(this.db).AddAsync(car);                
            }
            else
            {
                this.UpdateProperties(entity, car);
            }

            await this.db.SaveChangesAsync();
        }

        protected abstract void UpdateProperties(TDto entity, TModel car);
    }
}