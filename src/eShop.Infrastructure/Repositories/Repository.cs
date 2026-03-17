using eShop.Infrastructure.Context;
using eShop.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace eShop.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ShopContext _ctx;
        protected readonly DbSet<T> DbSet;

        public Repository(ShopContext ctx)
        {
            _ctx = ctx;
            DbSet = _ctx.Set<T>();
        }

        public IEnumerable<T> GetAll(
            Expression<Func<T, bool>>? filter = null,
            string? includeProperties = null)
        {
            IQueryable<T> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProperty in includeProperties
                             .Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty.Trim());
                }
            }

            return query.ToList();
        }

        public T? Get(
            Expression<Func<T, bool>> filter,
            string? includeProperties = null,
            bool tracked = false)
        {
            IQueryable<T> query = tracked ? DbSet : DbSet.AsNoTracking();

            query = query.Where(filter);

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProperty in includeProperties
                             .Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty.Trim());
                }
            }

            return query.FirstOrDefault();
        }

        public void Add(T entity)
        {
            DbSet.Add(entity);
        }

        public void Remove(T entity)
        {
            DbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            DbSet.RemoveRange(entities);
        }
    }
}
