using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Imagegram.Repositories
{
    public interface IRepository<TEntity>
    {
        IEnumerable<TEntity> GetAll();
        IQueryable<TEntity> GetQueryable();
        IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "");
        TEntity GetById(int id);
        void Add(TEntity entity);
        void Update(TEntity entity);
        void UpdateWithExceptions(TEntity entityToUpdate,string[] excludeColumns);
        void Delete(TEntity entity);
        void DeleteAll(ICollection<TEntity> entitiesToDelete);
    }
}
