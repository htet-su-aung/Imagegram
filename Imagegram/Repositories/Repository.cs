using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Imagegram.Models;
using Microsoft.EntityFrameworkCore;

namespace Imagegram.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        internal ImagegramContext context;
        internal DbSet<TEntity> dbSet;

        public Repository(ImagegramContext context)
        {
            this.context = context;

            this.dbSet = context.Set<TEntity>();
        }


        public virtual IEnumerable<TEntity> GetAll()
        {

            IQueryable<TEntity> query = dbSet;
            return query.ToList();

        }
        public virtual IQueryable<TEntity> GetQueryable()
        {
            IQueryable<TEntity> query = dbSet;
            return query;
        }
        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {

            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {

                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }
        public virtual TEntity GetById(int id)
        {
            return dbSet.Find(id);
        }

        public virtual void Add(TEntity entity)
        {
            dbSet.Add(entity);
            context.SaveChanges();
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            dbSet.Remove(entityToDelete);
            context.SaveChanges();
        }
        public virtual void DeleteAll(ICollection<TEntity> entitiesToDelete)
        {
            foreach (var entityToDelete in entitiesToDelete)
            {
                dbSet.Remove(entityToDelete);
            }
            
            context.SaveChanges();
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
            context.SaveChanges();
        }

        public virtual void UpdateWithExceptions(TEntity entityToUpdate, string[] excludeColumns)
        {
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
            foreach (var item in excludeColumns)
            {
                context.Entry(entityToUpdate).Property(item).IsModified = false;
            }
            context.SaveChanges();
        }
    }
}
