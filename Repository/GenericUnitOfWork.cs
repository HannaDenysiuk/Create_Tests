using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Repository
{
    public class GenericUnitOfWork : IDisposable
    {
        // Initialize code 

        DbContext context;

        public GenericUnitOfWork(DbContext context)
        {
            this.context = context;
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }

        public Dictionary<Type, object> repositories = new Dictionary<Type, object>();

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            //перший раз створює, інші рази просто користується
            if (repositories.Keys.Contains(typeof(TEntity)) == true)
            {
                return repositories[typeof(TEntity)] as IGenericRepository<TEntity>;
            }

            IGenericRepository<TEntity> repo = new EFGenericRepository<TEntity>(context);
            repositories.Add(typeof(TEntity), repo);
            return repo;
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }

}
