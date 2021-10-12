using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Repository
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        void Add(TEntity item);

        TEntity FindById(object id);

        IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate);
        TEntity FindFirst(Expression<Func<TEntity, bool>> predicate);
        void Remove(TEntity item);

        void Update(TEntity item);

        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);

        IEnumerable<TEntity> GetAllData();
    }

}
