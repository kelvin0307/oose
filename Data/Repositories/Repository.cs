using System.Collections;
using System.Linq.Expressions;
using Data.Interfaces.Repositories;
using Data.Context;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where
            TEntity : class
    {
        protected DbSet<TEntity> DbSet;
        private readonly DataContext _context;
        private IQueryable<TEntity> _queryable => DbSet;
        public Type ElementType => _queryable.ElementType;

        public Expression Expression => _queryable.Expression;

        public IQueryProvider Provider => _queryable.Provider;

        public IEnumerator<TEntity> GetEnumerator()
        {
            return _queryable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _queryable.GetEnumerator();
        }

        public Repository(DataContext context)
        {
            _context = context;

            DbSet = context.Set<TEntity>();
        }

        public async Task<TEntity> CreateAndCommit(TEntity entity)
        {
            _context.Add(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public void Delete(int id)
        {
            var entity = _context.Set<TEntity>().Find(id);
            if (entity != null)
            {
                _context.Set<TEntity>().Remove(entity);
            }
        }

        public void Update(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
        }

        public void Create(List<TEntity> entity)
        {
            _context.Set<List<TEntity>>().Add(entity);
        }

        public async Task<TEntity?> Get(int id)
        {
            return await DbSet.FindAsync(id);
        }

        public async Task<TEntity?> Get(Expression<Func<TEntity, bool>> action)
        {
            return await FirstOrDefaultAsyncWrapper(DbSet, action);
        }

        public async Task<List<TEntity>> GetAll()
        {
            return await ToListAsyncWrapper(DbSet);
        }

        public async Task<List<TEntity>> GetAll(Expression<Func<TEntity, bool>> action)
        {
            return await ToListAsyncWrapper(WhereWrapper(DbSet, action));
        }

        public async Task SaveManually()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<TEntity> UpdateAndCommit(TEntity entity)
        {
            DbSet.Update(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task<TEntity> DeleteAndCommit(int id)
        {
            try
            {
                // Use the FindAsync overload with object[] and CancellationToken to match test mocks
                var valueTask = await DbSet.FindAsync(new object[] { id }, CancellationToken.None);
                var objectData = valueTask ?? throw new Exception($"Object with id:{id} not found.");

                DbSet.Remove(objectData);
                await _context.SaveChangesAsync();
                return objectData;
            }
            catch (Exception)
            {
                //TODO: add logging with message and stacktrace
                throw;
            }
        }

        public async Task<List<TEntity>> ToListAsync(IQueryable<TEntity> query)
        {
            return await ToListAsyncWrapper(query);
        }

        public async Task<TEntity?> FirstOrDefaultAsync(IQueryable<TEntity> query)
        {
            return await FirstOrDefaultAsyncWrapper(query);
        }

        public IQueryable<TEntity> Include<TProperty>(Expression<Func<TEntity, TProperty>> navigationProperty)
        {
            return IncludeWrapper(DbSet, navigationProperty);
        }

        public IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return WhereWrapper(DbSet, predicate);
        }

        // Virtual wrapper methods for EF Core extension methods to make them mockable

        #region wrapper methods
        public virtual async Task<List<TEntity>> ToListAsyncWrapper(IQueryable<TEntity> query)
        {
            return await query.ToListAsync();
        }

        public virtual async Task<TEntity?> FirstOrDefaultAsyncWrapper(IQueryable<TEntity> query)
        {
            return await query.FirstOrDefaultAsync();
        }

        public virtual async Task<TEntity?> FirstOrDefaultAsyncWrapper(IQueryable<TEntity> query, Expression<Func<TEntity, bool>> predicate)
        {
            return await query.FirstOrDefaultAsync(predicate);
        }

        public virtual IQueryable<TEntity> WhereWrapper(IQueryable<TEntity> query, Expression<Func<TEntity, bool>> predicate)
        {
            return query.Where(predicate);
        }

        public virtual IQueryable<TEntity> IncludeWrapper<TProperty>(IQueryable<TEntity> query, Expression<Func<TEntity, TProperty>> navigationProperty)
        {
            return query.Include(navigationProperty);
        }
        #endregion
    }
}
